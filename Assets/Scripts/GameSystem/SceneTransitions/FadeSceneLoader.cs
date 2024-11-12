using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class FadeSceneLoader : MonoBehaviour
{
    public Image fadePanelImage;
    public GameObject FadePanel;
    [SerializeField] private float _fadeDuration = 1.0f;
    private UiInputSystem _FadeUiInputSystem;
    public FadeSceneLoader fadeSceneLoader;
    bool Panel_SetActive = true;

    void Start()
    {
        _FadeUiInputSystem = new UiInputSystem();
        _FadeUiInputSystem.Enable();

    }

    void Update()
    {
        if (Panel_SetActive == true)
        {
            if (_FadeUiInputSystem.UI.Submit.triggered)
            {
                fadeSceneLoader.CallCoroutine();
            }
        }
    }

    void OnDisable()
    {
        _FadeUiInputSystem.Disable();
    }

    public void CallCoroutine()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    public IEnumerator FadeOutAndLoadScene()
    {
        fadePanelImage.enabled = true;                
        float elapsedTime = 0.0f;                 
        Color startColor = fadePanelImage.color;       
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f); 

        // フェードアウトアニメーションを実行
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;                       
            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);  // フェードの進行度を計算
            fadePanelImage.color = Color.Lerp(startColor, endColor, t); 
            yield return null;                                     
        }

        fadePanelImage.color = endColor;  // フェードが完了したら最終色に設定
        FadePanel.SetActive(false);
        Panel_SetActive = false;
    }
}
