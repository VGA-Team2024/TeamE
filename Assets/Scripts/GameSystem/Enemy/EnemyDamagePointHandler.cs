using System;
using UnityEngine;

public class EnemyDamagePointHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem _damageEffect;
    [SerializeField] private ParticleSystem _pointEffect;
    [SerializeField, InspectorVariantName("最初からロック状態にするか")] private bool _initializeLock;
    private bool _locked = false;
    public bool Locked
    {
        get => _locked;
        set
        {
            _locked = value;
            if (!_pointEffect) return;
            if (value)
            {
                _pointEffect.gameObject.SetActive(false);
            }
            else
            {
                _pointEffect.gameObject.SetActive(true);
            }
        }
    }
    public Action OnDamage;

    private void Awake()
    {
        if (_initializeLock)
        {
            Locked = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_locked) return;
        if (other.TryGetComponent(out PlayerAttackController attackController))
        {
            attackController.CanStab = true;
            attackController.DamagePointHandler = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_locked) return;
        if (other.TryGetComponent(out PlayerAttackController attackController))
        {
            attackController.CanStab = false;
        }
    }
    public void Damage()
    {
        Locked = true;
        _damageEffect?.Play();
        OnDamage?.Invoke();
    }
}