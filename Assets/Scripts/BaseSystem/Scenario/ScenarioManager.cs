using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    private Block currentBlock;

    // Startメソッドでシナリオ開始
    void Start()
    {
        SetupScenario(); // シナリオの初期設定
        currentBlock.ExecuteCommands(); // 最初のBlockを実行
    }

    // シナリオの設定
    void SetupScenario()
    {
        // コマンドを設定
        Block block1 = new Block(new List<Action> {
            () => Debug.Log("キャラクターがセリフを話す: 「こんにちは！」"),
            () => Debug.Log("選択肢：次のセリフに進む")
        });

        Block block2 = new Block(new List<Action> {
            () => Debug.Log("キャラクターがセリフを話す: 「次のステップに進みます。」")
        });

        // Blockのつながりを設定
        block1.nextBlock = block2;

        // 最初のBlockを設定
        currentBlock = block1;
    }
}
