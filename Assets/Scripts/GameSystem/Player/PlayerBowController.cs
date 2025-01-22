using UnityEngine;
using UnityEngine.Animations;
public class PlayerBowController : MonoBehaviour, IHasPlayerVariable
{
    [SerializeField] private float _arrowChargeTime;
    [SerializeField , Range(0f , 1f)] private float _lookAtWeight;
    [SerializeField] float _arrowTargetDistance = 100f;
    [SerializeField] private float _arrowInterpolationTime = 0.1f;
    [SerializeField] ParentConstraint _bowStringConstraint;
    [SerializeField] PositionConstraint _resetBowStringConstraint;
    [SerializeField] GameObject _arrowObject;
    [SerializeField] GameObject _arrowParticle;
    [SerializeField] GameObject _arrowStart;
    private bool _isArrowReleasing;
    private float _arrowInterpolationTimer;
    private float _arrowChargeTimer ;
    private int _arrowMotionLayerIndex;
    private int _aimJumpLayerIndex;
    private PlayerVariable _variable;
    private void Start()
    {
        _arrowMotionLayerIndex = _variable.Animator.GetLayerIndex("Upper");
        _aimJumpLayerIndex = _variable.Animator.GetLayerIndex("AimJump");
        //_stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_arrowMotionLayerIndex];
    }
    void OnAnimatorIK(int layerIndex)
    {
        if (_variable.IsArrowCharging)
        {
            _arrowInterpolationTimer += Time.deltaTime;
            if (_arrowInterpolationTimer > _arrowInterpolationTime)
            {
                _arrowInterpolationTimer = _arrowInterpolationTime;
            }
        }
        else
        {
            _arrowInterpolationTimer = 0f;
        }
        if (layerIndex == _arrowMotionLayerIndex)
        {
            if (_arrowInterpolationTimer > 0f)
            {
                float chargeRate = _arrowInterpolationTimer / _arrowInterpolationTime;
                if (_variable.IsArrowCharging || _isArrowReleasing)
                {
                    var arrowDestination = _variable.CameraTransform.position + _variable.CameraTransform.forward * _arrowTargetDistance;
                    _variable.Animator.SetLookAtWeight(_lookAtWeight);
                    _variable.Animator.SetLookAtPosition(arrowDestination); 
                    _variable.Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand , chargeRate); 
                    _variable.Animator.SetIKPosition(AvatarIKGoal.LeftHand, arrowDestination);
                    //  胴がフォーカスしている高さに向くように回転させる。
                    var waist = _variable.Animator.GetBoneTransform(HumanBodyBones.Spine);
                    waist.RotateAround(waist.position, waist.up, -_variable.CameraTransform.eulerAngles.x);
                    _variable.Animator.SetBoneLocalRotation(HumanBodyBones.Spine, waist.localRotation);
                }
            }
        }
    }

    public float ArrowCharge()
    {
        if (!_variable.IsArrowCharging)
        {
            _variable.IsArrowCharging = true;
            _arrowObject.SetActive(true);
            _resetBowStringConstraint.constraintActive = false;
            _bowStringConstraint.constraintActive = true;
            _variable.Animator.SetBool(AnimHashUtil.Charge, true);
            _variable.Animator.SetLayerWeight(_aimJumpLayerIndex, 1);
        }

        var chargeRate = _arrowChargeTimer / _arrowChargeTime;
        _variable.Animator.SetLayerWeight(_arrowMotionLayerIndex, 1);
        if(_arrowChargeTimer < _arrowChargeTime) _arrowChargeTimer += Time.deltaTime;
        return chargeRate;
    }
    public void ArrowRelease(bool canceled)
    {
        if (!canceled &&  _arrowChargeTimer > _arrowChargeTime)
        {
            if (!_isArrowReleasing)
            {
                CRIAudioManager.SE.Play("CueSheet_SE", "SE_player_bow_shot");
                _variable.Animator.SetTrigger(AnimHashUtil.Release);
                _variable.Animator.SetBool(AnimHashUtil.Charge, false);
                _isArrowReleasing = true;
                var arrowStart = _arrowStart.transform.position;
                var arrowDestination = _variable.CameraTransform.position + _variable.CameraTransform.forward * _arrowTargetDistance;
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
            _variable.Animator.SetBool(AnimHashUtil.Charge, false);
            _variable.IsArrowCharging = false;
            ResetBow();
        }
    }
    private void ResetBow()
    {
        _variable.IsArrowCharging = false;
        _isArrowReleasing = false;
        _arrowChargeTimer = 0f;
        _arrowObject.SetActive(false);
        _bowStringConstraint.constraintActive = false;
        _resetBowStringConstraint.constraintActive = true;
        _variable.Animator.SetLayerWeight(_arrowMotionLayerIndex, 0f);
        _variable.Animator.SetLayerWeight(_aimJumpLayerIndex, 0f);
    }

    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }
}
