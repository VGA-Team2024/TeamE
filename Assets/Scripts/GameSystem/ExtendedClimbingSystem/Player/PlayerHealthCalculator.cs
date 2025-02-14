using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthCalculator : MonoBehaviour
{
    [SerializeField, InspectorVariantName("最大体力")] private float _maxHealth = 20f;
    [SerializeField, InspectorVariantName("自然回復量/s")] private float _recoveryHealthPerSecond = 0.5f;
    [SerializeField, InspectorVariantName("落下ダメージを受ける高さの閾値")] private float _fallHeightThreshold = 4f;
    [SerializeField, InspectorVariantName("落下ダメージの乗数")] private float _fallDamageMultiplier = 0.7f;
    [SerializeField] private PlayerHealthIndicator _playerHealthIndicator;
    private ReactiveProperty<float> _currentHealth = new();

    private void Awake()
    {
        _currentHealth.Value = _maxHealth;
        _currentHealth.Subscribe(health =>
        {
            if (health > _maxHealth)
            {
                _currentHealth.Value = _maxHealth;
                return;
            }

            if (health < 0f)
            {
                _currentHealth.Value = 0f;
                SceneManager.LoadScene("GameOver");
                return;
            }
            _playerHealthIndicator.SetRatio(health /_maxHealth);
        }).AddTo(this);
    }

    private void Update()
    {
        RecoveryHealth();
    }

    private void RecoveryHealth()
    {
        if (_currentHealth.Value >= _maxHealth || _currentHealth.Value <= 0) return;
        _currentHealth.Value += Time.deltaTime * _recoveryHealthPerSecond;
    }
    public void TakeDamage(float damage)
    {
        _currentHealth.Value -= damage;
    }
    /// <summary>
    /// 落下ダメージの計算
    /// </summary>
    /// <param name="maxHeight">滞空時の最高点</param>
    /// <param name="landingHeight">着地した高さ</param>
    /// <returns>ダメージを受けたかどうか</returns>
    public bool FallDamage(float maxHeight, float landingHeight)
    {
        if (maxHeight <= landingHeight) return false;
        var fallDistance = maxHeight - landingHeight;
        if (fallDistance <= _fallHeightThreshold) return false;
        TakeDamage(fallDistance * _fallDamageMultiplier);
        return true;
    }
}