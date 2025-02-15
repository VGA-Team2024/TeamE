using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TitleFadeSceneTransition : MonoBehaviour
{
    [SerializeField] GameObject _fadePanel;
    Image _fadePanelImage;
    [Header("FadeTime+WaitTime=待ち時間")]
    [SerializeField] float _fadeTime;
    [SerializeField] float _waitTime;
    Color _startColor = new Color(0f, 0f, 0f, 0f);
    Color _endColor = Color.black;

    void Start()
    {
        _fadePanelImage = _fadePanel.GetComponent<Image>();
        _fadePanelImage.color = _startColor;
        _fadePanelImage.enabled = false;
    }
    public async UniTaskVoid FadeSceneTransition(Action callback)
    {
        if (_fadeTime != 0f)
        {
            _fadePanelImage.enabled = true;
            float deltaTime = 0f;
            while (deltaTime < _fadeTime)
            {
                deltaTime += Time.deltaTime;
                var a = Mathf.Clamp01(deltaTime / _fadeTime);
                _fadePanelImage.color = new Color(0f, 0f, 0f, a);
                await UniTask.Yield(destroyCancellationToken);
            }
            _fadePanelImage.color = _endColor;
        }//フェード時間が０の場合フェードせずにそのまま遷移

        await UniTask.WaitForSeconds(_waitTime, cancellationToken: destroyCancellationToken);
        callback?.Invoke();
    }
}