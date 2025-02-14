using UnityEngine;

/// <summary>敵のエフェクトを制御するクラス</summary>
public class EnemyEffectHandler : MonoBehaviour
{
    [SerializeField, InspectorVariantName("後ろ蹴りのエフェクト")] private GameObject backAttackEffect;
    [SerializeField, InspectorVariantName("後ろ蹴りのエフェクトを生成する位置（右）")] private Transform rightBackAttackEffectPos;
    [SerializeField, InspectorVariantName("後ろ蹴りのエフェクトを生成する位置（左）")] private Transform leftBackAttackEffectPos;
    [SerializeField, InspectorVariantName("前足のエフェクト")] private GameObject frontAttackEffect;
    [SerializeField, InspectorVariantName("前足のエフェクトを生成する位置（右）")] private Transform rightFrontAttackEffectPos;
    [SerializeField, InspectorVariantName("前足のエフェクトを生成する位置（左）")] private Transform leftFrontAttackEffectPos;
    private readonly string SECueSheetName = "CueSheet_SE";
    private readonly string VOICECueSheetName = "CueSheet_VOICE";
    private readonly string FootSoundCueName = "SE_boss_footsound";
    private readonly string DownCueName = "SE_boss_down";
    private readonly string Shout1CueName = "Voice_boss_shout1";
    private readonly string Shout2CueName = "Voice_boss_shout2";
    private readonly string Shout3CueName = "Voice_boss_shout3";
    
    public void OnDown()
    {
        CRIAudioManager.SE.Play(SECueSheetName, DownCueName, volume: 3f);
    }

    public void OnShout(ShoutType shout)
    {
        switch (shout)
        {
            case ShoutType.Shout1:
                CRIAudioManager.SE.Play(VOICECueSheetName, Shout1CueName, volume: 50f);
                break;
            case ShoutType.Shout2:
                CRIAudioManager.SE.Play(VOICECueSheetName, Shout2CueName, volume: 50f);
                break;
            case ShoutType.Shout3:
                CRIAudioManager.SE.Play(VOICECueSheetName, Shout3CueName, volume: 50f);
                break;
        }
    }
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
        CRIAudioManager.SE.Play3D(rightFrontAttackEffectPos.transform.position, SECueSheetName, FootSoundCueName);
        //Play3DSoundEffect(rightFrontAttackEffectPos.transform.position, CueSheetName, FootSoundCueName, 5f);
        //effect.transform.rotation = Quaternion.Euler(new Vector3(125f, 5f, 0f));
    }
    
    /// <summary>左前脚の攻撃エフェクトを生成する</summary>
    public void OnLeftFrontAttack()
    {
        var effect = Instantiate(frontAttackEffect, leftFrontAttackEffectPos.transform.position, 
            Quaternion.identity);
        CRIAudioManager.SE.Play3D(leftFrontAttackEffectPos.transform.position, SECueSheetName, FootSoundCueName);
        //Play3DSoundEffect(leftFrontAttackEffectPos.transform.position, CueSheetName, FootSoundCueName, 5f);
        //effect.transform.rotation = Quaternion.Euler(new Vector3(125f, 5f, 0f));
    }

    public enum ShoutType
    {
        Shout1, Shout2, Shout3
    }
}
