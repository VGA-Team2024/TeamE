using Cinemachine;
using UnityEngine;
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Vector3 _normalFollowOffset;
    [SerializeField] private Vector3 _aimFollowOffset;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private GameObject _reticuleImage;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _cameraLookAtTarget;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private bool _inverseX;
    [SerializeField] private bool _inverseY;
    [SerializeField] private float _arrowTargetOffsetX;
    [Header("X感度")] public float XSensibility = 1f;
    [Header("Y感度")] public float YSensibility = 1f;
    [SerializeField] [Header("YAxis上限角度")] private float _maxUpAngle = 40f;
    [SerializeField] [Header("YAxis下限角度")] private float _minDownAngle = -30f;
    private CinemachineTransposer _transposer;
    private Vector3 _defaultTargetPosition;
    private Vector2 _currentInput;
    private float _rotationX;
    private float _rotationY;
    private CameraMode _currentCameraMode;
    private Vector3 _currentOffset;
    [HideInInspector] public float ArrowChargeRate;

    private void Start()
    {
        _defaultTargetPosition = _cameraLookAtTarget.localPosition;
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        _currentOffset = _normalFollowOffset;
    }
    private void FixedUpdate()
    {
        _currentInput = PlayerInputProvider.Instance.LookValue;
        
        _rotationX += _inverseX ? -1 : 1 * _currentInput.x * XSensibility;
        _rotationY += _inverseY ? -1 : 1 * -_currentInput.y * YSensibility;
        _rotationY = Mathf.Clamp(_rotationY, -_maxUpAngle, -_minDownAngle);
        var playerPosition = _playerTransform.position;
        _cameraLookAtTarget.position = playerPosition + _defaultTargetPosition;
        _cameraLookAtTarget.rotation = Quaternion.Euler(-_rotationY, _rotationX, 0f);
        if (_currentCameraMode == CameraMode.Aim)
        {
            _transposer.m_FollowOffset = Vector3.Lerp(_normalFollowOffset, _aimFollowOffset, ArrowChargeRate);
        }
    }

    public void ChangeMode(CameraMode cameraMode)
    {
        _currentCameraMode = cameraMode;
        switch (cameraMode)
        {
            case CameraMode.Normal:
                _transposer.m_FollowOffset = _normalFollowOffset;
                _reticuleImage.SetActive(false);
                break;
            case CameraMode.Aim:
                _reticuleImage.SetActive(true);
                break;
        }
    }
}