using R3;
using R3.Triggers;
using System;
using UnityEngine;

public class ECMoveController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed;
    [SerializeField , Range(0f , 1f)] private float _moveInputThreshold;
    [SerializeField , Range(0f , 1f)] private float _aimSpeedDecrease;
    [SerializeField , Range(0f , 50f)] private float _stoppingRate;
    
    [SerializeField] private float _rotateLerpRateIsNormal;
    [SerializeField] private float _rotateLerpRateIsAiming;
    [SerializeField] private float _rotateThreshold;
    
    [SerializeField] private float _jumpPower = 3.0f;

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
    
    [HideInInspector] public bool IsLanding;
    [HideInInspector] public bool IsGrounded;
    
    int _baseLayerIndex;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int IsGround = Animator.StringToHash("IsGround");

    private void Start()
    {
        _baseLayerIndex = _animator.GetLayerIndex("BaseLayer");
        _stateMachineTrigger = _stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_baseLayerIndex];
    }
    private void Update()
    {
        var magnitude = new Vector3(_rigidBody.velocity.x , 0f , _rigidBody.velocity.z).magnitude;

        
        _animator.SetFloat(Speed , magnitude / _forwardSpeed);
    }
    
    public void JumpStart()
    {
        _rigidBody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
        _animator.SetBool(Jump, true);
    }

    public void SetIsGround(bool isGround)
    {
        _animator.SetBool(IsGround, isGround);
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
                var centerPos = _playerCapsule.transform.position + _playerCapsule.center;
                var radius = _playerCapsule.radius;
                var halfHeight = _playerCapsule.height / 2.0f;
                var pos1 = centerPos + _playerCapsule.transform.up * halfHeight;
                var pos2 = centerPos - _playerCapsule.transform.up * halfHeight;
                if (!IsGrounded && Physics.CapsuleCast(pos1, pos2, radius, new Vector3(velocity.x, 0, velocity.z), out RaycastHit hit, _wallCheckRayLength, _wallLayer) 
                    && Vector3.Angle(hit.normal, Vector3.up) >= _wallAngle)
                {
                    _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
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
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_wallCheckRayPoint.position, _wallCheckRayPoint.position + new Vector3(_currentVelocity.x, 0, _currentVelocity.z) * _wallCheckRayLength);
    }
#endif
}
