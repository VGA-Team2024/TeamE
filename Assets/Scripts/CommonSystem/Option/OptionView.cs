using UnityEngine;

namespace CommonSystem.Option
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private GameObject _optionPanel;

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