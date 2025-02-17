using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public void LoadTitle()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        FadeSceneManager.Instance.FadeLoadScene("Title", 1f).Forget();
    }

    public void LoadInGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        FadeSceneManager.Instance.FadeLoadScene("InGame", 1f).Forget();
    }
    [ContextMenu("Load Release Ending")]
    public void LoadReleaseEnding()
    {
        TimelinePlayer.TimelineType = TimelineType.ReleaseEnding;
        FadeSceneManager.Instance.FadeLoadScene("Timeline", 1f).Forget();
    }
}
