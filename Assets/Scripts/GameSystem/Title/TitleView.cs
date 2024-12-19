using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameSystem.Title
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _optionButton;
        [SerializeField] private Button _exitButton;

        public void SetButtonListeners(Action onStart, Action onOption, Action onExit)
        {
            if (_startButton != null)
                _startButton.onClick.AddListener(() => onStart?.Invoke());

            if (_optionButton != null)
                _optionButton.onClick.AddListener(() => onOption?.Invoke());

            if (_exitButton != null)
                _exitButton.onClick.AddListener(() => onExit?.Invoke());
        }
    }
}