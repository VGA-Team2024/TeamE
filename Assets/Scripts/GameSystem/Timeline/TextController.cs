using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private List<string> _data;
    private int _currentIndex = 0;

    public void DisplayText()
    {
        if (_currentIndex < _data.Count)
        {
            _text.text = _data[_currentIndex];
            _currentIndex++;
        }
    }

    public void ClearText()
    {
        _text.text = "";
    }
}