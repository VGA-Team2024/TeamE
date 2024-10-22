using UnityEngine;

public class SingletonMonoBehavior< T > : MonoBehaviour where T : SingletonMonoBehavior<T>
{
    public static T Instance { get; private set; }
    protected virtual void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Debug.LogError($"シングルトンの{this.gameObject}が複数生成されました。");
            Destroy(this.gameObject);
        }
        Instance = this as T;
    }


    protected virtual void OnDestroy()
    {
        Debug.Log($"{this}を破棄しました");
        Instance = null;
    }

}
