using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerVariable _variable;
    [SerializeField] private float _climbHoppingLimit = 0.2f;
    [SerializeField] private Material _vineMat;
    [SerializeField] private RASCALSkinnedMeshCollider _rascalSkinnedMesh;
    [SerializeField] private float _playerClimbRayLength = 0.5f;
    [SerializeField] private float _playerClimbThreshold = 0.5f;
    [SerializeField] private Transform _playerClimbRayPoint;
    [SerializeField] private float _ignoreGroundTime = 0.1f;
    [SerializeField] private float _groundCheckRayCastOffsetY;
    [SerializeField] private float _groundCheckRayCastLength;
    [SerializeField] private GameObject _bowObject;
    [SerializeField] private float _overlapSphereOffset = 0.8f;
    [SerializeField] private float _overlapSphereRadius = 0.8f;
    private bool _isAiming;
    private bool _isJumping;
    private bool _previousIsGround = true;
    private Vector3 _currentClosestPoint;
    private Vector2 _currentMoveInput;
    private Vector3 _currentNormal;
    private float _ignoreGroundTimer; //ジャンプ時等に一時的に接地判定を無視するためのタイマー
    private float _highestPoint;
    private bool _isClimbHopping;
    private Vector3 _overlapSphereOrigin;
    private Vector3 _prevPos;
    private readonly ReactiveProperty<bool> _isGround = new(true);
    private readonly ReactiveProperty<bool> _hasParent = new();
    private readonly ReactiveProperty<bool> _isClimbable = new();
    private CRIAudioManager.SoundPlayer _bowPullSoundPlayer;
    public PlayerVariable Variable => _variable;

    private void Awake()
    {
        _variable.InjectSelf();
    }

    private void Start()
    {
        PlayerInputProvider.Instance.JumpSubject.Subscribe(_ => Jump()).AddTo(this);
        PlayerInputProvider.Instance.AimSubject.Subscribe(Aim).AddTo(this);
        PlayerInputProvider.Instance.AttackSubject.Subscribe(_ => Attack()).AddTo(this);

        _isClimbable.Where(flag => !flag) // falseになった瞬間のみ通す
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.2f)).TakeUntil(_isClimbable.Where(flag => flag)))
            .Subscribe(_ => _variable.ClimbController.ClimbEnd())
            .AddTo(this);
        _hasParent.Where(flag => !flag)
            .SelectMany(_ => Observable.Timer(TimeSpan.FromSeconds(0.2f)).TakeUntil(_hasParent.Where(flag => flag)))
            .Subscribe(_ =>
            {
                _variable.PlayerRoot.SetParent(null);
                _variable.PlayerRoot.localScale = Vector3.one;
            }) //  元の親に戻す
            .AddTo(this);
        _isGround.Where(flag => !flag).Subscribe(_ => _highestPoint = _variable.PlayerRoot.position.y).AddTo(this);
    }

    private void UpdateHighestPoint()
    {
        var currentHeight = _variable.PlayerRoot.position.y;
        if (currentHeight > _highestPoint)
        {
            _highestPoint = currentHeight;
        }
    }
    private void Update()
    {
        if (_isAiming) _variable.ArrowChargeRate = _variable.BowController.ArrowCharge();
        _currentMoveInput = PlayerInputProvider.Instance.MoveValue;
        _variable.IsClimbing = _variable.IsClimbing;

        if (_ignoreGroundTimer < Mathf.Epsilon)
        {
            _isGround.Value = Physics.SphereCast(_variable.Rigidbody.position + new Vector3(0f, _groundCheckRayCastOffsetY, 0f),
                _variable.CapsuleCollider.radius, Vector3.down, out var hitGround,
                _groundCheckRayCastLength - _variable.CapsuleCollider.radius, _variable.GroundLayer);
            if (_isGround.Value)
            {
                _variable.GroundNormal = hitGround.normal;
                _variable.CanWalk = Vector3.Angle(hitGround.normal, Vector3.up) < _variable.WallAngle;
                if(!_variable.IsKnockBack) _variable.PlayerRoot.SetParent(hitGround.collider.transform);
                //SetLossyScale(_variable.PlayerRoot);
                _hasParent.Value = true;
            }
            else
            {
                _variable.GroundNormal = Vector3.up;
                _variable.CanWalk = false;
                _hasParent.Value = false;
                UpdateHighestPoint();
            }

            _variable.MoveController.SetIsGround(_isGround.Value);
            if (_isGround.Value)
            {
                if (!_variable.IsLanding && !_previousIsGround && _isGround.Value && !_variable.IsClimbing)
                {
                    _variable.MoveController.Landing();
                    _variable.PlayerEventReceiver.ReceivedFallDamage =
                        _variable.HealthCalculator.FallDamage(_highestPoint,
                            _variable.MoveController.transform.position.y);
                    _variable.Animator.SetFloat(AnimHashUtil.FallDistance, _highestPoint - _variable.MoveController.transform.position.y);
                }

                if (!_variable.IsLanding)
                    if (_isJumping)
                        _isJumping = false;
                // if (IsClimbing && !IsClimbPullUp)
                // {
                //     _playerClimbController.ClimbEnd();
                // }
            }

            _previousIsGround = _isGround.Value;
        }
        else
        {
            _isGround.Value = false;
            _ignoreGroundTimer -= Time.deltaTime;
        }

        var castOrigin = _playerClimbRayPoint.position;
        _overlapSphereOrigin = castOrigin;
        _isClimbable.Value = Physics.SphereCast(castOrigin, _overlapSphereRadius,
            _variable.PlayerRoot.forward,
            out var hitWall, _overlapSphereRadius,
            _variable.WallLayer) && (PlayerInputProvider.Instance.Grab || _variable.IsAttack) && !_isClimbHopping;
        if (_isClimbable.Value)
        {
            if (hitWall.transform.CompareTag("ClimbableWall"))
            {
                _isClimbable.Value = true;
            }
            else if (hitWall.collider is MeshCollider)
            {
                var mat = GetMaterial(hitWall.collider);
                if (mat != null)
                {
                    _isClimbable.Value = mat == _vineMat;
                    if (!_isClimbable.Value) _variable.PlayerRoot.position = _prevPos;
                }
                else
                {
                    _isClimbable.Value = false;
                }
            }
            else
            {
                _isClimbable.Value = false;
            }
        }

        if (_isClimbable.Value)
        {
            _currentNormal = hitWall.normal;
            _currentClosestPoint = hitWall.point;
            if(!_variable.IsKnockBack) _variable.PlayerRoot.SetParent(hitWall.collider.transform);
            //SetLossyScale(_variable.PlayerRoot);
            _hasParent.Value = true;
        }
        else
        {
            _hasParent.Value = false;
        }

        if (!_variable.IsLanding && !_isAiming && _isClimbable.Value && !_variable.IsClimbing)
        {
            _ignoreGroundTimer = _ignoreGroundTime;
            _variable.ClimbController.ClimbStart(_currentNormal, _currentClosestPoint);
            //ClimbStart
        }
    }

    private void FixedUpdate()
    {
        if (_isClimbable.Value && _variable.IsClimbing)
        {
            _variable.ClimbController.ClimbMove(_currentMoveInput, _currentNormal, _currentClosestPoint);
        }
        else if (!_isClimbHopping)
        {
            _variable.MoveController.MovePlayer(_currentMoveInput, _isAiming);
        }

        _prevPos = _variable.PlayerRoot.transform.position;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_variable?.PlayerRoot == null || _playerClimbRayPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_overlapSphereOrigin + _variable.PlayerRoot.forward * _overlapSphereRadius,
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
        if (_variable.IsAttack || _variable.IsKnockBack) return;
        if (_variable.IsClimbing)
        {
            _variable.Animator.SetTrigger(AnimHashUtil.ClimbJump);
            Observable.Timer(TimeSpan.FromSeconds(0.23f)).Subscribe(_ =>
            {
                _isClimbHopping = true;
                _variable.ClimbController.ClimbJump();
            }).AddTo(this);
            Observable.Timer(TimeSpan.FromSeconds(0.53f)).Subscribe(_ =>
            {
                _isClimbHopping = false;
                _variable.ClimbController.ClimbEnd();
            }).AddTo(this);
        }

        if (_isGround.Value && !_variable.IsLanding && !_isJumping)
        {
            _isJumping = true;
            _ignoreGroundTimer = _ignoreGroundTime;
            _variable.MoveController.JumpStart();
        }
    }

    private void Aim(InputAction.CallbackContext context)
    {
        if (context.started && !_variable.IsAttack && !_variable.IsKnockBack)
        {
            _isAiming = true;
            _bowObject.SetActive(true);
            _variable.CameraController.ChangeMode(CameraMode.Aim);
            _bowPullSoundPlayer = CRIAudioManager.SE.Play("CueSheet_SE", "SE_player_bow_pull");
            _bowPullSoundPlayer.SetVolume(0.1f);
        }

        if (context.canceled)
        {
            _bowPullSoundPlayer?.Stop();
            _bowPullSoundPlayer?.SetVolume(1f);
            AimStop();
            if (_variable.IsArrowCharging)
            {
                _variable.BowController.ArrowRelease(false);
            }
        }
    }

    private void AimStop()
    {
        _isAiming = false;
        _bowObject.SetActive(false);
        _variable.CameraController.ChangeMode(CameraMode.Normal);
    }

    private void Attack()
    {
        AimStop();
        if (_variable.IsArrowCharging) _variable.BowController.ArrowRelease(true);
        if (_variable.IsKnockBack) return;
        _variable.AttackController.Attack(_variable.IsClimbing, _variable.Animator);
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