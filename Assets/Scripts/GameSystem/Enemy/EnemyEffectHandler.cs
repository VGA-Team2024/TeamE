using UnityEngine;

/// <summary>敵のエフェクトを制御するクラス</summary>
public class EnemyEffectHandler : MonoBehaviour
{
    [Header("後ろ蹴りのエフェクト"), SerializeField] private GameObject backAttackEffect;

    [Header("後ろ蹴りのエフェクトを生成する位置（右）"), SerializeField] 
    private Transform rightBackAttackEffectPos;
    
    [Header("後ろ蹴りのエフェクトを生成する位置（左）"), SerializeField] 
    private Transform leftBackAttackEffectPos;

    [Header("前足のエフェクトを"), SerializeField] private GameObject frontAttackEffect;

    [Header("前足のエフェクトを生成する位置（右）"), SerializeField]
    private Transform rightFrontAttackEffectPos;

    [Header("前足のエフェクトを生成する位置（左）"), SerializeField]
    private Transform leftFrontAttackEffectPos;

    /// <summary>右後脚の攻撃エフェクトを生成する</summary>
    public void OnRightBackAttack()
    {
        var effect = Instantiate(backAttackEffect, rightBackAttackEffectPos.transform.position, Quaternion.identity);
        effect.transform.rotation = Quaternion.Euler(new Vector3(125f, 5f, 0f));
    }

    /// <summary>左後脚の攻撃エフェクトを生成する</summary>
    public void OnLeftBackAttack()
    {
        var effect = Instantiate(backAttackEffect, leftBackAttackEffectPos.transform.position, Quaternion.identity);
        effect.transform.rotation = Quaternion.Euler(new Vector3(125f, 5f, 0f));
    }

    /// <summary>右前脚の攻撃エフェクトを生成する</summary>
    public void OnRightFrontAttack()
    {
        var effect = Instantiate(frontAttackEffect, rightFrontAttackEffectPos.transform.position, 
            Quaternion.identity);
        //effect.transform.rotation = Quaternion.Euler(new Vector3(125f, 5f, 0f));
    }
    
    /// <summary>左前脚の攻撃エフェクトを生成する</summary>
    public void OnLeftFrontAttack()
    {
        var effect = Instantiate(frontAttackEffect, leftFrontAttackEffectPos.transform.position, 
            Quaternion.identity);
        //effect.transform.rotation = Quaternion.Euler(new Vector3(125f, 5f, 0f));
    }
}
