using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GameEvent;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームのイベントを記録する
/// </summary>
public class ReviewTest : MonoBehaviour
{
    public void GameStart()
    {
        GameEventRecorder.GameStart();
    }

    public void GameEnd()
    {
        GameEventRecorder.GameEnd(ReviewEnd);
    }

    void ReviewEnd()
    {
        //
        //SceneLoader.LoadScene("Title");
    }
}