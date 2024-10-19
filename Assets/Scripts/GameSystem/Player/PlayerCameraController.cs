using System;
using UnityEngine;
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private GameObject _reticleImage;
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
    [SerializeField] [Header("ターゲットに与える初期回転")] private float _playerRotationY;

    private Vector3 _defaultTargetPosition;
    private Vector2 _currentInput;
    private float _rotationX;
    private float _rotationY;

    private void Start()
    {
        _defaultTargetPosition = _cameraLookAtTarget.position;
    }
    private void Update()
    {
        _currentInput = new Vector2(Input.GetAxis("R_XAxis"), Input.GetAxis("R_YAxis"));
    }
    private void FixedUpdate()
    {
        _rotationX += _inverseX ? -1 : 1 * _currentInput.x * XSensibility;
        _rotationY += _inverseY ? -1 : 1 * -_currentInput.y * YSensibility;
        _rotationY = Mathf.Clamp(_rotationY, -_maxUpAngle, -_minDownAngle);
        var playerPosition = _playerTransform.position;
        _cameraLookAtTarget.position = new Vector3(playerPosition.x, _defaultTargetPosition.y, playerPosition.z);
        _cameraLookAtTarget.rotation = Quaternion.Euler(-_rotationY, _rotationX, 0f);
    }

    public void ChangeMode(CameraMode cameraMode)
    {
        switch (cameraMode)
        {
            case CameraMode.Normal:
                _cameraFollow.localPosition = Vector3.zero;
                _reticleImage.SetActive(false);
                break;
            case CameraMode.Aim:
                _cameraFollow.localPosition = Vector3.right * _arrowTargetOffsetX;
                _reticleImage.SetActive(true);
                break;
        }
    }
}