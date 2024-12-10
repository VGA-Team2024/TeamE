using UnityEngine;

namespace CommonSystem.Option
{
    public class OptionWindowPresenter : MonoBehaviour
    {
        [SerializeField] private OptionWindowView _optionWindowView;
        private OptionWindowModel _model;

        public void Initialize()
        {
            _model = new OptionWindowModel();
            _optionWindowView.Initialize();
        }
    }
}