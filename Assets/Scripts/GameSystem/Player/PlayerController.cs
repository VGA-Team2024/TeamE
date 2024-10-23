using UnityEngine;

public enum CameraMode
{
    Normal ,
    Aim ,
}

public class PlayerController : MonoBehaviour
{
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
    [SerializeField] private PlayerMoveController _playerMoveController;
    [SerializeField] private PlayerBowController _playerBowController;
    [SerializeField] private PlayerCameraController _playerCameraController;
    [SerializeField] private PlayerClimbController _playerClimbController;
    public bool IsAiming;
    public bool IsArrowReleasing;
    public bool IsArrowCharging;
    public bool IsJumping;
    public bool PreviousIsGround = true;
    public bool IsGround = true;
    public bool IsClimbable;
    public bool IsClimbing;
    public bool IsClimbPullUp;
    public bool IsLanding;
    private RaycastHit _climeTargetHit;
    private Vector2 _currentMoveInput;
    private float _ignoreGroundTimer; //ジャンプ時等に一時的に接地判定を無視するためのタイマー

    private void Update()
    {
        _currentMoveInput = new Vector2(Input.GetAxis("L_XAxis"), Input.GetAxis("L_YAxis"));

        IsLanding = _playerMoveController.IsLanding;
        IsClimbing = _playerClimbController.IsClimbing;
        IsClimbPullUp = _playerClimbController.IsPullUp;
        IsArrowReleasing = _playerBowController.IsArrowReleasing;
        IsArrowCharging = _playerBowController.IsArrowCharging;
        
        if (_ignoreGroundTimer < Mathf.Epsilon)
        {
            IsGround = Physics.Raycast(_rigidBody.position + new Vector3(0f, _groundCheckRayCastOffsetY, 0f), Vector3.down, out var hit, _groundCheckRayCastLength, _groundCheckRayCastLayerMask);
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
                    if (IsClimbing && !IsClimbPullUp)
                    {
                        _playerClimbController.ClimbEnd();
                    }
                }

            }
            PreviousIsGround = IsGround;
        }
        else
        {
            IsGround = false;
            _ignoreGroundTimer -= Time.deltaTime;
        }
             
        //壁の判定
        IsClimbable = Physics.Raycast(_playerClimbRayPoint.position , _playerClimbRayPoint.forward , out _climeTargetHit , _playerClimbRayLength , _climbLayerMask);
        
        if(IsClimbing && !IsClimbable && !IsClimbPullUp)
        {
            _playerClimbController.ClimbEnd();
        }
        
        if(IsGround && !_playerMoveController.IsLanding && !IsAiming && IsClimbable && Vector3.Dot(-_climeTargetHit.normal , new Vector3(_currentMoveInput.x , 0f , _currentMoveInput.y)) > _playerClimbThreshold)
        {
            _ignoreGroundTimer = _ignoreGroundTime;
            _playerClimbController.ClimbStart(hitWall: _climeTargetHit);
            //ClimbStart
        }
        
        if (Input.GetButtonDown("X"))
        {
            if ( IsClimbing || !IsClimbable && !IsClimbPullUp)
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
        
        if ((int)Input.GetAxisRaw("LT") == 1)
        {
            IsAiming = true;
            _bowObject.SetActive(true);
            _playerCameraController.ChangeMode(CameraMode.Aim);
        }
        
        if(IsAiming && (int)Input.GetAxisRaw("LT") == 0)
        {
            IsAiming = false;
            _bowObject.SetActive(false);
            _playerCameraController.ChangeMode(CameraMode.Normal);
        }
        
        if (IsAiming)
        {
            if ( (int)Input.GetAxisRaw("RT") == 1)
            {
                _playerBowController.ArrowCharge();
            }
        
            if(IsArrowCharging && (int)Input.GetAxisRaw("RT") == 0)
            {
                _playerBowController.ArrowRelease(canceled:false);
            } 
        }
        else
        {
            if (IsArrowCharging)
            {
                _playerBowController.ArrowRelease(canceled:true);
            }
        }
    }

    private void FixedUpdate()
    {
        if(IsClimbable && IsClimbing || IsClimbPullUp)
        {
            _playerClimbController.ClimbMove(_currentMoveInput , _climeTargetHit);
        }
        else
        {
            _playerMoveController.MovePlayer(_currentMoveInput , IsAiming , _playerCameraTransform);
        }
    }



}


