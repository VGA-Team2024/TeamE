using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
struct TimelinePath
{
    [SerializeField] private string _sceneName;
    [SerializeField] private int _containerIndex;
    public string SceneName => _sceneName;
    public int ContainerIndex => _containerIndex;
}
public class TimelineManager : SingletonMonoBehavior<TimelineManager>
{
    [SerializeField] private List<TimelinePath> _timelineOrder;

    public async UniTaskVoid Play()
    {
        CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_opening"); 
        string prevScene = String.Empty;
        foreach (var path in _timelineOrder)
        {
            if (prevScene != path.SceneName)
            {
                await SceneManager.LoadSceneAsync(path.SceneName).ToUniTask();
                prevScene = path.SceneName;
            }
            var container = FindObjectOfType<TimelineContainer>();
            var director = container.PlayableDirectors[path.ContainerIndex];
            director.Play();
            await UniTask.WaitUntil(()=>director.time >= director.duration);
        }
        CRIAudioManager.BGM.Stop();
        SceneManager.LoadScene("Temple");
    }
}