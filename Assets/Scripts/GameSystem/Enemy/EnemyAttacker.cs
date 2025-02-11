using UnityEngine;

/// <summary>敵の攻撃を制御するクラス</summary>
public class EnemyAttacker : MonoBehaviour
{
    private BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    [Header("与えるダメージ"), SerializeField] private float damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーにダメージを与える処理
            Debug.Log($"{damage} ダメージ");
        }
    }

    public void TemporarilyActivateCollider()
    {
        _boxCollider.enabled = true;
        Invoke(nameof(DeactivateCollider), 0.1f);
    }

    private void DeactivateCollider()
    {
        _boxCollider.enabled = false;
    }
}