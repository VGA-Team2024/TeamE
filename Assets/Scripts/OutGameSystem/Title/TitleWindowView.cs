using UnityEngine;
using UnityEngine.UI;

namespace OutGameSystem.Title
{
    public class TitleWindowView : MonoBehaviour
    {
        [SerializeField] private GameObject _titleWindow;
        [SerializeField] private GameObject _optionsWindow;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _optionButton;
        [SerializeField] private Button _exitButton;

        public void Initialize()
        {
        }
    }
}