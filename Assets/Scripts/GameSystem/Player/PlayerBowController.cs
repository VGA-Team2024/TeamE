using UnityEngine;
using UnityEngine.Animations;
public class PlayerBowController : MonoBehaviour
{
    [SerializeField] private Transform _waistBone;
    [SerializeField] private float _arrowChargeTime;
    [SerializeField , Range(0f , 1f)] private float _lookAtWeight;
    [SerializeField] float _arrowTargetDistance = 100f;
    [SerializeField] private float _arrowInterpolationTime = 0.1f;
    [SerializeField] ParentConstraint _bowStringConstraint;
    [SerializeField] PositionConstraint _resetBowStringConstraint;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] GameObject _arrowObject;
    [SerializeField] GameObject _arrowParticle;
    [SerializeField] GameObject _arrowStart;
    [SerializeField] private Animator _animator;
    public bool IsArrowCharging;
    public bool IsArrowReleasing;
    private float _arrowInterpolationTimer;
    private float _arrowChargeTimer ;
    private int _arrowMotionLayerIndex;
    private int _aimJumpLayerIndex;
    //private ObservableStateMachineTrigger _stateMachineTrigger;
    private static readonly int Charge = Animator.StringToHash("ArrowCharge");
    private static readonly int Release = Animator.StringToHash("ArrowRelease");
    private void Start()
    {
        _arrowMotionLayerIndex = _animator.GetLayerIndex("Upper");
        _aimJumpLayerIndex = _animator.GetLayerIndex("AimJump");
        //_stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_arrowMotionLayerIndex];
    }
    void OnAnimatorIK(int layerIndex)
    {
        if (layerIndex == _arrowMotionLayerIndex)
        {
            if (IsArrowCharging)
            {
                _arrowInterpolationTimer += Time.deltaTime;
                if (_arrowInterpolationTimer > _arrowInterpolationTime)
                {
                    _arrowInterpolationTimer = _arrowInterpolationTime;
                }
            }
            else
            {
                if(_arrowInterpolationTimer > 0f)
                {
                    _arrowInterpolationTimer -= Time.deltaTime; 
                }
            }

            if (_arrowInterpolationTimer > 0f)
            {
                float chargeRate = (_arrowInterpolationTimer / _arrowInterpolationTime);
                if (IsArrowCharging || IsArrowReleasing)
                {
                    var arrowDestination = _cameraTransform.position + _cameraTransform.forward * _arrowTargetDistance;
                    _animator.SetLookAtWeight(_lookAtWeight);
                    _animator.SetLookAtPosition(arrowDestination); 
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand , chargeRate); 
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, arrowDestination);
                    //  胴がフォーカスしている高さに向くように回転させる。
                    var waist = _animator.GetBoneTransform(HumanBodyBones.Spine);
                    waist.RotateAround(waist.position, waist.up, -_cameraTransform.eulerAngles.x);
                    _animator.SetBoneLocalRotation(HumanBodyBones.Spine, waist.localRotation);
                }
            }
        }
    }

    public void ArrowCharge()
    {
        if (!IsArrowCharging)
        {
            IsArrowCharging = true;
            _arrowObject.SetActive(true);
            _resetBowStringConstraint.constraintActive = false;
            _bowStringConstraint.constraintActive = true;
            _animator.SetBool(Charge, true);
            _animator.SetLayerWeight(_aimJumpLayerIndex, 1);
        }
        _animator.SetLayerWeight(_arrowMotionLayerIndex, (_arrowChargeTimer / _arrowChargeTime));
        _arrowChargeTimer += Time.deltaTime;
    }
    public void ArrowRelease(bool canceled)
    {
        if (!canceled &&  _arrowChargeTimer > _arrowChargeTime)
        {
            if (!IsArrowReleasing)
            {
                _animator.SetTrigger(Release);
                _animator.SetBool(Charge, false);
                IsArrowReleasing = true;
                var arrowStart = _arrowStart.transform.position;
                var arrowDestination = _cameraTransform.position + _cameraTransform.forward * _arrowTargetDistance;
                var arrowDirection = arrowDestination - arrowStart;
                Instantiate(_arrowParticle, arrowStart, Quaternion.LookRotation(arrowDirection), null);
                Invoke(nameof(ResetBow), 0.2f);
                // _stateMachineTrigger
                //     .OnStateExitAsObservable()
                //     .Where(x => x.LayerIndex == _arrowMotionLayerIndex && x.StateInfo.IsName("ArrowRelease"))
                //     .Subscribe( _ => ResetBow())
                //     .AddTo(this);
            }
        }
        else
        {
            _animator.SetBool(Charge, false);
            IsArrowCharging = false;
            ResetBow();
        }
    }
    private void ResetBow()
    {
        IsArrowCharging = false;
        IsArrowReleasing = false;
        _arrowChargeTimer = 0f;
        _arrowObject.SetActive(false);
        _bowStringConstraint.constraintActive = false;
        _resetBowStringConstraint.constraintActive = true;
        _animator.SetLayerWeight(_arrowMotionLayerIndex, 0f);
        _animator.SetLayerWeight(_aimJumpLayerIndex, 0f);
    }
}
