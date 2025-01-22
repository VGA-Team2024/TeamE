using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TextStorage : MonoBehaviour
{
    public Text _text;
    public string[] conversation;
    public string[] anotherconversation;
    int x;
    int y;
    public void textyes()
    {
        _text.text = (y <= conversation.Length - 1) ? conversation[y].ToString() : "";
        y = (y <= conversation.Length - 1) ? y + 1 : y;

    }
    public void textno()
    {
        _text.text = (x <= anotherconversation.Length - 1) ? anotherconversation[x].ToString() : "";
        x = (x <= anotherconversation.Length - 1) ? x + 1 : x;

    }
}