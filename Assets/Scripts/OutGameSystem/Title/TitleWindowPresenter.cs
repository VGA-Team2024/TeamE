using UnityEngine;

namespace OutGameSystem.Title
{
    public class TitleWindowPresenter : MonoBehaviour
    {
        [SerializeField] private TitleWindowView _titleWindowView;
        private TitleWindowModel _model;

        void Start()
        {
            _model = new TitleWindowModel();
            _titleWindowView.Initialize();
        }
    }
}