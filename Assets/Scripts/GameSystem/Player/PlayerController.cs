using System;
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
    public bool IsArrowCharging;
    public bool IsAiming;
    public bool IsJumping;
    public bool PreviousIsGround = true;
    public bool IsGround = true;
    public bool IsClimbable;
    public bool IsClimbing;
    public bool IsLanding;
    private RaycastHit _climeTargetHit;
    private Vector2 _currentMoveInput;
    private float _ignoreGroundTimer; //ジャンプ時等に一時的に接地判定を無視するためのタイマー

    private void Update()
    {
        _currentMoveInput = new Vector2(Input.GetAxis("L_XAxis"), Input.GetAxis("L_YAxis"));
        if(IsLanding && _playerMoveController.IsLanding)
        {
            IsLanding = false;
        }
        
        if (_ignoreGroundTimer < Mathf.Epsilon)
        {
            IsGround = Physics.Raycast(_rigidBody.position + new Vector3(0f, _groundCheckRayCastOffsetY, 0f), Vector3.down, out var hit, _groundCheckRayCastLength, _groundCheckRayCastLayerMask);
            _playerMoveController.SetIsGround(IsGround);
            if (IsGround)
            {
                if (!IsLanding && !PreviousIsGround && IsGround)
                {
                    IsLanding = true;
                    _playerMoveController.Landing();
                }
                if (!IsLanding)
                {
                    if (IsJumping)
                    {
                        IsJumping = false;
                    }
                    if (IsClimbing)
                    {
                        IsClimbing = false;
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
        
        if(IsClimbing && !IsClimbable)
        {
            IsClimbing = false;
            _playerClimbController.ClimbEnd();
        }
        
        if(IsGround && !_playerMoveController.IsLanding && !IsAiming && IsClimbable && Vector3.Dot(-_climeTargetHit.normal , new Vector3(_currentMoveInput.x , 0f , _currentMoveInput.y)) > _playerClimbThreshold)
        {
            IsClimbing = true;
            _ignoreGroundTimer = _ignoreGroundTime;
            _playerClimbController.ClimbStart();
            //ClimbStart
        }
        
        if (Input.GetButtonDown("X"))
        {
            if ( IsClimbing || !IsClimbable)
            {
                IsClimbing = false;
                _playerClimbController.ClimbEnd();
                //climb Cancel
            }
            if (IsGround && !IsJumping)
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
                IsArrowCharging = true;
            }
        
            if(IsArrowCharging && (int)Input.GetAxisRaw("RT") == 0)
            {
                _playerBowController.ArrowRelease(canceled:false);
                IsArrowCharging = false;
            } 
        }
        else
        {
            if (IsArrowCharging)
            {
                _playerBowController.ArrowRelease(canceled:true);
                IsArrowCharging = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if(IsClimbable && IsClimbing)
        {
            _playerClimbController.ClimbMove(_currentMoveInput , _climeTargetHit);
        }
        else
        {
            _playerMoveController.MovePlayer(_currentMoveInput , IsAiming , _playerCameraTransform);
        }
    }



}


