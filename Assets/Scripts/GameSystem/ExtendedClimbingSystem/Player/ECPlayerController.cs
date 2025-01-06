using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class ECPlayerController : MonoBehaviour
{
    private static readonly int ClimbJumpHash = Animator.StringToHash("ClimbJump");
    private static readonly int FallDistanceHash = Animator.StringToHash("FallDistance");
    [SerializeField] private float _climbHoppingLimit = 0.2f;
    [SerializeField] private Material _vineMat;
    [SerializeField] private RASCALSkinnedMeshCollider _rascalSkinnedMesh;
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
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private ECMoveController _playerMoveController;
    [SerializeField] private PlayerBowController _playerBowController;
    [SerializeField] private PlayerCameraController _playerCameraController;
    [SerializeField] private ECClimbController _playerClimbController;
    [SerializeField] private PlayerHealthCalculator _playerHealthCalculator;
    [SerializeField] private PlayerAttackController _playerAttackController;
    [SerializeField] private float _overlapSphereOffset = 0.8f;
    [SerializeField] private float _overlapSphereRadius = 0.8f;
    [SerializeField] private CapsuleCollider _capsuleCollider;
    public bool IsAiming;
    public bool IsArrowReleasing;
    public bool IsArrowCharging;
    public bool IsJumping;
    public bool PreviousIsGround = true;
    public ReactiveProperty<bool> IsGround = new(true);
    public bool IsClimbing;
    public bool IsClimbPullUp;
    public bool IsLanding;
    private RaycastHit _climeTargetHit;
    private Vector3 _currentClosestPoint;
    private Vector2 _currentMoveInput;
    private Vector3 _currentNormal;
    private readonly ReactiveProperty<bool> _hasParent = new();
    private float _ignoreGroundTimer; //ジャンプ時等に一時的に接地判定を無視するためのタイマー
    private float _highestPoint;
    private bool _isClimbHopping;
    private Vector3 _overlapSphereOrigin;
    private Vector3 _prevPos;
    public ReactiveProperty<bool> IsClimbable = new();

    private void Start()
    {
        PlayerInputProvider.Instance.JumpSubject.Subscribe(_ => Jump()).AddTo(this);
        PlayerInputProvider.Instance.AimSubject.Subscribe(Aim).AddTo(this);
        PlayerInputProvider.Instance.AttackSubject.Subscribe(_ => Attack()).AddTo(this);

        IsClimbable.Where(flag => !flag) // falseになった瞬間のみ通す
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.2f)).TakeUntil(IsClimbable.Where(flag => flag)))
            .Subscribe(_ => _playerClimbController.ClimbEnd())
            .AddTo(this);
        _hasParent.Where(flag => !flag)
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.2f)).TakeUntil(_hasParent.Where(flag => flag)))
            .Subscribe(_ =>
            {
                _playerMoveController.transform.parent = null;
                SetLossyScale(_playerMoveController.transform);
            }) //  元の親に戻す
            .AddTo(this);
        IsGround.Where(flag => !flag).Subscribe(_ => _highestPoint = _playerMoveController.transform.position.y).AddTo(this);
    }

    private void UpdateHighestPoint()
    {
        var currentHeight = _playerMoveController.transform.position.y;
        if (currentHeight > _highestPoint)
        {
            _highestPoint = currentHeight;
        }
    }
    private void Update()
    {
        if (IsAiming) _playerCameraController.ArrowChargeRate = _playerBowController.ArrowCharge();
        _currentMoveInput = PlayerInputProvider.Instance.MoveValue;

        IsLanding = _playerMoveController.IsLanding;
        IsClimbing = _playerClimbController.IsClimbing;
        IsClimbPullUp = _playerClimbController.IsPullUp;
        IsArrowReleasing = _playerBowController.IsArrowReleasing;
        IsArrowCharging = _playerBowController.IsArrowCharging;
        _playerMoveController.IsClimbing = _playerClimbController.IsClimbing;

        if (_ignoreGroundTimer < Mathf.Epsilon)
        {
            IsGround.Value = Physics.SphereCast(_rigidBody.position + new Vector3(0f, _groundCheckRayCastOffsetY, 0f),
                _capsuleCollider.radius, Vector3.down, out var hitGround,
                _groundCheckRayCastLength - _capsuleCollider.radius, _groundCheckRayCastLayerMask);
            if (IsGround.Value)
            {
                _playerMoveController.GroundNormal = hitGround.normal;
                _playerMoveController.CanWalk = Vector3.Angle(hitGround.normal, Vector3.up) < _wallAngle;
                //_playerMoveController.transform.parent = hitGround.collider.transform;
                //SetLossyScale(_playerMoveController.transform);
                _hasParent.Value = true;
            }
            else
            {
                _playerMoveController.GroundNormal = Vector3.up;
                _playerMoveController.CanWalk = false;
                _hasParent.Value = false;
                UpdateHighestPoint();
            }

            _playerMoveController.IsGround = IsGround.Value;
            _playerMoveController.SetIsGround(IsGround.Value);
            if (IsGround.Value)
            {
                if (!IsClimbPullUp && !IsLanding && !PreviousIsGround && IsGround.Value)
                {
                    _playerMoveController.Landing();
                    _playerHealthCalculator.FallDamage(_highestPoint, _playerMoveController.transform.position.y);
                    _playerMoveController.Animator.SetFloat(FallDistanceHash, _highestPoint - _playerMoveController.transform.position.y);
                }
                if (!IsLanding)
                    if (IsJumping)
                        IsJumping = false;
                // if (IsClimbing && !IsClimbPullUp)
                // {
                //     _playerClimbController.ClimbEnd();
                // }
            }

            PreviousIsGround = IsGround.Value;
        }
        else
        {
            IsGround.Value = false;
            _ignoreGroundTimer -= Time.deltaTime;
        }

        var castOrigin = _playerClimbRayPoint.position;
        _overlapSphereOrigin = castOrigin;
        IsClimbable.Value = Physics.SphereCast(castOrigin, _overlapSphereRadius,
            _playerMoveController.transform.forward,
            out var hitWall, _overlapSphereRadius,
            _climbLayerMask) && (PlayerInputProvider.Instance.Grab || _playerAttackController.IsAttack) && !_isClimbHopping;
        if (IsClimbable.Value)
        {
            if (hitWall.collider is MeshCollider)
            {
                var mat = GetMaterial(hitWall.collider);
                if (mat != null)
                {
                    IsClimbable.Value = mat == _vineMat;
                    if (!IsClimbable.Value) _playerMoveController.transform.position = _prevPos;
                }
                else
                {
                    IsClimbable.Value = false;
                }
            }

            IsClimbable.Value = hitWall.transform.CompareTag("ClimbableWall");
        }

        if (IsClimbable.Value)
        {
            _currentNormal = hitWall.normal;
            _currentClosestPoint = hitWall.point;
            //_playerMoveController.transform.parent = hitWall.collider.transform;
            //SetLossyScale(_playerMoveController.transform);
            _hasParent.Value = true;
        }
        else
        {
            _hasParent.Value = false;
        }

        if (!_playerMoveController.IsLanding && !IsAiming && IsClimbable.Value && !IsClimbing)
        {
            _ignoreGroundTimer = _ignoreGroundTime;
            _playerClimbController.ClimbStart(_climeTargetHit, _currentNormal, _currentClosestPoint);
            //ClimbStart
        }
    }

    private void FixedUpdate()
    {
        if ((IsClimbable.Value && IsClimbing) || IsClimbPullUp)
        {
            _playerClimbController.ClimbMove(_currentMoveInput, _climeTargetHit, _currentNormal, _currentClosestPoint, _playerAttackController.IsAttack);
        }
        else if (!_isClimbHopping)
        {
            _playerMoveController.MovePlayer(_currentMoveInput, IsAiming, _playerCameraTransform, _playerAttackController.IsAttack);
        }

        //_prevPos = _playerMoveController.transform.position;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_overlapSphereOrigin + _playerMoveController.transform.forward * _overlapSphereRadius,
            _overlapSphereRadius);
        Gizmos.DrawLine(_playerClimbRayPoint.position,
            _playerClimbRayPoint.position + _playerClimbRayPoint.forward * _playerClimbRayLength);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_currentClosestPoint, 0.1f);
        Gizmos.DrawLine(_currentClosestPoint, _currentClosestPoint + _currentNormal * 5);
    }
