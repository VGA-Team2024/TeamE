using R3;
using R3.Triggers;
using System;
using System.Collections;
using UnityEngine;

public class ECMoveController : MonoBehaviour
{
    [SerializeField] private float _footIKWeightIncreaseRate = 0.5f;
    [SerializeField] private float _maxFootIKWeight = 0.6f;
    [SerializeField] private Vector3 _footIKOffset;
    [SerializeField] private float _forwardSpeed;
    [SerializeField , Range(0f , 1f)] private float _moveInputThreshold;
    [SerializeField , Range(0f , 1f)] private float _aimSpeedDecrease;
    [SerializeField , Range(0f , 50f)] private float _stoppingRate;
    
    [SerializeField] private float _rotateLerpRateIsNormal;
    [SerializeField] private float _rotateLerpRateIsAiming;
    [SerializeField] private float _rotateThreshold;
    
    [SerializeField] private float _jumpPower = 3.0f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private Transform _wallCheckRayPoint;
    [SerializeField] private float _wallCheckRayLength = 0.5f;
    [SerializeField] private CapsuleCollider _playerCapsule;

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _playerTransform;
    
    [SerializeField] private float _wallAngle = 55f;
    [SerializeField] private float _wallCheckRayRadius = 0.3f;
    private Vector3 _currentVelocity;

