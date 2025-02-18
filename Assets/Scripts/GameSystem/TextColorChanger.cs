using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextColorChanger : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private Color _textColor;
    private Color _originalColor;

    private void Awake()
    {
        _originalColor = _targetText.color;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _targetText.color = _textColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _targetText.color = _originalColor;
    }

    private void OnDisable()
    {
        _targetText.color = _originalColor;
    }
}
