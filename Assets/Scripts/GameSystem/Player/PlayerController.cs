using UnityEngine;

public enum CameraMode
{
    Normal ,
    Aim ,
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveInputThreshold;
    [SerializeField] private float _forwardSpeed = 7.0f;
    [SerializeField , Range(0f , 1f)] private float _aimSpeedDecrease;
    [SerializeField] private float _jumpPower = 3.0f;
    [SerializeField] private GameObject _bowObject;
    [SerializeField] Transform _playerCameraTransform;
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] private PlayerAnimationController _playerAnimationController;
    [SerializeField] private PlayerCameraController _playerCameraController;
    public bool IsArrowCharging;
    public bool IsAiming;
    private Vector2 _currentMoveInput;
    

    private void Update()
    {
        _currentMoveInput = new Vector2(Input.GetAxis("L_XAxis"), Input.GetAxis("L_YAxis"));
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
                _playerAnimationController.ArrowCharge();
                IsArrowCharging = true;
            }
        
            if(IsArrowCharging && (int)Input.GetAxisRaw("RT") == 0)
            {
                _playerAnimationController.ArrowRelease(canceled:false);
                IsArrowCharging = false;
            } 
        }
        else
        {
            if (IsArrowCharging)
            {
                _playerAnimationController.ArrowRelease(canceled:true);
                IsArrowCharging = false;
            }
        }
        

        
        _playerAnimationController.SetLocomotionSpeed(_rigidBody.velocity.magnitude / _forwardSpeed);
    }

    private void FixedUpdate()
    {
        MovePlayer(_currentMoveInput);
    }


    private void MovePlayer(Vector2 input)
    {
        var forward = _playerCameraTransform.transform.forward;
        var right = _playerCameraTransform.transform.right;

        if (IsAiming)
        {
            //弓を引いている間は、プレイヤーが動いてなくともカメラ向きを計算する。
            transform.forward = new Vector3(forward.x, 0f , forward.z);
        }
        
        if (Mathf.Abs(_currentMoveInput.magnitude) > _moveInputThreshold)
        {
            var moveX = new Vector3(forward.x, 0f, forward.z).normalized * input.y;
            var moveZ = new Vector3(right.x, 0f, right.z).normalized * input.x;
            var dir = ((moveX + moveZ).magnitude < 1f ? (moveX + moveZ) : (moveX + moveZ).normalized);
            if (!IsAiming)
            {
                //プレイヤーが動いている時のみ、カメラ向きと合わせる。
                transform.forward = dir; 
                _rigidBody.velocity = transform.forward * (dir.magnitude * _forwardSpeed );
            }
            else
            {
                _rigidBody.velocity = dir * (_forwardSpeed * _aimSpeedDecrease); 
            }
           
        }
        


    }
}


