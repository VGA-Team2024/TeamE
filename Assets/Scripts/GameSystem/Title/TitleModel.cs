
namespace GameSystem.Title
{
    public class TitleModel
    {
        public TitleModel()
        {
            CRIAudioManager.Initialize();
            CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_title"); 
        }

        public void StartButton()
        {
            TimelinePlayer.TimelineType = TimelineType.Opening;
            GameEventRecorder.GameStart();
            FadeSceneManager.Instance.FadeLoadScene("Timeline", 0.25f).Forget();
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