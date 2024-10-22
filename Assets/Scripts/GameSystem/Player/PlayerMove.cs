using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed;
    [SerializeField , Range(0f , 1f)] private float _moveInputThreshold;
    [SerializeField , Range(0f , 1f)] private float _aimSpeedDecrease;
    [SerializeField , Range(0f , 50f)] private float _stoppingRate;
    [SerializeField] private float _jumpPower = 3.0f;

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _playerTransform;

    private static readonly int Speed = Animator.StringToHash("Speed");
    
    public bool PreviousIsGround = true;
    public bool IsLanding;
    
    int _baseLayerIndex;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    
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
        _animator.SetBool("Jump", true);
    }

    public void SetIsGround(bool isGround)
    {
        _animator.SetBool("IsGround", isGround);
    }

    public void Landing()
    {
        IsLanding = true;
        _stateMachineTrigger
            .OnStateExitAsObservable()
            .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("Landing"))
            .Subscribe( _ => IsLanding = false)
            .AddTo(this);
    }

    
    public void MovePlayer(Vector2 input , bool IsAiming , in Transform _playerCameraTransform)
    {
        var forward = _playerCameraTransform.forward;
        var right = _playerCameraTransform.right;

        if (IsAiming)
        {
            //弓を引いている間は、プレイヤーが動いてなくともカメラ向きを計算する。
            _playerTransform.forward = new Vector3(forward.x, 0f , forward.z);
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
                _playerTransform.forward = dir;
                var velocity = _playerTransform.forward * (dir.magnitude * _forwardSpeed);
                _rigidBody.velocity = new Vector3(velocity.x, _rigidBody.velocity.y, velocity.z);
            }
        }
        else
        {
            _rigidBody.velocity = Vector3.Lerp(_rigidBody.velocity, new Vector3(0f, _rigidBody.velocity.y, 0f), _stoppingRate * Time.fixedDeltaTime);
        }


    }
}