    [HideInInspector] public bool IsSwimming;
    [HideInInspector] public bool IsLanding;
    [HideInInspector] public bool IsGround;
    [HideInInspector] public bool CanWalk;
    [HideInInspector] public bool IsClimbing;
    [HideInInspector] public Vector3 GroundNormal = Vector3.up;
    private int _baseLayerIndex;
    private float _currentFootIKWeight;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int IsGroundHash = Animator.StringToHash("IsGround");
    private static readonly int InputXHash = Animator.StringToHash("InputX");
    private static readonly int InputYHash = Animator.StringToHash("InputY");
    private static readonly int SwimHash = Animator.StringToHash("Swim");
    private Vector3 _leftFootIKLineFrom, _leftFootIKLineTo;
    private Vector3 _rightFootIKLineFrom, _rightFootIKLineTo;
    private float _normalForwardSpeed;
    public Animator Animator => _animator;
    private void Start()
    {
        _normalForwardSpeed = _forwardSpeed;
        _baseLayerIndex = _animator.GetLayerIndex("BaseLayer");
        _stateMachineTrigger = _stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_baseLayerIndex];
    }
    private void Update()
    {
        var magnitude = new Vector3(_rigidBody.velocity.x , 0f , _rigidBody.velocity.z).magnitude;
        _animator.SetFloat(SpeedHash , magnitude);
        var prevX = _animator.GetFloat(InputXHash);
        var prevY = _animator.GetFloat(InputYHash);
        _animator.SetFloat(InputXHash, Mathf.SmoothStep(prevX, PlayerInputProvider.Instance.MoveValue.x, Time.deltaTime * 15f));
        _animator.SetFloat(InputYHash, Mathf.SmoothStep(prevY, PlayerInputProvider.Instance.MoveValue.y, Time.deltaTime * 15f));
        if (_currentFootIKWeight < _maxFootIKWeight) _currentFootIKWeight += Time.deltaTime * _footIKWeightIncreaseRate;
    }
    
    public void JumpStart()
    {
        if (IsSwimming) return;
        StartCoroutine(JumpCoroutine());
    }

    IEnumerator JumpCoroutine()
    {
        _jumpTrigger = true;
        yield return new WaitForFixedUpdate();
        var copy = _rigidBody.velocity;
        copy.y = _jumpPower;
        _rigidBody.velocity = copy;
        _animator.SetBool(JumpHash, true);
        yield return new WaitForSeconds(0.3f);
        _jumpTrigger = false;
    }

    private bool _jumpTrigger;

    public void SetIsGround(bool isGround)
    {
        _animator.SetBool(IsGroundHash, isGround);
    }

    public void Landing()
    {
        IsLanding = true;
        
        _stateMachineTrigger
            .OnStateExitAsObservable()
            .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("Landing"))
            .Select(_ => Unit.Default)
            .Merge(Observable.Timer(TimeSpan.FromSeconds(1)))
            .Subscribe(_ => IsLanding = false)
            .AddTo(this);
    }

    
    public void MovePlayer(Vector2 input , bool IsAiming , in Transform _playerCameraTransform)
    {
        var forward = _playerCameraTransform.forward;
        var right = _playerCameraTransform.right;

        if (IsAiming)
        {
            //弓を引いている間は、プレイヤーが動いてなくともカメラ向きを計算する。
            //プレイヤーとカメラのなす角が大きい場合、補完を行う
            var angle = Vector3.Angle(_playerTransform.forward, new Vector3(forward.x, 0f, forward.z));
            if (angle > _rotateThreshold)
            {
                var target = Quaternion.LookRotation(new Vector3(forward.x, 0f, forward.z));
                _playerTransform.rotation = Quaternion.Slerp(_playerTransform.rotation, target, Time.fixedDeltaTime * _rotateLerpRateIsAiming);
            }
            else
            {
                _playerTransform.forward = new Vector3(forward.x, 0f, forward.z);
            }
        }
        
        if (!IsLanding && Mathf.Abs(input.magnitude) > _moveInputThreshold)
        {
            var moveX = new Vector3(forward.x, 0f, forward.z).normalized * input.y;
            var moveZ = new Vector3(right.x, 0f, right.z).normalized * input.x;
            var dir = ((moveX + moveZ).magnitude < 1f ? (moveX + moveZ) : (moveX + moveZ).normalized);
            if (IsAiming)
            {
                //エイム時カメラ基準で移動を行う
                var velocity = dir * (_forwardSpeed * _aimSpeedDecrease);
                _rigidBody.velocity = new Vector3(velocity.x, _rigidBody.velocity.y, velocity.z);
            }
            else
            {
                //通常時はカメラの回転をプレイヤー移動に混ぜる。
                var angle = Vector3.Angle(_playerTransform.forward, dir);
                if (angle > _rotateThreshold)
                {
                    var target = Quaternion.LookRotation(dir);
                    _playerTransform.rotation = Quaternion.Slerp(_playerTransform.rotation, target, Time.fixedDeltaTime * _rotateLerpRateIsNormal);
                }
                else
                {
                    _playerTransform.forward = dir;
                }
                var velocity = _playerTransform.forward * (dir.magnitude * _forwardSpeed);
                _currentVelocity = velocity;
                // var centerPos = _playerCapsule.transform.position + _playerCapsule.center;
                // var radius = _playerCapsule.radius;
                // var halfHeight = _playerCapsule.height / 2.0f;
                // var pos1 = centerPos + _playerCapsule.transform.up * halfHeight;
                // var pos2 = centerPos - _playerCapsule.transform.up * halfHeight;
                // if (!IsGround && Physics.CapsuleCast(pos1, pos2, radius, new Vector3(velocity.x, 0, velocity.z), out RaycastHit hit, _wallCheckRayLength, _wallLayer) 
                //     && Vector3.Angle(hit.normal, Vector3.up) >= _wallAngle)
                // {
                //     _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
                // }
                // else
                // {
                //     if (!_jumpTrigger && CanWalk)
                //     {
                //         _rigidBody.velocity = Vector3.ProjectOnPlane(new Vector3(velocity.x, 0, velocity.z), GroundNormal).normalized * _forwardSpeed;
                //     }
                //     else
                //     {
                //         _rigidBody.velocity = new Vector3(velocity.x, _rigidBody.velocity.y, velocity.z);
                //     }
                // }
                if (!_jumpTrigger && CanWalk)
                {
                    _rigidBody.velocity = Vector3.ProjectOnPlane(new Vector3(velocity.x, 0, velocity.z), GroundNormal).normalized * _forwardSpeed;
                }
                else
                {
                    _rigidBody.velocity = new Vector3(velocity.x, _rigidBody.velocity.y, velocity.z);
                }
            }
        }
        else
        {
            _rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, new Vector3(0f, _rigidBody.velocity.y, 0f), _stoppingRate * Time.fixedDeltaTime);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!IsGround || IsClimbing || IsSwimming)
        {
            _currentFootIKWeight = 0;
            return;
        }

        Vector3 leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        Vector3 rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
        
        leftFoot = GetFootPoint(leftFoot + Vector3.up, leftFoot, 0.8f, false);
        rightFoot = GetFootPoint(rightFoot + Vector3.up, rightFoot, 0.8f, true);
        
        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _currentFootIKWeight);
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _currentFootIKWeight);
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot);
    }

    Vector3 GetFootPoint(Vector3 upperLeg, Vector3 foot, float addDistance, bool isRight)
    {
        var prevFoot = foot;
        var vector = (foot - upperLeg).normalized;
        foot += vector * addDistance;
        if (isRight)
        {
            _rightFootIKLineFrom = upperLeg;
            _rightFootIKLineTo = foot;
        }
        else
        {
            _leftFootIKLineFrom = upperLeg;
            _leftFootIKLineTo = foot;
        }
        if (Physics.Linecast(upperLeg, foot, out var hit, ~_playerLayer))
        {
            return hit.point + _footIKOffset;
        }

        return prevFoot;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Water"))
        {
            IsSwimming = true;
            _forwardSpeed = _normalForwardSpeed * 0.35f;
            _animator.SetBool(SwimHash, true);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        IsSwimming = false;
        _forwardSpeed = _normalForwardSpeed;
        _animator.SetBool(SwimHash, false);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_wallCheckRayPoint.position, _wallCheckRayPoint.position + new Vector3(_currentVelocity.x, 0, _currentVelocity.z) * _wallCheckRayLength);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_leftFootIKLineFrom, _leftFootIKLineTo);
        Gizmos.DrawLine(_rightFootIKLineFrom, _rightFootIKLineTo);
    }
#endif
}
