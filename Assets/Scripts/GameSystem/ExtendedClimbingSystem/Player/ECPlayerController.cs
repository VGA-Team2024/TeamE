using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class ECPlayerController : MonoBehaviour
{
    [SerializeField] private float _wallAngle = 55f;
    [SerializeField] private LayerMask _climbLayerMask;
    [SerializeField] private float _playerClimbRayLength = 0.5f;
    [SerializeField] private float _playerClimbThreshold = 0.5f;
    [SerializeField] private Transform _playerClimbRayPoint;
    [SerializeField] private float _ignoreGroundTime = 0.1f;
    [SerializeField] private LayerMask _groundCheckRayCastLayerMask;
    [SerializeField] private float _groundCheckRayCastOffsetY;
    [SerializeField] private float _groundCheckRayCastLength;
    [SerializeField] private GameObject _bowObject;
    [SerializeField] Transform _playerCameraTransform;
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] private ECMoveController _playerMoveController;
    [SerializeField] private PlayerBowController _playerBowController;
    [SerializeField] private PlayerCameraController _playerCameraController;
    [SerializeField] private ECClimbController _playerClimbController;
    [SerializeField] private BoneChecker _boneChecker;

    [SerializeField] private float _overlapSphereOffset = 0.8f;
    [SerializeField] private float _overlapSphereRadius = 0.8f;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    private Vector3 _overlapSphereOrigin;
    
    public bool IsAiming;
    public bool IsArrowReleasing;
    public bool IsArrowCharging;
    public bool IsJumping;
    public bool PreviousIsGround = true;
    public bool IsGround = true;
    public ReactiveProperty<bool> IsClimbable = new();
    public bool IsClimbing;
    public bool IsClimbPullUp;
    public bool IsLanding;
    private RaycastHit _climeTargetHit;
    private Vector2 _currentMoveInput;
    private float _ignoreGroundTimer; //ジャンプ時等に一時的に接地判定を無視するためのタイマー
    //private BoneContainer _boneContainer;
    private Vector3 _currentNormal;
    private Vector3 _currentClosestPoint;

    private void Start()
    {
        PlayerInputProvider.Instance.JumpSubject.Subscribe(_ => Jump()).AddTo(this);
        PlayerInputProvider.Instance.AimSubject.Subscribe(Aim).AddTo(this);
        PlayerInputProvider.Instance.AttackSubject.Subscribe(_ => Attack()).AddTo(this);
    }

    void Jump()
    {
        if (IsClimbing || !IsClimbable.Value && !IsClimbPullUp)
        {
            _playerClimbController.ClimbEnd();
            //climb Cancel
        }
        if (IsGround && !IsLanding && !IsJumping)
        {
            IsJumping = true;
            _ignoreGroundTimer = _ignoreGroundTime;
            _playerMoveController.JumpStart();
        }
    }

    void Aim(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsAiming = true;
            _bowObject.SetActive(true);
            _playerCameraController.ChangeMode(CameraMode.Aim);
        }

        if (context.canceled)
        {
            AimStop();
            if(IsArrowCharging)
            {
                _playerBowController.ArrowRelease(canceled:false);
            } 
        }
    }

    void AimStop()
    {
        IsAiming = false;
        _bowObject.SetActive(false);
        _playerCameraController.ChangeMode(CameraMode.Normal);
    }
    void Attack()
    {
        AimStop();
        if (IsArrowCharging)
        {
            _playerBowController.ArrowRelease(canceled:true);
        }
    }
    private void Update()
    {
        if (IsAiming)
        {
            _playerBowController.ArrowCharge();
        }
        _currentMoveInput = PlayerInputProvider.Instance.MoveValue;

        IsLanding = _playerMoveController.IsLanding;
        IsClimbing = _playerClimbController.IsClimbing;
        IsClimbPullUp = _playerClimbController.IsPullUp;
        IsArrowReleasing = _playerBowController.IsArrowReleasing;
        IsArrowCharging = _playerBowController.IsArrowCharging;

        if (_ignoreGroundTimer < Mathf.Epsilon)
        {
            IsGround = Physics.SphereCast(_rigidBody.position + new Vector3(0f, _groundCheckRayCastOffsetY, 0f),_capsuleCollider.radius , Vector3.down, out var hit, _groundCheckRayCastLength - _capsuleCollider.radius, _groundCheckRayCastLayerMask);
            if (IsGround)
            {
                _playerMoveController.GroundNormal = hit.normal;
                _playerMoveController.CanWalk = Vector3.Angle(hit.normal, Vector3.up) < _wallAngle;
            }
            else
            {
                _playerMoveController.GroundNormal = Vector3.up;
                _playerMoveController.CanWalk = false;
            }
            _playerMoveController.IsGround = IsGround;
            _playerMoveController.SetIsGround(IsGround);
            if (IsGround)
            {
                if (!IsClimbPullUp && !IsLanding && !PreviousIsGround && IsGround)
                {
                    _playerMoveController.Landing();
                }
                if (!IsLanding)
                {
                    if (IsJumping)
                    {
                        IsJumping = false;
                    }
                    // if (IsClimbing && !IsClimbPullUp)
                    // {
                    //     _playerClimbController.ClimbEnd();
                    // }
                }

            }
            PreviousIsGround = IsGround;
        }
        else
        {
            IsGround = false;
            _ignoreGroundTimer -= Time.deltaTime;
        }

        var castOrigin = _playerClimbRayPoint.position + _playerClimbRayPoint.forward * _overlapSphereOffset;
        _overlapSphereOrigin = castOrigin;
        var hitColliders = Physics.OverlapSphere(castOrigin,
            _overlapSphereRadius, _climbLayerMask);

        //bool foundBoneContainer = false;
        foreach (var c in hitColliders)
        {
            if (c is MeshCollider && c.TryGetComponent(out MeshUpdater updater))
            {
                var tris = new int[3];
                _currentClosestPoint = updater.ClosestPoint(castOrigin, out tris);
                var vertices = updater.BakedMesh.vertices;
                var subA = vertices[tris[1]] - vertices[tris[0]];
                var subB = vertices[tris[2]] - vertices[tris[0]];
                _currentNormal = new Vector3(
                    subA.y * subB.z - subA.z * subB.y, 
                    subA.z * subB.x - subA.x * subB.z,
                    subA.x * subB.y - subA.y * subB.x).normalized;
                // if (c.TryGetComponent(out BoneContainer bones))
                // {
                //     _boneContainer = bones;
                //     foundBoneContainer = true;
                // }
            }
            else
            {
                _currentClosestPoint = c.ClosestPoint(castOrigin);
                _currentNormal = (castOrigin - _currentClosestPoint).normalized;
            }
        }

        //if (!foundBoneContainer) _boneContainer = null;
        
        //壁の判定
        IsClimbable.Value = hitColliders.Length > 0;
        
        //IsClimbable = Physics.Raycast(_playerClimbRayPoint.position , _playerClimbRayPoint.forward , out _climeTargetHit , _playerClimbRayLength , _climbLayerMask);
        //IsClimbable = IsClimbable && Vector3.Angle(_climeTargetHit.normal, Vector3.up) >= _wallAngle;
        
        if (_boneChecker.BoneContainer)
        {
            (float dist, Transform trans) minDistance = (float.MaxValue, null);
            foreach (var bone in _boneChecker.BoneContainer.Bones)
            {
                var distance = (bone.position - _playerMoveController.transform.position).sqrMagnitude;
                if (minDistance.dist > distance)
                {
                    minDistance = (distance, bone);
                }
            }
            _playerMoveController.transform.SetParent(minDistance.trans);
        }
        else
        {
            _playerMoveController.transform.SetParent(transform);
        }
        
        //  登っている途中で壁の判定が取れないかつ登りあがる処理が行われていなければければ登るのをやめる
        // if(IsClimbing && !IsClimbable.Value && !IsClimbPullUp)
        // {
        //     _playerClimbController.ClimbEnd();
        // }

        IsClimbable.Where(flag => !flag) // falseになった瞬間のみ通す
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.2f))) // 指定時間のタイマーを開始
            .TakeUntil(IsClimbable.Where(flag => flag)) // フラグがtrueになったらキャンセル
            .Subscribe(_ =>
            {
                if (IsClimbing && !IsClimbPullUp)
                {
                    _playerClimbController.ClimbEnd();
                }
            })
            .AddTo(this);
        if(IsGround && !_playerMoveController.IsLanding && !IsAiming && IsClimbable.Value && Vector3.Dot(-_currentNormal , _playerMoveController.transform.TransformDirection(new Vector3(_currentMoveInput.x , 0f , _currentMoveInput.y))) > _playerClimbThreshold)
        {
            _ignoreGroundTimer = _ignoreGroundTime;
            _playerClimbController.ClimbStart(hitWall: _climeTargetHit, _currentNormal, _currentClosestPoint);
            //ClimbStart
        }
    }

    private void FixedUpdate()
    {
        if(IsClimbable.Value && IsClimbing || IsClimbPullUp)
        {
            _playerClimbController.ClimbMove(_currentMoveInput , _climeTargetHit, _currentNormal, _currentClosestPoint);
        }
        else
        {
            _playerMoveController.MovePlayer(_currentMoveInput , IsAiming , _playerCameraTransform);
        }
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_overlapSphereOrigin, _overlapSphereRadius);
        Gizmos.DrawLine(_playerClimbRayPoint.position, _playerClimbRayPoint.position + _playerClimbRayPoint.forward * _playerClimbRayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_currentClosestPoint, 0.1f);
        Gizmos.DrawLine(_currentClosestPoint, _currentClosestPoint + _currentNormal * 5);
    }
#endif
}