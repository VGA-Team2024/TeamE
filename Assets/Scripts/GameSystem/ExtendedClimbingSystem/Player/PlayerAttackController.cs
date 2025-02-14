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
    private readonly string CueSheetName = "CueSheet_SE";
    private readonly string SwordPierceCueName = "SE_player_sword_pierce";
    private readonly string SwordPutCueName = "SE_player_sword_put";
    public bool CanStab { get; set; }
    public EnemyDamagePointHandler DamagePointHandler { get; set; }
    private void Awake()
    {
        _attackObject?.SetActive(false);
        _stabObject?.SetActive(false);
    }

    public void Attack(bool isClimb, Animator animator)
    {
        if (_variable.IsAttack) return;
        switch (isClimb, _canStab: CanStab)
        {
            case (true, true):
                PlayAnimation(animator, AttackType.WallStab, destroyCancellationToken).Forget();
                InsertSE(CueSheetName, SwordPierceCueName, _stabSETime, destroyCancellationToken).Forget();
                break;
            case (false, true):
                PlayAnimation(animator, AttackType.GroundStab, destroyCancellationToken).Forget();
                InsertSE(CueSheetName, SwordPierceCueName, _stabSETime, destroyCancellationToken).Forget();
                break;
            case (false, false):
                PlayAnimation(animator, AttackType.Slash, destroyCancellationToken).Forget();
                InsertSE(CueSheetName, SwordPutCueName, _slashSETime, destroyCancellationToken).Forget();
                break;
        }
    }

    async UniTaskVoid InsertSE(string sheet, string name,float time,  CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: ct);
        CRIAudioManager.SE.Play(sheet, name);
    }
    async UniTaskVoid PlayAnimation(Animator animator, AttackType type,  CancellationToken ct)
    {
        GameObject visualObject = null;
        int stateHash = 0;
        float waitTime = 0;
        switch (type)
        {
            case AttackType.Slash:
                visualObject = _attackObject;
                stateHash = AnimHashUtil.Slash;
                waitTime = _slashTime;
                break;
            case AttackType.WallStab:
                visualObject = _stabObject;
                stateHash = AnimHashUtil.WallStab;
                waitTime = _stabTime;
                break;
            case AttackType.GroundStab:
                visualObject = _stabObject;
                stateHash = AnimHashUtil.GroundStab;
                waitTime = _stabTime;
                break;
        }
        _variable.IsAttack = true;
        visualObject?.SetActive(true);
        animator.Play(stateHash);
        await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: ct);
        visualObject?.SetActive(false);
        _variable.IsAttack = false;
        if (type is AttackType.GroundStab or AttackType.WallStab)
        {
            CanStab = false;
            DamagePointHandler.Damage();
        }
    }

    public void InjectVariable(PlayerVariable variable)
    {
        _variable = variable;
    }

    private enum AttackType
    {
        Slash, WallStab, GroundStab
    }
}