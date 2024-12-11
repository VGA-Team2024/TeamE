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
}
