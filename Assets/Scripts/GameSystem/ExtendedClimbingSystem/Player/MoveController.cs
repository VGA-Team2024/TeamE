using R3;
using R3.Triggers;
using System;
using System.Collections;
using UnityEngine;
public class MoveController : MonoBehaviour, IHasPlayerVariable
{
    [SerializeField, InspectorVariantName("移動速度")] private float _moveSpeed;
    [SerializeField, Range(0f , 1f)] private float _moveInputThreshold;
    [SerializeField, Range(0f , 1f)] private float _aimSpeedDecrease;
    [SerializeField, Range(0f , 50f)] private float _stoppingRate;
    [SerializeField] private float _rotateLerpRateIsNormal;
    [SerializeField] private float _rotateLerpRateIsAiming;
    [SerializeField] private float _rotateThreshold;
    [SerializeField] private float _jumpPower = 3.0f;
    [SerializeField] private Transform _wallCheckRayPoint;
    [SerializeField] private float _wallCheckRayLength = 0.5f;
    [SerializeField] private float _wallCheckRayRadius = 0.3f;
    private Vector3 _currentVelocity;
    private int _baseLayerIndex;
    private PlayerVariable _variable;
    private ObservableStateMachineTrigger _stateMachineTrigger;

    private void Start()
    {
        _baseLayerIndex = _variable.Animator.GetLayerIndex("BaseLayer");
        _stateMachineTrigger = _stateMachineTrigger =  _variable.Animator.GetBehaviours<ObservableStateMachineTrigger>()[_baseLayerIndex];
    }
    private void Update()
    {
        var magnitude = new Vector3(_variable.Rigidbody.velocity.x, 0f, _variable.Rigidbody.velocity.z).magnitude;
        _variable.Animator.SetFloat(AnimHashUtil.Speed , magnitude);
        var prevX =  _variable.Animator.GetFloat(AnimHashUtil.InputX);
        var prevY =  _variable.Animator.GetFloat(AnimHashUtil.InputY);
        _variable.Animator.SetFloat(AnimHashUtil.InputX, Mathf.SmoothStep(prevX, PlayerInputProvider.Instance.MoveValue.x, Time.deltaTime * 15f));
        _variable.Animator.SetFloat(AnimHashUtil.InputY, Mathf.SmoothStep(prevY, PlayerInputProvider.Instance.MoveValue.y, Time.deltaTime * 15f));
    }
    
    public void JumpStart()
    {
        StartCoroutine(JumpCoroutine());
        CRIAudioManager.SE.Play("CueSheet_SE", "SE_player_jump");
    }

    IEnumerator JumpCoroutine()
    {
        _jumpTrigger = true;
        yield return new WaitForFixedUpdate();
        var copy = _variable.Rigidbody.velocity;
        copy.y = _jumpPower;
        _variable.Rigidbody.velocity = copy;
        _variable.Animator.SetBool(AnimHashUtil.Jump, true);
        yield return new WaitForSeconds(0.3f);
        _jumpTrigger = false;
    }

    private bool _jumpTrigger;

    public void SetIsGround(bool isGround)
    {
        _variable.Animator.SetBool(AnimHashUtil.IsGround, isGround);
    }

    public void Landing()
    {
        _variable.IsLanding = true;
        
        _stateMachineTrigger
            .OnStateExitAsObservable()
            .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("Landing"))
            .Select(_ => Unit.Default)
            .Merge(Observable.Timer(TimeSpan.FromSeconds(0.3f)))
            .Subscribe(_ => _variable.IsLanding = false)
            .AddTo(this);
    }

    
    public void MovePlayer(Vector2 input, bool IsAiming)
    {
        if (_variable.Rigidbody.isKinematic) return;
        var forward = _variable.CameraTransform.forward;
        var right = _variable.CameraTransform.right;

        if (IsAiming)
        {
            //弓を引いている間は、プレイヤーが動いてなくともカメラ向きを計算する。
            //プレイヤーとカメラのなす角が大きい場合、補完を行う
            var angle = Vector3.Angle(_variable.PlayerRoot.forward, new Vector3(forward.x, 0f, forward.z));
            if (angle > _rotateThreshold)
            {
                var target = Quaternion.LookRotation(new Vector3(forward.x, 0f, forward.z));
                _variable.PlayerRoot.rotation = Quaternion.Slerp(_variable.PlayerRoot.rotation, target, Time.fixedDeltaTime * _rotateLerpRateIsAiming);
            }
            else
            {
                _variable.PlayerRoot.forward = new Vector3(forward.x, 0f, forward.z);
            }
        }
        
        if (!_variable.IsLanding && input != Vector2.zero && !_variable.IsAttack && !_variable.IsKnockBack)
        {
            _variable.Animator.SetFloat(AnimHashUtil.Direction, input.x);
            var inputPower = input.magnitude;
            var moveX = new Vector3(forward.x, 0f, forward.z).normalized * input.y;
            var moveZ = new Vector3(right.x, 0f, right.z).normalized * input.x;
            var dir = (moveX + moveZ).normalized;
            if (IsAiming)
            {
                //エイム時カメラ基準で移動を行う
                var velocity = dir * (inputPower * _moveSpeed * _aimSpeedDecrease);
                _variable.Rigidbody.velocity = new Vector3(velocity.x, _variable.Rigidbody.velocity.y, velocity.z);
            }
            else
            {
                //通常時はカメラの回転をプレイヤー移動に混ぜる。
                var angle = Vector3.Angle(_variable.PlayerRoot.forward, dir);
                if (angle > _rotateThreshold)
                {
                    var target = Quaternion.LookRotation(dir);
                    _variable.PlayerRoot.rotation = Quaternion.Slerp(_variable.PlayerRoot.rotation, target, Time.fixedDeltaTime * _rotateLerpRateIsNormal);
                }
                else
                {
                    _variable.PlayerRoot.forward = dir;
                }

                var velocity = dir * (inputPower * _moveSpeed);
                _currentVelocity = velocity;
                if (!_jumpTrigger && _variable.CanWalk)
                {
                    _variable.Rigidbody.velocity = Vector3.ProjectOnPlane(new Vector3(velocity.x, 0, velocity.z), _variable.GroundNormal).normalized * (inputPower *  _moveSpeed);
                }
                else
                {
                    _variable.Rigidbody.velocity = new Vector3(velocity.x, _variable.Rigidbody.velocity.y, velocity.z);
                }
            }
        }
        else
        {
            _variable.Rigidbody.velocity = Vector3.Lerp(_variable.Rigidbody.velocity, new Vector3(0f, _variable.Rigidbody.velocity.y, 0f), _stoppingRate * Time.fixedDeltaTime);
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_wallCheckRayPoint.position, _wallCheckRayPoint.position + new Vector3(_currentVelocity.x, 0, _currentVelocity.z) * _wallCheckRayLength);
    }
#endif
    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }
}
