namespace GameSystem.Title
{
    public class TitleModel
    {
        private TitleFadeSceneTransition _fadeSceneTransition;

        public TitleModel(TitleFadeSceneTransition fadeSceneTransition)
        {
            _fadeSceneTransition = fadeSceneTransition;
            
            CRIAudioManager.Initialize();
            CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_title"); 
        }

        public void StartButton()
        {
            _fadeSceneTransition.CollCoroutine();
            GameEventRecorder.GameStart();
        }

        public void OptionButton()
        {
        }

        public void ExitButton()
        {
            // ゲーム終了前のメッセージを表示する
            // ゲーム終了
            UnityEngine.Application.Quit();
        }
    }
}