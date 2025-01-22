using UnityEngine;

public class ResultEnter : MonoBehaviour
{
    void Start()
    {
        GameEventRecorder.GameEnd(ReturnTitle);
    }

    void ReturnTitle()
    {
        SceneLoader.LoadScene("Title");
    }
}
