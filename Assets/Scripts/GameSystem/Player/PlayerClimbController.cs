using R3;
using R3.Triggers;
using System;
using UnityEngine;
public class PlayerClimbController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _cliffRelativeDistance;
    [SerializeField] private float _lookAtWeight;
    private int _baseLayerIndex;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    public bool IsPullUp;
    public bool IsClimbing;
    private static readonly int IsClimb = Animator.StringToHash("IsClimb");

    private void OnAnimatorIK(int layerIndex)
    {
        if ( IsClimbing && layerIndex == _baseLayerIndex)
        {
            _animator.SetLookAtWeight(_lookAtWeight);
            _animator.SetLookAtPosition(-_rigidBody.transform.up); 
        }
    }
    private void Start()
    {
        _baseLayerIndex = _animator.GetLayerIndex("BaseLayer");
        _stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_baseLayerIndex];
    }
    
    public void ClimbStart(RaycastHit hitWall)
    {
        _animator.SetBool(IsClimb, true);
        _rigidBody.useGravity = false;  
        _collider.isTrigger = true;
        IsClimbing = true;
        _animator.enabled = false;
        _rigidBody.position = new Vector3(hitWall.point.x , _rigidBody.position.y , hitWall.point.z) + hitWall.normal * _cliffRelativeDistance;
    }

    public void ClimbMove(Vector2 input, RaycastHit hitWall)
    {
        if (IsPullUp || !hitWall.collider)
        {
            return;
        }
        
        if(!IsPullUp)
        {
            if (hitWall.collider.CompareTag("ClimbableWallTop"))
            {
                PullUp();
                return;
            }
        }

        
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

    public void PullUp()
    {
        _rigidBody.velocity = Vector3.zero;
        IsPullUp = true;
        _collider.isTrigger = true;
        _animator.enabled = true;
        _animator.applyRootMotion = true;
        _animator.SetBool("IsPullUp", true);
        _animator.SetBool(IsClimb, false);
        _stateMachineTrigger
            .OnStateExitAsObservable()
            .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("PullUp"))
            .Subscribe( _ => ClimbEnd())
            .AddTo(this);
    }
    
    public void ClimbEnd()
    {
        _collider.isTrigger = false;
        _animator.SetBool(IsClimb, false);
        _animator.SetBool("IsPullUp", false);
        IsClimbing = false;
        IsPullUp = false;
        _animator.applyRootMotion = false;
        _animator.enabled = true;
        _rigidBody.useGravity = true;  
    }
    
    
}