using System;
using System.Threading;
using R3;
using UnityEngine;

public class RagdollController : MonoBehaviour, IHasPlayerVariable
{
    [SerializeField] private Transform _pelvisTransform;
    [SerializeField] private Transform _rootTransform;
    [SerializeField] private Transform _bodyTransform;
    private Collider[] _colliders;
    private Rigidbody[] _rigidbodies;
    private PlayerVariable _variable;
    private ReactiveProperty<bool> _isActive = new();
    private Vector3 _rootOffset;
    private float[] _bodyAngles = new float[4];
    CancellationTokenSource _waitAnimationCts = new();
    public bool IsActive
    {
        get => _isActive.Value;
        set
        {
            _isActive.Value = value;
            SetRagdollEnabled(value);
        }
    }

    private void Start()
    {
        _colliders = GetComponentsInChildren<Collider>();
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        _rootOffset = _pelvisTransform.InverseTransformPoint(_rootTransform.position);
        _isActive.Skip(1).Subscribe(flag =>
        {
            if (flag)
            {
                _variable.CameraController.ChangeMode(CameraMode.KnockBack);
                _variable.IsKnockBack = true;
            }
            else
            {
                _variable.CameraController.ChangeMode(CameraMode.Normal);
                _variable.PlayerRoot.position = _pelvisTransform.TransformPoint(_rootOffset);
                _bodyAngles[(int)BodyDirection.DownForward] = Vector3.Angle(Vector3.down, _bodyTransform.forward);
                _bodyAngles[(int)BodyDirection.UpForward] =  Vector3.Angle(Vector3.up, _bodyTransform.forward);
                _bodyAngles[(int)BodyDirection.DownRight] = Vector3.Angle(Vector3.down, _bodyTransform.right);
                _bodyAngles[(int)BodyDirection.UpRight] = Vector3.Angle(Vector3.up, _bodyTransform.right);
                float minAngle = 180f;
                BodyDirection minDir = BodyDirection.None;
                for (int i = 0; i < 4; i++)
                {
                    if (_bodyAngles[i] <= minAngle)
                    {
                        minAngle = _bodyAngles[i];
                        minDir = (BodyDirection)i;
                    }
                }

                switch (minDir)
                {
                    case BodyDirection.DownForward:
                        _variable.Animator.Play("GetBackUpFront");
                        _variable.PlayerRoot.forward = new Vector3(_bodyTransform.up.x, 0f, _bodyTransform.up.z).normalized;
                        break;
                    case BodyDirection.UpForward:
                        _variable.Animator.Play("GetBackUpBack");
                        _variable.PlayerRoot.forward = -new Vector3(_bodyTransform.up.x, 0f, _bodyTransform.up.z).normalized;
                        break;
                    case BodyDirection.DownRight:
                        _variable.Animator.Play("GetBackUpRight");
                        _variable.PlayerRoot.right = new Vector3(_bodyTransform.up.x, 0f, _bodyTransform.up.z).normalized;
                        break;
                    case BodyDirection.UpRight:
                        _variable.Animator.Play("GetBackUpLeft");
                        _variable.PlayerRoot.right = -new Vector3(_bodyTransform.up.x, 0f, _bodyTransform.up.z).normalized;
                        break;
                }
                _waitAnimationCts?.Cancel();
                _waitAnimationCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
                //  Playと同じフレームでアニメーションの時間を取得できないので次のフレームでアニメーションの時間待つ処理を行う。
                Observable.NextFrame(_waitAnimationCts.Token)
                    .Subscribe(_ =>
                    {
                        Observable.Timer(TimeSpan.FromSeconds(_variable.Animator.GetCurrentAnimatorStateInfo(0).length), _waitAnimationCts.Token)
                            .Subscribe(_=>_variable.IsKnockBack = false).AddTo(this);
                    }).AddTo(this);
            }
        }).AddTo(this);
        IsActive = false;
    }

    private void Update()
    {
        if (_isActive.Value)
        {
            _variable.CameraController.VirtualPlayerPosition = _pelvisTransform.TransformPoint(_rootOffset);
        }
    }

    [ContextMenu("有効化")]
    public void Active()
    {
        IsActive = true;
        Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ => IsActive = false).AddTo(this);
    }
    private void SetRagdollEnabled(bool isEnabled)
    {
        foreach (var col in _colliders)
        {
            col.enabled = isEnabled;
        }

        foreach (var rb in _rigidbodies)
        {
            rb.isKinematic = !isEnabled;
        }

        foreach (var col in _variable.Colliders)
        {
            col.enabled = !isEnabled;
        }

        _variable.Rigidbody.isKinematic = isEnabled;
        _variable.Animator.enabled = !isEnabled;
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        if (!_isActive.Value) return;
        foreach (var rb in _rigidbodies)
        {
            rb.AddForce(force, mode);
        }
    }

    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }
    private enum BodyDirection
    {
        DownForward,
        UpForward,
        DownRight,
        UpRight,
        None
    }
}