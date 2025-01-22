using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;
public class PlayerEventReceiver : MonoBehaviour
{
    [SerializeField] private float _landingSETimer = 0.6f;
    private Task _landingTask;
    private readonly string CueSheetSE = "CueSheet_SE";
    private readonly string FootSound = "SE_player_footsound";
    private readonly string Falling = "SE_player_falling";
    private readonly string Landing = "SE_player_landing";
    public bool ReceivedFallDamage { get; set; }

    public void PlaySE(string path)
    {
        var splitPath = path.Split(',');
        if (splitPath.Length < 2) return;
        CRIAudioManager.SE.Play(splitPath[0], splitPath[1]);
    }
    public void PlayFootSound()
    {
        var input = PlayerInputProvider.Instance.MoveValue;
        if (input is { x: 0, y: 0 }) return;
        CRIAudioManager.SE.Play(CueSheetSE, FootSound);
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