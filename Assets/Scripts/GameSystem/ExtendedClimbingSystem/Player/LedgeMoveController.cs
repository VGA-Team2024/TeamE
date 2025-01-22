using UnityEngine;

public class LedgeMoveController : MonoBehaviour
{
    //private Vector3 _pullUpRayFrom, _pullUpRayTo;
    //private Vector3 _fulcrumRayFrom, _fulcrumRayTo;
    public void ClimbMove(Vector2 input, Vector3 normal, Vector3 closestPoint, bool isAttack)
    {
        //_pullUpRayFrom = _climbUpRayCastPoint.position;
        //_pullUpRayTo = _pullUpRayFrom + dir * _climbUpRayCastLength;
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
    }
    // public void PullUp(Vector3 goalPos)
    // {
    //     transform.forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
    //     _rigidBody.velocity = Vector3.zero;
    //     IsPullUp = true;
    //     _collider.includeLayers = ~_wallLayer;
    //     _collider.excludeLayers = _wallLayer;
    //     //_animator.enabled = true;
    //     //_animator.applyRootMotion = true;
    //     _animator.SetBool("IsPullUp", true);
    //     _animator.SetBool(IsClimb, false);
    //     var startPos = transform.position;
    //     var yVector = new Vector3(startPos.x, goalPos.y, startPos.z) - startPos;
    //     var fulcrumPos = startPos + yVector;
    //     var startToGoalLine = goalPos - startPos;
    //     var startToFulcrumLine = fulcrumPos - startPos;
    //     float t = Vector3.Dot(startToFulcrumLine, startToGoalLine) / startToGoalLine.sqrMagnitude;
    //     var projection = startPos + t * startToGoalLine;
    //     var rayDir = (projection - fulcrumPos).normalized;
    //     _fulcrumRayFrom = fulcrumPos;
    //     _fulcrumRayTo = fulcrumPos + rayDir * _fulcrumRaycastLength;
    //     if (Physics.Raycast(_fulcrumRayFrom, rayDir, out RaycastHit hit, _fulcrumRaycastLength, _wallLayer))
    //     {
    //         fulcrumPos = hit.point;
    //     }
    //     Vector3[] path = new[]
    //     {
    //         startPos,
    //         fulcrumPos,
    //         goalPos
    //     };
    //     transform.DOPath(path, _wallTopExitTime).OnComplete(ClimbEnd).SetLink(gameObject);
    //     //transform.DOMove(goalPos, _wallTopExitTime).OnComplete(ClimbEnd).SetLink(gameObject);
    //                 
    //     // _stateMachineTrigger
    //     //     .OnStateExitAsObservable()
    //     //     .Where(x => x.LayerIndex == _baseLayerIndex && x.StateInfo.IsName("PullUp"))
    //     //     .Subscribe( _ => ClimbEnd())
    //     //     .AddTo(this);
    // }
    
    // #if UNITY_EDITOR
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(_pullUpRayFrom, _pullUpRayTo);
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawLine(_fulcrumRayFrom, _fulcrumRayTo);
    // }
    // #endif
}