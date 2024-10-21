using R3;
using R3.Triggers;
using System;
using UnityEngine;
using UnityEngine.Animations;
public class PlayerClimbController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _climbIKWeight;
    private int _baseLayerIndex;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    
    public bool IsLeftAwait; //trueが初期状態
    public bool IsRightAwait;
    public bool IsClimbing;

    private void Start()
    {
        _baseLayerIndex = _animator.GetLayerIndex("BaseLayer");
        _stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_baseLayerIndex];
    }
    
    public void ClimbStart()
    {
        _animator.SetBool("IsClimb", true);
        _rigidBody.useGravity = false;  
        IsClimbing = true;
        IsLeftAwait = true;
        _animator.enabled = false;
    }

    public void ClimbMove(Vector2 input, RaycastHit hitWall)
    {
        _rigidBody.transform.forward = -hitWall.normal;
        
        // 壁に対しての軸を計算
        Vector3 xAxis = Vector3.Cross(Vector3.up, hitWall.normal).normalized;
        Vector3 yAxis = Vector3.Cross(hitWall.normal, xAxis).normalized;

        Vector3 moveDir = -xAxis * input.x + yAxis * input.y;
        moveDir = moveDir.magnitude > 1f ? moveDir.normalized : moveDir;
        // 結果のベクトルを適用（壁に対して並行に移動）
        _rigidBody.velocity = moveDir * _climbSpeed;

        float animateDeltaTime = Time.fixedDeltaTime * moveDir.magnitude;
        if (input.y > 0f)
        {
            _animator.Update(animateDeltaTime);
        }
        else
        {
            _animator.Update(animateDeltaTime);
        }
        
        
    }
    
    public void ClimbEnd()
    {
        _animator.SetBool("IsClimb", false);
        IsClimbing = false;
        _animator.enabled = true;
        _rigidBody.useGravity = true;  
    }
    
    
}