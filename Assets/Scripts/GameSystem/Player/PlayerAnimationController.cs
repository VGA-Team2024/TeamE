using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Animations;
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private float _arrowChargeTime;
    [SerializeField , Range(0f , 1f)] private float _lookAtWeight;
    [SerializeField] float _arrowTargetDistance = 100f;
    [SerializeField] ParentConstraint _bowStringConstraint;
    [SerializeField] PositionConstraint _resetBowStringConstraint;

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] GameObject _arrowObject;
    [SerializeField] GameObject _arrowParticle;
    [SerializeField] GameObject _arrowStart;
    [SerializeField] private Animator _animator;
    bool _isArrowChageing;
    bool _isArrowTaking;
    float _arrowInterpolationTimer;
    [SerializeField] private float _arrowInterpolationTime = 0.1f;
    float _arrowChargeTimer ;
    int _arrowMotionLayerIndex;
    int _WalkingLayerIndex;
    
    private ObservableStateMachineTrigger _stateMachineTrigger;
    private static readonly int Charge = Animator.StringToHash("ArrowCharge");
    private static readonly int Release = Animator.StringToHash("ArrowRelease");
    
    static readonly int Speed = Animator.StringToHash("Speed");
    private void Start()
    {
        _arrowMotionLayerIndex = _animator.GetLayerIndex("ArrowMotionLayer");
        _WalkingLayerIndex = _animator.GetLayerIndex("WalkingMask");
        _stateMachineTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
    }
    void OnAnimatorIK(int layerIndex)
    {
        if (_isArrowTaking)
        {

            
        }
        if (layerIndex == _arrowMotionLayerIndex)
        {
 
            if (_isArrowTaking)
            {
                _arrowInterpolationTimer += Time.deltaTime;
                if (_arrowInterpolationTimer > _arrowInterpolationTime)
                {
                    _arrowInterpolationTimer = _arrowInterpolationTime;
                }

            }
            else
            {
                _arrowInterpolationTimer -= Time.deltaTime;
                if (_arrowInterpolationTimer < 0f)
                {
                    _arrowInterpolationTimer = 0f;
                }
            }
            float chargeRate = (_arrowInterpolationTimer / _arrowInterpolationTime);
            _animator.bodyRotation *= Quaternion.Euler(0, 90 * chargeRate, 0);
            
            var arrowDestination = _cameraTransform.position + _cameraTransform.forward * _arrowTargetDistance;
            _animator.SetLookAtWeight(_lookAtWeight);
            _animator.SetLookAtPosition(arrowDestination); 
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand , chargeRate); 
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, arrowDestination);   
        }

        
    }

    public void SetLocomotionSpeed(float normalizedSpeed)
    {
        _animator.SetFloat(Speed , normalizedSpeed);
    }
    public void ArrowCharge()
    {
        if (!_isArrowChageing)
        {
            _isArrowChageing = true;
            _isArrowTaking = true;
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
        if (!canceled && _arrowChargeTimer > _arrowChargeTime)
        {
            _animator.SetTrigger(Release);
            _animator.SetBool(Charge, false);
            _isArrowTaking = false;
            _stateMachineTrigger
                .OnStateExitAsObservable()
                .Where(x => x.LayerIndex == _arrowMotionLayerIndex && x.StateInfo.IsName("ArrowRelease"))
                .Subscribe( _ => ResetCharge())
                .AddTo(this);

            var arrowStart = _arrowStart.transform.position;
            var arrowDestination = _cameraTransform.position + _cameraTransform.forward * _arrowTargetDistance;
            var arrowDirection = arrowDestination - arrowStart;
            Instantiate(_arrowParticle, arrowStart, Quaternion.LookRotation(arrowDirection), null);

        }
        else
        {
            _animator.SetBool(Charge, false);
            _isArrowTaking = false;
            ResetCharge();
        }
    }

    private void ResetCharge()
    {
        _arrowChargeTimer = 0f;
        _arrowObject.SetActive(false);
        _bowStringConstraint.constraintActive = false;
        _resetBowStringConstraint.constraintActive = true;
        _isArrowChageing = false;
        _animator.SetLayerWeight(_arrowMotionLayerIndex, 0f);
    }
    public void JumpWait()
    {
        _animator.SetBool("JumpWait", true);
    }
    public void JumpUp()
    {
        _animator.SetBool("Jump", true);
    }
    public void JumpEnd()
    {
        _animator.SetBool("JumpWait", false);
        _animator.SetBool("Jump", false);
    }
    
       
}
