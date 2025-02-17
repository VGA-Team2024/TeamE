using UnityEngine;

public class SingletonMonoBehavior<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }
    void Awake()
    {
        if (Instance && Instance != this)
        {
            Debug.LogWarning($"シングルトンの{this.gameObject}が複数生成されました。");
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
        OnAwake();
    }
    protected virtual void OnAwake(){}
}
