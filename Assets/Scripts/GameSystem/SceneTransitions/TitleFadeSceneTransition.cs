using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleFadeSceneTransition : MonoBehaviour
{
    [SerializeField] string _inGameSceneName;
    [SerializeField] GameObject _fadePanel;
    Image _fadePanelImage;
    [Header("FadeTime+WaitTime=待ち時間")]
    [SerializeField] float _fadeTime;
    [SerializeField] float _waitTime;

    void Start()
    {
        _fadePanelImage = _fadePanel.GetComponent<Image>();
        _fadePanelImage.color = new Color(0f, 0f, 0f, 0f);
        _fadePanel.SetActive(false);
    }

    public void CollCoroutine()
    {
        StartCoroutine(SceneChange());
    }

    IEnumerator SceneChange()
    {
        if (_fadeTime != 0f)
        {
            _fadePanel.SetActive(true);
            float deltaTime = 0f;
            while (deltaTime < _fadeTime)
            {
                deltaTime += Time.deltaTime;
                var a = Mathf.Clamp01(deltaTime / _fadeTime);
                _fadePanelImage.color = new Color(0f, 0f, 0f, a);
                yield return null;
            }
            _fadePanelImage.color = new Color(0f, 0f, 0f, 1f);
        }
        yield return new WaitForSeconds(_waitTime);
        SceneManager.LoadScene(_inGameSceneName);
    }
}
