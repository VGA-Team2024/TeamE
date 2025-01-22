using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockWithChoices : Block
{
    public List<Choice> choices;

    public BlockWithChoices(List<Action> commands, List<Choice> choices, Block nextBlock = null)
        : base(commands, nextBlock)
    {
        this.choices = choices;
    }

    // Overrideして選択肢を表示
    public void ShowChoices()
    {
        foreach (var choice in choices)
        {
            Debug.Log("選択肢：" + choice.description);
        }
    }

    public void SelectChoice(int index)
    {
        if (index < choices.Count)
        {
            choices[index].onSelect.Invoke(); // 選択されたアクションを実行
        }
    }
}
