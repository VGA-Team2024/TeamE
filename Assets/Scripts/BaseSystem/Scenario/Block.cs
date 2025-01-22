using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    // Block内で実行するアクションのリスト
    public List<Action> commands = new List<Action>();

    // 次のBlock
    public Block nextBlock;

    // コンストラクタ
    public Block(List<Action> commands, Block nextBlock = null)
    {
        this.commands = commands;
        this.nextBlock = nextBlock;
    }

    // コマンドを順に実行するメソッド
    public void ExecuteCommands()
    {
        foreach (var command in commands)
        {
            command.Invoke(); // コマンドを実行
        }

        // 次のBlockがある場合は次に進む
        if (nextBlock != null)
        {
            nextBlock.ExecuteCommands();
        }
    }
}
