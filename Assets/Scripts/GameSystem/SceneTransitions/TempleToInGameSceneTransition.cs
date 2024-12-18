using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempleToInGameSceneTransition : MonoBehaviour
{
    [SerializeField] string _sceneName = "InGame";
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene(_sceneName);
    }
}
