using UnityEngine;

public enum CameraMode
{
    Normal ,
    Aim ,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject _bowObject;
    [SerializeField] Transform _playerCameraTransform;
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] private PlayerMoveController _playerMoveController;
    [SerializeField] private PlayerBowController _playerBowController;
    [SerializeField] private PlayerCameraController _playerCameraController;
    [SerializeField] private float _groundCheckRayCastOffsetY;
    [SerializeField] private float _groundCheckRayCastLength;
    [SerializeField] private LayerMask _groundCheckRayCastLayerMask;
    public bool IsArrowCharging;
    public bool IsAiming;
    public bool IsJumping;
    public bool IsGround;
    private Vector2 _currentMoveInput;
    private float _ignoreGroundTimer;
    
    private void Update()
    {
        if (_ignoreGroundTimer < Mathf.Epsilon)
        { 
            IsGround = Physics.Raycast(_rigidBody.position + new Vector3(0f, _groundCheckRayCastOffsetY, 0f), Vector3.down, out var hit, _groundCheckRayCastLength, _groundCheckRayCastLayerMask);
            _playerMoveController.SetIsGround(IsGround);
            if (IsGround)
            {
                //_rigidBody.position = new Vector3(_rigidBody.position.x , hit.point.y, _rigidBody.position.z);
                if (IsJumping)
                {
                    IsJumping = false;
                    _playerMoveController.JumpStart();
                }
            }
        }
        else
        {
            _ignoreGroundTimer -= Time.deltaTime;
        }
        
        _currentMoveInput = new Vector2(Input.GetAxis("L_XAxis"), Input.GetAxis("L_YAxis"));

        if (IsGround && Input.GetButtonDown("X"))
        {
            IsJumping = true;
            _playerMoveController.JumpStart();
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
        _playerMoveController.MovePlayer(_currentMoveInput , IsAiming , _playerCameraTransform);
    }



}


