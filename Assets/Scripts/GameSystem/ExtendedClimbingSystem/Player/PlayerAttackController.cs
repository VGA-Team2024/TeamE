using System;
using R3;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int StabHash = Animator.StringToHash("Stab");
    [SerializeField] private float _attackTime = 1.1f;
    [SerializeField] private float _stabTime = 2.2f;
    [SerializeField] private GameObject _attackObject;
    [SerializeField] private GameObject _stabObject;
    private bool _isAttack;
    public bool IsAttack => _isAttack;

    private void Awake()
    {
        _attackObject?.SetActive(false);
        _stabObject?.SetActive(false);
    }

    public void Attack(bool isClimb, Animator animator)
    {
        if (_isAttack) return;
        _isAttack = true;
        if (isClimb)
        {
            _stabObject?.SetActive(true);
            animator.SetTrigger(StabHash);
            Observable.Timer(TimeSpan.FromSeconds(_stabTime)).Subscribe(_=>
            {
                _stabObject?.SetActive(false);
                _isAttack = false;
            }).AddTo(this);
        }
        else
        {
            _attackObject?.SetActive(true);
            animator.SetTrigger(AttackHash);
            Observable.Timer(TimeSpan.FromSeconds(_attackTime)).Subscribe(_=>
            {
                _attackObject?.SetActive(false);
                _isAttack = false;
            }).AddTo(this);
        }
    }
}