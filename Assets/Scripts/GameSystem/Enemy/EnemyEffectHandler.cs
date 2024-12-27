using UnityEngine;

/// <summary>敵のエフェクトを制御するクラス</summary>
public class EnemyEffectHandler : MonoBehaviour
{
    [Header("後ろ蹴りのエフェクト"), SerializeField] 
    private GameObject backAttackEffect;

    [Header("後ろ蹴りのエフェクトを生成する位置（右）"), SerializeField]
    private Transform rightBackAttackEffectPos;
    
    [Header("後ろ蹴りのエフェクトを生成する位置（左）"), SerializeField]
    private Transform leftBackAttackEffectPos;

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
}
