using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームのイベントを記録する
/// </summary>
public class ReviewStar : MonoBehaviour
{
    [SerializeField] int _index = 1;
    [SerializeField] TextMeshProUGUI _offStar;
    [SerializeField] TextMeshProUGUI _onStar;
    [SerializeField] TextMeshProUGUI _label;

    public int Index => +_index;


    public void Setup()
    {
        _label.text = _index.ToString();
        SetStar(false);
    }

    public void SetStar(bool isOn)
    {
        if(isOn)
        {
            _offStar.enabled = false;
            _onStar.enabled = true;
        }
        else
        {
            _offStar.enabled = true;
            _onStar.enabled = false;
        }
    }
}