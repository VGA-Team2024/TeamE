using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneTransition : MonoBehaviour
{
    [SerializeField] string _inGameSceneName;

    public void SceneChange()
    {
        SceneManager.LoadScene(_inGameSceneName);
    }
}
