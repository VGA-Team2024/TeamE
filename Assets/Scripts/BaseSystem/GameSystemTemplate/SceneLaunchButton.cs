using UnityEngine;

/// <summary>
/// シーン起動用
/// </summary>
public class SceneLaunchButton : MonoBehaviour
{
    UnityEngine.UI.Text _text = null;
    UnityEngine.UI.Button _btn = null;
    string _sceneName = "";

    //シーン移動用のボタン
    //このボタンはチェックが入っているシーンだけ出る
    private void Awake()
    {
        _btn = GetComponentInChildren<UnityEngine.UI.Button>();
        _text = GetComponentInChildren<UnityEngine.UI.Text>();

        _btn.onClick.AddListener(OnClick);
    }

    public void SetScene(string str)
    {
        _text.text = str;
        _sceneName = str;
    }

    void OnClick()
    {
        SceneLoader.LoadScene(_sceneName);
    }
}