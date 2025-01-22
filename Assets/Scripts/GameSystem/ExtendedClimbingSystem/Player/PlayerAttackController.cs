using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour, IHasPlayerVariable
{
    [SerializeField] private float _slashTime = 1.1f;
    [SerializeField] private float _stabTime = 2.2f;
    [SerializeField] private float _slashSETime = 0.434124f;
    [SerializeField] private float _stabSETime = 1.33753f;
    [SerializeField] private GameObject _attackObject;
    [SerializeField] private GameObject _stabObject;
    private PlayerVariable _variable;
    private void Awake()
    {
        _attackObject?.SetActive(false);
        _stabObject?.SetActive(false);
    }

    public void Attack(bool isClimb, Animator animator)
    {
        if (_variable.IsAttack) return;
        _variable.IsAttack = true;
        if (isClimb)
        {
            StabAnimation(animator, destroyCancellationToken).Forget();
            InsertSE("CueSheet_SE", "SE_player_sword_pierce", _stabSETime, destroyCancellationToken).Forget();
        }
        else
        {
            SlashAnimation(animator, destroyCancellationToken).Forget();
            InsertSE("CueSheet_SE", "SE_player_sword_put", _slashSETime, destroyCancellationToken).Forget();
        }
    }

    async UniTaskVoid InsertSE(string sheet, string name,float time,  CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct);
        CRIAudioManager.SE.Play(sheet, name);
    }
    async UniTaskVoid StabAnimation(Animator animator, CancellationToken ct)
    {
        _stabObject?.SetActive(true);
        animator.SetTrigger(AnimHashUtil.Stab);
        await UniTask.Delay(TimeSpan.FromSeconds(_stabTime), cancellationToken: ct);
        _stabObject?.SetActive(false);
        _variable.IsAttack = false;
    }

    async UniTaskVoid SlashAnimation(Animator animator, CancellationToken ct)
    {
        _attackObject?.SetActive(true);
        animator.SetTrigger(AnimHashUtil.Attack);
        await UniTask.Delay(TimeSpan.FromSeconds(_slashTime), cancellationToken: ct);
        _attackObject?.SetActive(false);
        _variable.IsAttack = false;
    }

    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }
}