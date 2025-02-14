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
        if (other.TryGetComponent(out PlayerHealthCalculator playerHealth))
        {
            // プレイヤーにダメージを与える処理
            playerHealth.TakeDamage(damage);
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