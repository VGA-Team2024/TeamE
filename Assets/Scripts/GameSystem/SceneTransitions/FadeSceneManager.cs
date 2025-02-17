using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeSceneManager : SingletonMonoBehavior<FadeSceneManager>
{
    [SerializeField] CanvasGroup _fadePanel;
    protected override void OnAwake()
    {
        _fadePanel.alpha = 0f;
        _fadePanel.interactable = false;
        _fadePanel.blocksRaycasts = false;
    }
    public async UniTaskVoid FadeLoadScene(string sceneName, float fadeTime, Action onComplete = null)
    {
        await _fadePanel.DOFade(1f, fadeTime).SetUpdate(true);
        await SceneManager.LoadSceneAsync(sceneName);
        await _fadePanel.DOFade(0f, fadeTime);
        onComplete?.Invoke();
    }
}