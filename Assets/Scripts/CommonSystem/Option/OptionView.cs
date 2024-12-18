using UnityEngine;
using UnityEngine.UI;

namespace CommonSystem.Option
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private GameObject _optionPanel;
        [SerializeField] private Button _closeButton;

        public void Initialize(System.Action onBackButtonPressed)
        {
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(() => onBackButtonPressed?.Invoke());
            }
        }

        /// <summary>オプションパネルを表示します。</summary>
        public void ShowOptionPanel()
        {
            _optionPanel.SetActive(true);
        }

        /// <summary>オプションパネルを非表示にします。</summary>
        public void HideOptionPanel()
        {
            _optionPanel.SetActive(false);
        }

        /// <summary>オプションパネルの状態を切り替えます。</summary>
        public void ToggleOptionPanel()
        {
            _optionPanel.SetActive(!_optionPanel.activeSelf);
        }
    }
}