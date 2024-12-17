using DG.Tweening;
using R3.Triggers;
using UnityEngine;
public class ECClimbController : MonoBehaviour
{
    [SerializeField] private float _climbSideJumpPower = 3f;
    [SerializeField] private float _climbJumpPower = 5f;
    [SerializeField] private float _wallTopExitTime = 3.233f;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private Transform _climbUpRayCastPoint;
    [SerializeField] private float _climbUpRayCastLength = 1.5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _lookAtWeight;
    [SerializeField] private float _wallDistance = 0.5f;
    [SerializeField] private float _fulcrumRaycastLength = 2f;
    private int _baseLayerIndex;
    private ObservableStateMachineTrigger _stateMachineTrigger;
    [HideInInspector] public bool IsPullUp;
    [HideInInspector] public bool IsClimbing;
    private static readonly int IsClimb = Animator.StringToHash("IsClimb");

    private Vector3 _pullUpRayFrom, _pullUpRayTo;
    private Vector3 _fulcrumRayFrom, _fulcrumRayTo;
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
        //_collider.isTrigger = true;
        _collider.includeLayers = ~_wallLayer;
        _collider.excludeLayers = _wallLayer;
        IsClimbing = true;
        //_animator.enabled = false;
        _rigidBody.position = new Vector3(point.x , _rigidBody.position.y , point.z) + normal * _wallDistance;
    }
    public void ClimbMove(Vector2 input, RaycastHit hitWall, Vector3 normal, Vector3 closestPoint)
    {
        if (IsPullUp) return;
        _pullUpRayFrom = _climbUpRayCastPoint.position;
        var dir = Vector3.ProjectOnPlane(_climbUpRayCastPoint.forward, Vector3.up);
        _pullUpRayTo = _pullUpRayFrom + dir * _climbUpRayCastLength;
        // if (!Physics.Raycast(_pullUpRayFrom, dir, _climbUpRayCastLength, _wallLayer))
        // {
        //     float offset = 0;
        //     for (int i = 0; i < 20; i++)
        //     {
        //         if (Physics.Raycast(_pullUpRayFrom + Vector3.up * (_climbUpRayCastLength * 0.99f) + dir * offset, Vector3.down, out RaycastHit hit, _climbUpRayCastLength, _wallLayer))
        //         {
        //             var angle =  Vector3.Angle(hit.normal, Vector3.up);
        //             if (angle <= 50)
        //             {
        //                 PullUp(hit.point);
        //                 return;
        //             }
        //         }
        //         offset += 0.1f;
        //     }
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
    public void PullUp(Vector3 goalPos)
    {
        transform.forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        _rigidBody.velocity = Vector3.zero;
        IsPullUp = true;
        _collider.includeLayers = ~_wallLayer;
        _collider.excludeLayers = _wallLayer;
        //_animator.enabled = true;
        //_animator.applyRootMotion = true;
        _animator.SetBool("IsPullUp", true);
        _animator.SetBool(IsClimb, false);
        var startPos = transform.position;
        var yVector = new Vector3(startPos.x, goalPos.y, startPos.z) - startPos;
        var fulcrumPos = startPos + yVector;
        var startToGoalLine = goalPos - startPos;
        var startToFulcrumLine = fulcrumPos - startPos;
        float t = Vector3.Dot(startToFulcrumLine, startToGoalLine) / startToGoalLine.sqrMagnitude;
        var projection = startPos + t * startToGoalLine;
        var rayDir = (projection - fulcrumPos).normalized;
        _fulcrumRayFrom = fulcrumPos;
        _fulcrumRayTo = fulcrumPos + rayDir * _fulcrumRaycastLength;
        if (Physics.Raycast(_fulcrumRayFrom, rayDir, out RaycastHit hit, _fulcrumRaycastLength, _wallLayer))
        {
            fulcrumPos = hit.point;
        }
        Vector3[] path = new[]
        {
            startPos,
            fulcrumPos,
            goalPos
        };
        transform.DOPath(path, _wallTopExitTime).OnComplete(ClimbEnd).SetLink(gameObject);
        //transform.DOMove(goalPos, _wallTopExitTime).OnComplete(ClimbEnd).SetLink(gameObject);
                    
        // _stateMachineTrigger
        //     .OnStateExitAsObservable()
        //     .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("PullUp"))
        //     .Subscribe( _ => ClimbEnd())
        //     .AddTo(this);
    }
    
    public void ClimbEnd()
    {
        _collider.includeLayers = 0;
        _collider.excludeLayers = 0;
        _animator.SetBool(IsClimb, false);
        _animator.SetBool("IsPullUp", false);
        IsClimbing = false;
        IsPullUp = false;
        _animator.applyRootMotion = false;
        //_animator.enabled = true;
        _rigidBody.useGravity = true;  
    }

    public void ClimbJump()
    {
        //ClimbEnd();
        var moveValue = PlayerInputProvider.Instance.MoveValue;
        Vector3 moveDir;
        if (Vector3.up.Equals(-transform.forward))
        {
            moveDir = new Vector3(moveValue.x, 0, moveValue.y);
        }
        else
        {
            Vector3 xAxis = Vector3.Cross(Vector3.up, -transform.forward).normalized;
            Vector3 yAxis = Vector3.Cross(-transform.forward, xAxis).normalized;
            
            moveDir = -xAxis * moveValue.x + yAxis * moveValue.y;
            moveDir = moveDir.magnitude > 1f ? moveDir.normalized : moveDir;
        }

        // 結果のベクトルを適用（壁に対して並行に移動）
        _rigidBody.velocity = moveDir * _climbJumpPower;
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_pullUpRayFrom, _pullUpRayTo);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(_fulcrumRayFrom, _fulcrumRayTo);
    }
#endif
}