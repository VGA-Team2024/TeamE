using UnityEngine;

/// <summary>
/// 敵のデータを管理するクラス
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/CreateEnemyData")]
public class EnemyData : ScriptableObject
{
    //-------------------------------------------------------------------------------
    // 前足の攻撃範囲のデータ
    //-------------------------------------------------------------------------------
    
    [Header("前足の攻撃範囲の位置")] public Transform frontAttackPosition;
    [Header("前足の攻撃範囲の横幅")] public float frontAttackWidth;
    [Header("前足の攻撃範囲の高さ")] public float frontAttackHeight;
    [Header("前足の攻撃範囲の奥行")] public float frontAttackDepth;
    
    //-------------------------------------------------------------------------------
    // 胴体の攻撃範囲のデータ
    //-------------------------------------------------------------------------------
    
    [Header("胴体の攻撃範囲の位置")] public Transform centerAttackPosition;
    [Header("胴体の攻撃範囲の横幅")] public float centerAttackWidth;
    [Header("胴体の攻撃範囲の高さ")] public float centerAttackHeight;
    [Header("胴体の攻撃範囲の奥行")] public float centerAttackDepth;
    
    //-------------------------------------------------------------------------------
    // 後足の攻撃範囲のデータ
    //-------------------------------------------------------------------------------
    
    [Header("後足の攻撃範囲の位置")] public Transform backAttackPosition;
    [Header("後足の攻撃範囲の横幅")] public float backAttackWidth;
    [Header("後足の攻撃範囲の高さ")] public float backAttackHeight;
    [Header("後足の攻撃範囲の奥行")] public float backAttackDepth;
}
