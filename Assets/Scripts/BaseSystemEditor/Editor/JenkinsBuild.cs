using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using System.Text;

public class BuildCommand
{
    [MenuItem("VTNTools/Build Application")]
    public static void Build()
    {
        //プラットフォーム、オプション
        bool isDevelopment = true;
        BuildTarget platform = BuildTarget.StandaloneWindows;

        // チーム名
        var teamID = "Foundation";

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
                case "-team":
                    teamID = args[i + 1].Trim();
                    break;
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
                        case "Mac":
                            platform = BuildTarget.StandaloneOSX;
                            ext = ".app";
                            // Macの場合は対象CPUアーキテクチャを設定する
                            PlayerSettings.SetArchitecture(BuildTargetGroup.Standalone, 2);
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

        // Dynamicパスにソースコードを生成
        BuildStateBuild(teamID);

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


    const string targetPath = "Assets/Scripts/BaseSystem/Dynamic";
    const string source = @"
public class BuildState
{
    const string _hash = ""<Hash>"";
    public const string TeamID = ""<TeamID>"";

    public static string BuildHash
    {
        get
        {
#if UNITY_EDITOR
            return ""UNITY_EDITOR"";
#else
            return _hash;
#endif
        }
    }
};";

    static public void BuildStateBuild(string teamID)
    {
        Directory.CreateDirectory(targetPath);

        //動的生成
        using (FileStream fs = new FileStream(targetPath + "/BuildState.cs", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            string sourceCode = source;
            sourceCode = sourceCode.Replace("<Hash>", Guid.NewGuid().ToString()); //ビルドハッシュを新規生成する
            sourceCode = sourceCode.Replace("<TeamID>", teamID); //チームID
            byte[] bytes = Encoding.UTF8.GetBytes(sourceCode);
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}