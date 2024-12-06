using DG.Tweening;
using R3.Triggers;
using UnityEngine;
public class ECClimbController : MonoBehaviour
{
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private Transform _climbUpRayCastPoint;
    [SerializeField] private float _climbUpRayCastLength = 1.5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _cliffRelativeDistance;
    [SerializeField] private float _lookAtWeight;
    [SerializeField] private float _wallDistance = 0.5f;
    private int _baseLayerIndex;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    [HideInInspector] public bool IsPullUp;
    [HideInInspector] public bool IsClimbing;
    private static readonly int IsClimb = Animator.StringToHash("IsClimb");

    private Vector3 _gizmoFrom, _gizmoTo;
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
    
    public void ClimbStart(RaycastHit hitWall, Vector3 normal, Vector3 point)
    {
        _animator.SetBool(IsClimb, true);
        _rigidBody.useGravity = false;
        _collider.isTrigger = true;
        IsClimbing = true;
        _animator.enabled = false;
        _rigidBody.position = new Vector3(point.x , _rigidBody.position.y , point.z) + normal * _cliffRelativeDistance;
    }
    public void ClimbMove(Vector2 input, RaycastHit hitWall, Vector3 normal, Vector3 closestPoint)
    {
        if (IsPullUp) return;
        _gizmoFrom = _climbUpRayCastPoint.position;
        var dir = Vector3.ProjectOnPlane(_climbUpRayCastPoint.forward, Vector3.up);
        _gizmoTo = _gizmoFrom + dir * _climbUpRayCastLength;
        if (!Physics.Raycast(_gizmoFrom, dir, _climbUpRayCastLength, _wallLayer))
        {
            float offset = 0;
            for (int i = 0; i < 20; i++)
            {
                if (Physics.Raycast(_gizmoFrom + Vector3.up * (_climbUpRayCastLength * 0.99f) + dir * offset, Vector3.down, out RaycastHit hit, _climbUpRayCastLength, _wallLayer))
                {
                    var angle =  Vector3.Angle(hit.normal, Vector3.up);
                    if (angle <= 50)
                    {
                        float pullUpHeight = 0.5f;
                        // if (_collider is CapsuleCollider capsule)
                        // {
                        //     pullUpHeight = capsule.height / 2f;
                        // }
                        PullUp(hit.point + Vector3.up * pullUpHeight);
                        return;
                    }
                }
                offset += 0.1f;
            }
        }
        // if (!Physics.SphereCast(_gizmoFrom, 0.2f, dir, out var hit, _climbUpRayCastLength, _wallLayer))
        // {
        //     PullUp();
        //     return;
        // }

        Plane plane = new Plane(normal, closestPoint);
        _rigidBody.transform.position = Vector3.Lerp(_rigidBody.transform.position, plane.ClosestPointOnPlane(_rigidBody.transform.position) + normal * _wallDistance, 10f * Time.fixedDeltaTime);
            //plane.ClosestPointOnPlane(_rigidBody.transform.position) + normal * _wallDistance;
        _rigidBody.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-normal), 10f * Time.fixedDeltaTime);
        //_rigidBody.transform.forward = -normal;
       
        // 壁に対しての軸を計算
        Vector3 moveDir;
        if (Vector3.up.Equals(normal))
        {
            moveDir = new Vector3(input.x, 0, input.y);
        }
        else
        {
            Vector3 xAxis = Vector3.Cross(Vector3.up, normal).normalized;
            Vector3 yAxis = Vector3.Cross(normal, xAxis).normalized;
            
            moveDir = -xAxis * input.x + yAxis * input.y;
            moveDir = moveDir.magnitude > 1f ? moveDir.normalized : moveDir;
        }

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

    public void PullUp(Vector3 pos)
    {
        _rigidBody.velocity = Vector3.zero;
        IsPullUp = true;
        _collider.isTrigger = true;
        _animator.enabled = true;
        //_animator.applyRootMotion = true;
        _animator.SetBool("IsPullUp", true);
        _animator.SetBool(IsClimb, false);
        transform.DOMove(pos, 1.02f).OnComplete(ClimbEnd).SetLink(gameObject);
        // _stateMachineTrigger
        //     .OnStateExitAsObservable()
        //     .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("PullUp"))
        //     .Subscribe( _ => ClimbEnd())
        //     .AddTo(this);
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
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_gizmoFrom, _gizmoTo);
    }
#endif
}