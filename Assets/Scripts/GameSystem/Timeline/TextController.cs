using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField, TextArea] private List<string> _data;
    private int _currentIndex = 0;

    [SerializeField] private bool _disableAutoWrapping = true;

    public void Awake()
    {
        // 自動改行を無効化
        if (_disableAutoWrapping)
        {
            _text.enableWordWrapping = false;
        }
    }

    public void DisplayText()
    {
        if (_currentIndex < _data.Count)
        {
            string formattedText = _data[_currentIndex].Replace("/n", "\n");

            _text.text = formattedText;
            _currentIndex++;
        }
    }

    public void ClearText()
    {
        _text.text = "";
    }
}