using System.Collections;
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

    public void CollCoroutine()
    {
        StartCoroutine(FadeSceneTransition());
    }

    IEnumerator FadeSceneTransition()
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
                yield return null;
            }
            _fadePanelImage.color = _endColor;
        }//フェード時間が０の場合フェードせずにそのまま遷移
        yield return new WaitForSeconds(_waitTime);
        TimelineManager.Instance.Play().Forget();
    }
}