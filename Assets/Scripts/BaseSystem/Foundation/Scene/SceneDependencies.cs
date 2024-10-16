using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// シーン依存関係の設定用データ
/// TODO: 動的生成物なのでアセットメニューには載せない
/// </summary>
public class SceneDependencies : ScriptableObject
{
    public const string ScenePath = "/Scenes";
    public const string SOAssetPath = ScenePath + "/SceneDependencies.asset";

    [System.Serializable]
    public class Dependencies
    {
        public string Name;
        public string AssetPath;
        public SceneType SceneType;
    }

    [SerializeField] List<Dependencies> _dependencies;
    
    public Dependencies Get(string name)
    {
        return _dependencies.Where(d => d.Name == name).FirstOrDefault();
    }

    //生成コードなどはEditorにある

#if UNITY_EDITOR
    public void Set(List<Dependencies> dp) { _dependencies = dp; }
#endif
}
