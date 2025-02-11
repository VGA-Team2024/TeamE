using Cinemachine;
using UnityEngine;
public enum CameraMode
{
    Normal,
    Aim,
    KnockBack
}
public class PlayerCameraController : MonoBehaviour, IHasPlayerVariable
{
    [SerializeField] private Vector3 _normalFollowOffset;
    [SerializeField] private Vector3 _aimFollowOffset;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private GameObject _reticuleImage;
    [SerializeField] private Transform _cameraLookAtTarget;
    [SerializeField] private Transform _cameraFollow;
    [SerializeField] private bool _inverseX;
    [SerializeField] private bool _inverseY;
    [SerializeField] private float _arrowTargetOffsetX;
    [SerializeField, InspectorVariantName("X感度")] private float _xSensibility = 1f;
    [SerializeField, InspectorVariantName("Y感度")] private float _ySensibility = 1f;
    [SerializeField, InspectorVariantName("Y軸上限角度")] private float _maxUpAngle = 40f;
    [SerializeField, InspectorVariantName("Y軸下限角度")] private float _minDownAngle = -30f;
    private CinemachineTransposer _transposer;
    private Vector3 _defaultTargetPosition;
    private Vector2 _currentInput;
    private float _rotationX;
    private float _rotationY;
    private CameraMode _currentCameraMode;
    private PlayerVariable _variable;
    public Vector3 VirtualPlayerPosition { get; set; }

    private void Start()
    {
        _defaultTargetPosition = _cameraLookAtTarget.localPosition;
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }
    private void FixedUpdate()
    {
        _currentInput = PlayerInputProvider.Instance.LookValue;
        
        _rotationX += _inverseX ? -1 : 1 * _currentInput.x * _xSensibility;
        _rotationY += _inverseY ? -1 : 1 * -_currentInput.y * _ySensibility;
        _rotationY = Mathf.Clamp(_rotationY, -_maxUpAngle, -_minDownAngle);
        var playerPosition = _variable.PlayerRoot.position;
        if (_currentCameraMode == CameraMode.KnockBack)
        {
            _cameraLookAtTarget.position = VirtualPlayerPosition + _defaultTargetPosition;
        }
        else
        {
            _cameraLookAtTarget.position = playerPosition + _defaultTargetPosition;
        }
        _cameraLookAtTarget.rotation = Quaternion.Euler(-_rotationY, _rotationX, 0f);
        if (_currentCameraMode == CameraMode.Aim)
        {
            _transposer.m_FollowOffset = Vector3.Lerp(_normalFollowOffset, _aimFollowOffset, _variable.ArrowChargeRate);
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
            case CameraMode.KnockBack:
                _transposer.m_FollowOffset = _normalFollowOffset;
                _reticuleImage.SetActive(false);
                break;
        }
    }

    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }
}