using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;

public class PlayerEventReceiver : MonoBehaviour
{
    private static readonly int cFootSound = Animator.StringToHash("FootSound");
    private static readonly int cClimbSound = Animator.StringToHash("ClimbSound");
    [SerializeField] private float _landingSETimer = 0.6f;
    [SerializeField] float _footSoundThreshold = -0.03f;
    [SerializeField] float _climbSoundThreshold = -0.03f;
    private Task _landingTask;
    private Task _footStepTask;
    private readonly string FootSound = "SE_player_footsound_1";
    private readonly string CueSheetSE = "CueSheet_SE";
    private readonly string Falling = "SE_player_falling";
    private readonly string Landing = "SE_player_landing";
    private readonly string Climb = "SE_player_climb_1";
    private Animator _animator;
    private ReactiveProperty<bool> _footSoundRP = new();
    private readonly ReactiveProperty<bool> _climbSoundRP = new();
    public bool ReceivedFallDamage { get; set; }
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _footSoundRP.Subscribe(flag =>
        {
            if (flag)
            {
                CRIAudioManager.SE.Play(CueSheetSE, FootSound);
            }
        }).AddTo(this);
        _climbSoundRP.Subscribe(flag =>
        {
            if (flag)
            {
                CRIAudioManager.SE.Play(CueSheetSE, Climb);
            }
        }).AddTo(this);
    }
    private void Update()
    {
        var footSound = _animator.GetFloat(cFootSound);
        var climbSound = _animator.GetFloat(cClimbSound);
        _footSoundRP.Value = Mathf.Abs(footSound) <= 1f && footSound <= _footSoundThreshold;
        _climbSoundRP.Value = Mathf.Abs(climbSound) <= 1f && climbSound <= _climbSoundThreshold;
    }

    public void PlaySE(string path)
    {
        var splitPath = path.Split(',');
        if (splitPath.Length < 2) return;
        CRIAudioManager.SE.Play(splitPath[0], splitPath[1]);
    }

    public void PlayLanding()
    {
        if (_landingTask is { IsCompleted: false, IsCanceled: false }) return;
        _landingTask = Observable.Timer(TimeSpan.FromSeconds(_landingSETimer)).WaitAsync(destroyCancellationToken);
        if (ReceivedFallDamage)
            CRIAudioManager.SE.Play(CueSheetSE, Falling);
        else
            CRIAudioManager.SE.Play(CueSheetSE, Landing);
    }
}