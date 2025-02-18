using UnityEngine;

public class TempleToInGameSceneTransition : MonoBehaviour
{
    [SerializeField] string _sceneName = "InGame";
    private void OnTriggerEnter(Collider other)
    {
        FadeSceneManager.Instance.FadeLoadScene(_sceneName, 0.25f).Forget();
    }
}
