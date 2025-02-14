using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 敵のデータを管理するクラス
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/CreateEnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("放浪し始めるプレイヤーとの距離")] public float wanderDistance = 20f;

    [Header("放浪する最大距離")] public float wanderMaxDistance = 20f;
    //-------------------------------------------------------------------------------
    // 前足の攻撃範囲のデータ
    //-------------------------------------------------------------------------------
    
    [Header("前足の攻撃範囲の横幅")] public float frontAttackWidth = 20f;
    [Header("前足の攻撃範囲の高さ")] public float frontAttackHeight = 6f;
    [Header("前足の攻撃範囲の奥行")] public float frontAttackDepth = 10f;
    
    //-------------------------------------------------------------------------------
    // 胴体の攻撃範囲のデータ
    //-------------------------------------------------------------------------------
    
    [Header("胴体の攻撃範囲の横幅")] public float centerAttackWidth = 20f;
    [Header("胴体の攻撃範囲の高さ")] public float centerAttackHeight = 6f;
    [Header("胴体の攻撃範囲の奥行")] public float centerAttackDepth = 15f;
    
    //-------------------------------------------------------------------------------
    // 後足の攻撃範囲のデータ
    //-------------------------------------------------------------------------------
    
    [Header("後足の攻撃範囲の横幅")] public float backAttackWidth = 20f;
    [Header("後足の攻撃範囲の高さ")] public float backAttackHeight = 6f;
    [Header("後足の攻撃範囲の奥行")] public float backAttackDepth = 15f;
    
    //-------------------------------------------------------------------------------
    // 回転シーケンスのデータ
    //-------------------------------------------------------------------------------

    [Header("回転速度")] public float rotateSpeed = 1.25f;
    [Header("回転の閾値")] public float rotateThreshold = 7.5f;
    
    //-------------------------------------------------------------------------------
    // 回転シーケンスのデータ
    //-------------------------------------------------------------------------------

    [Header("移動速度")] public float moveSpeed = 5.0f;
    
    //-------------------------------------------------------------------------------
    // 攻撃に関するデータ
    //-------------------------------------------------------------------------------

    [Header("攻撃後の待機時間")] public float attackWaitTime = 1.0f;
    
    //-------------------------------------------------------------------------------
    // 弱点に関するデータ
    //-------------------------------------------------------------------------------

    [Header("ダウン後回復時間")] public float recoveryTime = 30f;
    
    //-------------------------------------------------------------------------------
    // アニメーションのデータ
    //-------------------------------------------------------------------------------

    public string rightFrontAttackTrigger = "RightFrontAttack";
    public string leftFrontAttackTrigger = "LeftFrontAttack";
    public string centerAttackTrigger = "CenterAttack";
    public string rightBackAttackTrigger = "RightBackAttack";
    public string leftBackAttackTrigger = "LeftBackAttack";
    public string rightDownFlag = "IsDownRight";
    public string leftDownFlag = "IsDownLeft";
}
