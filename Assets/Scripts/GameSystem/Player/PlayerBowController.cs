using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Animations;
public class PlayerBowController : MonoBehaviour
{
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
    private ObservableStateMachineTrigger _stateMachineTrigger;
    private static readonly int Charge = Animator.StringToHash("ArrowCharge");
    private static readonly int Release = Animator.StringToHash("ArrowRelease");
    private void Start()
    {
        _arrowMotionLayerIndex = _animator.GetLayerIndex("ArrowMotionLayer");
        _stateMachineTrigger = _animator.GetBehaviours<ObservableStateMachineTrigger>()[_arrowMotionLayerIndex];
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
                _animator.bodyRotation *= Quaternion.Euler(0, 90 * chargeRate, 0);
                if (IsArrowCharging || IsArrowReleasing)
                {
                    var arrowDestination = _cameraTransform.position + _cameraTransform.forward * _arrowTargetDistance;
                    _animator.SetLookAtWeight(_lookAtWeight);
                    _animator.SetLookAtPosition(arrowDestination); 
                    _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand , chargeRate); 
                    _animator.SetIKPosition(AvatarIKGoal.LeftHand, arrowDestination);   
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
                _stateMachineTrigger
                    .OnStateExitAsObservable()
                    .Where(x => x.LayerIndex == _arrowMotionLayerIndex && x.StateInfo.IsName("ArrowRelease"))
                    .Subscribe( _ => ResetBow())
                    .AddTo(this);
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
    }
}
