using NUnit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Content;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// ビルド前に実行する処理
/// </summary>
public class PreprocessBuild : IPreprocessBuildWithReport
{
    public int callbackOrder => 0; // ビルド前処理の中での処理優先順位 (0で最高)

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log($"IPreprocessBuildWithReport.OnPreprocessBuild for {report.summary.platform} at {report.summary.outputPath}");

        //ビルドバリデーション

        //Addressables
        AddressableCheck("Assets/Scenes/SceneDependencies.asset");
        AddressableCheck("Assets/Prefabs/Review/Review.prefab");
        AddressableCheck("Assets/Prefabs/CRI.prefab");
        AddressableCheck("Assets/ThirdParty/Shapes2D/Materials/Shape.mat");

        //アプリ名がFoundation
        if (PlayerSettings.productName == "Foundation")
        {
            throw new BuildFailedException("アプリ名(Foundation)を変更してください");
        }

        //IL2CPP
        if(PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup).ToString() != "IL2CPP")
        {
            //書き換える
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
            Debug.LogWarning("ScriptingBackendをIL2CPPに変更しました");
        }

        //シーンランチャ―を最初に登録していること
        var scene = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .First();

        if(!scene.Contains("GameLauncher"))
        {
            Debug.LogWarning("GameLauncherが起動シーンではないので初期化が失敗する可能性があります");
        }

        //ゲームランチャー設定確認
        {
            var gl = EditorBuildSettings.scenes
                .Where(scene => scene.path.Contains("GameLauncher"))
                .Select(scene => scene.path)
                .First();

            var grep = File.ReadAllLines(gl).Where(l => l.Contains("isDebug")).First();

            Debug.Log(grep);
            if (grep.Contains("1"))
            {
                Debug.LogWarning("GameLauncherのデバッグが有効です。提出ビルドの場合は注意しましょう。");
            }
        }


        //実装確認
        int implLine = 0;
        var files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
        foreach(var f in files)
        {
            if (f.Contains("ReviewTest.cs")) continue;
            if (f.Contains("GameEventRecorder.cs")) continue;
            if (f.Contains("PreprocessBuild.cs")) continue;

            var grep = File.ReadAllLines(f)
                .Select((s, i) => new { Index = i, Value = s })
                .Where(s => s.Value.Contains("GameEventRecorder.GameStart")
                            || s.Value.Contains("GameEventRecorder.Review")
                            || s.Value.Contains("GameEventRecorder.GameEnd"));

            implLine += grep.Count();
            foreach(var g in grep)
            {
                Debug.Log(g);
            }
        }
        if(implLine == 0)
        {
            throw new BuildFailedException("レビュー機能が実装されていません");
        }

        //ビルドハッシュの更新
        // Dynamicパスにソースコードを生成
        BuildCommand.BuildStateBuild(BuildState.TeamID);
    }

    void AddressableCheck(string path)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            throw new BuildFailedException($"そもそもAddressablesの設定がされていません。");
        }

        var guid = AssetDatabase.AssetPathToGUID(path);
        if (guid == null)
        {
            throw new BuildFailedException($"対象のアセットがありませんでした。[{path}]");
        }

        var find = settings.FindAssetEntry(guid);
        if (find == null)
        {
            throw new BuildFailedException($"Addressablesに登録されていません。[{path}]");
        }
    }
}