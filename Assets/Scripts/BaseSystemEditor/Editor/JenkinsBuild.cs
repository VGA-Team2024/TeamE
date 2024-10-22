using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;

public class BuildCommand
{
    [MenuItem("Assets/Build Application")]
    public static void Build()
    {
        //プラットフォーム、オプション
        bool isDevelopment = true;
        BuildTarget platform = BuildTarget.StandaloneWindows;

        // 出力名とか
        var exeName = PlayerSettings.productName;
        var ext = ".exe";
        var outpath = "C:\\Build\\";

        // ビルド対象シーンリスト
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        // Jenkins(コマンドライン)の引数をパース
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-projectPath":
                    outpath = args[i + 1] + "\\Build";
                    break;
                case "-devmode":
                    isDevelopment = args[i + 1] == "true";
                    break;
                case "-platform":
                    switch(args[i + 1])
                    {
                        case "Android":
                            platform = BuildTarget.Android;
                            ext = ".apk";
                            break;

                        case "Windows":
                            platform = BuildTarget.StandaloneWindows;
                            ext = ".exe";
                            break;

                        case "Switch":
                            platform = BuildTarget.Switch;
                            ext = "";
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        //ビルドオプションの成型
        var option = new BuildPlayerOptions();
        option.scenes = scenes;
        option.locationPathName = outpath + "\\" + exeName + ext;
        if (isDevelopment)
        {
            //optionsはビットフラグなので、|で追加していくことができる
            option.options = BuildOptions.Development | BuildOptions.AllowDebugging;
        }
        option.target = platform; //ビルドターゲットを設定. 今回はWin64

        // 実行
        var report = BuildPipeline.BuildPlayer(option);

        // 結果出力
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("BUILD SUCCESS");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError("BUILD FAILED");

            foreach(var step in report.steps)
            {
                Debug.Log(step.ToString());
            }

            Debug.LogError("Erro Count: " + report.summary.totalErrors);
            EditorApplication.Exit(1);
        }
    }

    [MenuItem("Assets/BuildAndCopyAddressables")]
    public static void BuildAndCopyAddressables()
    {
        var outPath = "Assets/Addressables";

        // ビルド対象シーンリスト
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (!Directory.Exists(outPath))
        {
            Directory.CreateDirectory(outPath);
        }

        // addressables_content_state.binを取得
        // ファイル選択パネルを出したい場合は引数をfalseに
        var path = ContentUpdateScript.GetContentStateDataPath(false);

        // 変更があった組み込みリソースを取得
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var modifiedEntries = ContentUpdateScript.GatherModifiedEntriesWithDependencies(settings, path);

        foreach (var modifiedEntry in modifiedEntries)
        {
            // 変更があったアセットのアドレスを出力
            Debug.Log(modifiedEntry.Key.address);
        }

        // Addressablesのビルド
        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
        
        //更新分においてローカルビルドのリソースIDを生成する

        //ローカルビルドをAWSにコピーする

    }

    static string sha256(string planeStr, string key)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] planeBytes = ue.GetBytes(planeStr);
        byte[] keyBytes = ue.GetBytes(key);

        System.Security.Cryptography.HMACSHA256 sha256 = new System.Security.Cryptography.HMACSHA256(keyBytes);
        byte[] hashBytes = sha256.ComputeHash(planeBytes);
        string hashStr = "";
        foreach (byte b in hashBytes)
        {
            hashStr += string.Format("{0,0:x2}", b);
        }
        return hashStr;
    }
}