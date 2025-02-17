using UnityEngine;

namespace GameSystem.Title
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleView _titleView;
        [SerializeField] private OptionPresenter _optionPresenter;
        private TitleModel _titleModel;

        private void Start()
        {
            _titleModel = new TitleModel();
            
            

            // ボタンイベントの登録
            _titleView.SetButtonListeners(
                _titleModel.StartButton,
                OpenOptionPanel,
                _titleModel.ExitButton
            );
        }

        /// <summary>
        /// オプションパネルを開く処理
        /// </summary>
        private void OpenOptionPanel()
        {
            if (_optionPresenter != null)
            {
                _optionPresenter.OpenOptionPanel();
            }
        }
    }
}