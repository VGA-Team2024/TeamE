using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public enum TimelineType
{
    Opening,
    KillEnding,
    ReleaseEnding
}
public class TimelinePlayer: MonoBehaviour
{
    public static TimelineType TimelineType = TimelineType.Opening;
    [SerializeField] private List<PlayableDirector> _opening = new();
    [SerializeField] private PlayableDirector _killEnding;
    [SerializeField] private PlayableDirector _releaseEnding;
    private void Start()
    {
        switch (TimelineType)
        {
            case TimelineType.Opening:
                PlayOpening().Forget();
                break;
            case TimelineType.KillEnding:
                PlayKillEnding().Forget();
                break;
            case TimelineType.ReleaseEnding:
                PlayReleaseEnding().Forget();
                break;
        }
    }

    private async UniTaskVoid PlayOpening()
    {
        CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_opening");
        foreach (var director in _opening)
        {
            director.Play();
            await UniTask.WaitUntil(()=>director.time >= director.duration);
        }
        SceneManager.LoadScene("Temple");
    }

    private async UniTaskVoid PlayKillEnding()
    {
        CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_ending");
        _killEnding.gameObject.SetActive(true);
        await UniTask.Yield(destroyCancellationToken);
        _killEnding.Play();
    }

    private async UniTaskVoid PlayReleaseEnding()
    {
        CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_ending");
        _releaseEnding.gameObject.SetActive(true);
        await UniTask.Yield(destroyCancellationToken);
        _releaseEnding.Play();
    }
}