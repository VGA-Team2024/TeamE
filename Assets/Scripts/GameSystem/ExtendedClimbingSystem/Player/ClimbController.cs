using UnityEngine;
public class ClimbController : MonoBehaviour, IHasPlayerVariable
{
    [SerializeField] private float _climbSideJumpPower = 3f;
    [SerializeField] private float _climbJumpPower = 5f;
    [SerializeField] private float _wallTopExitTime = 3.233f;
    [SerializeField] private Transform _climbUpRayCastPoint;
    [SerializeField] private float _climbUpRayCastLength = 1.5f;
    [SerializeField] private float _climbSpeed;
    [SerializeField] private float _wallDistance = 0.5f;
    [SerializeField] private float _fulcrumRaycastLength = 2f;
    private PlayerVariable _variable;
    
    public void ClimbStart(Vector3 normal, Vector3 point)
    {
        _variable.Animator.SetBool(AnimHashUtil.IsClimb, true);
        _variable.Rigidbody.useGravity = false;
        foreach (var col in _variable.Colliders)
        {
            col.includeLayers = ~_variable.WallLayer;
            col.excludeLayers = _variable.WallLayer;
        }
        _variable.IsClimbing = true;
        _variable.Rigidbody.position = new Vector3(point.x, _variable.Rigidbody.position.y , point.z) + normal * _wallDistance;
    }
    public void ClimbMove(Vector2 input, Vector3 normal, Vector3 closestPoint)
    {
        Plane plane = new Plane(normal, closestPoint);
        // _rigidBody.position = plane.ClosestPointOnPlane(_rigidBody.position) + normal * _wallDistance;
        // _rigidBody.transform.forward = -normal;
        _variable.PlayerRoot.position = Vector3.Lerp(_variable.PlayerRoot.position, plane.ClosestPointOnPlane(_variable.PlayerRoot.position) + normal * _wallDistance, 10f * Time.fixedDeltaTime);
        _variable.PlayerRoot.rotation = Quaternion.Lerp(_variable.PlayerRoot.rotation, Quaternion.LookRotation(-normal), 10f * Time.fixedDeltaTime);

        if (_variable.IsAttack)
        {
            _variable.Rigidbody.velocity = Vector3.zero;
            return;
        }
        // 壁に対しての軸を計算
        Vector3 xAxis = Vector3.Cross(transform.up, normal).normalized;
        Vector3 yAxis = Vector3.Cross(normal, xAxis).normalized;
        
        Vector3 moveDir = -xAxis * input.x + yAxis * input.y;
        moveDir = moveDir.magnitude > 1f ? moveDir.normalized : moveDir;

        // 結果のベクトルを適用（壁に対して並行に移動）
        _variable.Rigidbody.velocity = moveDir * _climbSpeed;

        // float animateDeltaTime = Time.fixedDeltaTime * moveDir.magnitude;
        // if (input.y > 0f)
        // {
        //     _animator.Update(animateDeltaTime);
        // }
        // else
        // {
        //     _animator.Update(animateDeltaTime);
        // }
    }
    
    public void ClimbEnd()
    {
        foreach (var col in _variable.Colliders)
        {
            col.includeLayers = 0;
            col.excludeLayers = 0;
        }
        _variable.Animator.SetBool(AnimHashUtil.IsClimb, false);
        _variable.Animator.SetBool("IsPullUp", false);
        _variable.IsClimbing = false;
        _variable.Animator.applyRootMotion = false;
        //_animator.enabled = true;
        _variable.Rigidbody.useGravity = true;  
    }

    public void ClimbJump()
    {
        var input = PlayerInputProvider.Instance.MoveValue;
        Vector3 normal = -transform.forward;
        Vector3 xAxis = Vector3.Cross(transform.up, normal).normalized;
        Vector3 yAxis = Vector3.Cross(normal, xAxis).normalized;
        Vector3 moveDir = -xAxis * input.x + yAxis * input.y;
        moveDir = moveDir.magnitude > 1f ? moveDir.normalized : moveDir;
        // 結果のベクトルを適用（壁に対して並行に移動）
        _variable.Rigidbody.velocity = moveDir * _climbJumpPower;
    }
    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }
}