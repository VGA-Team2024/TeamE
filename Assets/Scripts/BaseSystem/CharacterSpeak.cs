using UnityEngine;
using UnityEngine.Playables;

public class CharacterSpeak : MonoBehaviour
{
    public string[] dialogueLines; // キャラクターのセリフを格納する配列
    private int currentLineIndex = 0; // 現在のセリフのインデックス

    // セリフを表示するメソッド
    public void ShowNextDialogue()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            Debug.Log("キャラクターが話しています: " + dialogueLines[currentLineIndex]);
            currentLineIndex++;
        }
        else
        {
            Debug.Log("すべてのセリフが終了しました。");
        }
    }
}