#endif

    private void Jump()
    {
        if (_playerAttackController.IsAttack) return;
        if (IsClimbing)
        {
            _playerMoveController.Animator.SetTrigger(ClimbJumpHash);
            Observable.Timer(TimeSpan.FromSeconds(0.23f)).Subscribe(_ =>
            {
                _isClimbHopping = true;
                _playerClimbController.ClimbJump();
            }).AddTo(this);
            Observable.Timer(TimeSpan.FromSeconds(0.53f)).Subscribe(_ =>
            {
                _isClimbHopping = false;
                _playerClimbController.ClimbEnd();
            }).AddTo(this);
        }

        if (IsGround.Value && !IsLanding && !IsJumping)
        {
            IsJumping = true;
            _ignoreGroundTimer = _ignoreGroundTime;
            _playerMoveController.JumpStart();
        }
    }

    private void Aim(InputAction.CallbackContext context)
    {
        if (context.started && !_playerAttackController.IsAttack)
        {
            IsAiming = true;
            _bowObject.SetActive(true);
            _playerCameraController.ChangeMode(CameraMode.Aim);
        }

        if (context.canceled)
        {
            AimStop();
            if (IsArrowCharging) _playerBowController.ArrowRelease(false);
        }
    }

    private void AimStop()
    {
        IsAiming = false;
        _bowObject.SetActive(false);
        _playerCameraController.ChangeMode(CameraMode.Normal);
    }

    private void Attack()
    {
        AimStop();
        if (IsArrowCharging) _playerBowController.ArrowRelease(true);
        _playerAttackController.Attack(IsClimbing, _playerMoveController.Animator);
    }

    private Material GetMaterial(Collider collider)
    {
        if (!_rascalSkinnedMesh) return null;
        foreach (var skinfo in _rascalSkinnedMesh.skinfos)
        foreach (var bone in skinfo.bones)
        foreach (var boneMesh in bone.boneMeshes)
            if (boneMesh.meshCol == collider)
                return skinfo.skinnedMesh.sharedMaterials[boneMesh.skinnedMeshMaterialIndex];
        return null;
    }

    private void SetLossyScale(Transform target)
    {
        if (target.parent)
        {
            var worldDirectionLocalScale = target.parent.TransformDirection(Vector3.one);
            var scaleUnaffectedByParents = target.parent.InverseTransformVector(worldDirectionLocalScale);
            target.localScale = scaleUnaffectedByParents;
        }
        else
        {
            target.localScale = Vector3.one;
        }
    }
}