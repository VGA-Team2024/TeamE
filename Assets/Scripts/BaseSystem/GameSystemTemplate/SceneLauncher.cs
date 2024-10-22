using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// シーン起動用
/// </summary>
public class SceneLauncher : MonoBehaviour
{
    //デバッグモード有効時はシーン移動用のメニューをオーバーライドする
    [SerializeField] bool _isDebug;
    [SerializeField] string _sceneName;
    [SerializeField] GameObject _base;

    private void Start()
    {
        if (_isDebug)
        {
            var sceneDB = Addressables.LoadAssetAsync<SceneDependencies>(SceneDependencies.AssetPath).WaitForCompletion();
            var scenes = sceneDB.GetAll().Where(s => s.CanIDebugSelect).ToList();

            List<SceneLaunchButton> _buttonList = new List<SceneLaunchButton>();

            Transform root = _base.transform.parent;
            _buttonList.Add(_base.GetComponent<SceneLaunchButton>());
            for (int i = 0; i < scenes.Count - 1; ++i)
            {
                _buttonList.Add(GameObject.Instantiate(_base, root).GetComponent<SceneLaunchButton>());
            }

            for (int i = 0; i < scenes.Count; ++i)
            {
                _buttonList[i].SetScene(scenes[i].Name);
            }
        }
        else
        {
            //
            SceneLoader.LoadScene(_sceneName);
        }
    }
}