using UnityEngine;

/// <summary>敵を制御するクラス</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// プレイヤーを制御するクラス
    /// </summary>
    [SerializeField] private PlayerDummy player;
    
    /// <summary>
    /// 敵のデータを管理するクラス
    /// </summary>
    [SerializeField] private EnemyData data;

    /// <summary>
    /// BehaviourTreeのRootノード
    /// </summary>
    private readonly EnemyNodes.SelectorNode _rootNode = new EnemyNodes.SelectorNode();
    
    [Header("右前足の攻撃範囲の中心")] 
    [SerializeField] private Transform rightFrontAttackPosition;
    
    [Header("左前足の攻撃範囲の中心")] 
    [SerializeField] private Transform leftFrontAttackPosition;
    
    [Header("胴体の攻撃範囲の中心")] 
    [SerializeField] private Transform centerAttackPosition;
    
    [Header("後足の攻撃範囲の位置")] 
    [SerializeField] private Transform backAttackPosition;
    
    Animator _animator;
    
    //-------------------------------------------------------------------------------
    // 初期化
    //-------------------------------------------------------------------------------

    private void Start()
    {
        // コンポーネントを取得する
        GetAllNecessaryComponents();
        
        // BehaviourTreeを構築する
        SetUpBehaviourTree();
    }
    
    //-------------------------------------------------------------------------------
    // 初期化に関連する処理
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 必要なコンポーネントを全て取得するメソッド
    /// </summary>
    private void GetAllNecessaryComponents()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// BehaviourTreeの構築を行うメソッド
    /// </summary>
    private void SetUpBehaviourTree()
    {
        _rootNode.Add(SetUpRotateSequence());
        _rootNode.Add(SetUpChaseSequence());
        _rootNode.Add(SetUpRightFrontAttackSequence());
        _rootNode.Add(SetUpLeftFrontAttackSequence());
        _rootNode.Add(SetUpCenterAttackSequence());
        _rootNode.Add(SetUpBackAttackSequence());
    }
    
    //-------------------------------------------------------------------------------
    // 回転シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 回転シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpRotateSequence()
    {
        var rotateSeq = new EnemyNodes.SequenceNode();
        rotateSeq.Add(new EnemyNodes.ConditionNode(ShouldRotateTowardsPlayer));
        rotateSeq.Add(new EnemyNodes.ActionNode(Rotate));
        return rotateSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 回転シーケンスに関連する処理
    //-------------------------------------------------------------------------------

    /// <summary>
    /// Y座標を無視したプレイヤーへの方向を取得する
    /// </summary>
    private Vector3 GetFixedDirectionToPlayer()
    {
        var playerPos = player.transform.position;
        var enemyPos = transform.position;
        var directionToPlayer = playerPos - enemyPos;
        directionToPlayer.y = 0;
        return directionToPlayer.normalized;
    }
    
    /// <summary>
    /// Y座標を無視したプレイヤーへの角度を取得する
    /// </summary>
    /// <returns></returns>
    private float GetFixedAngleToPlayer()
    {
        return Vector3.Angle(transform.forward, GetFixedDirectionToPlayer());
    }
    
    /// <summary>
    /// プレイヤーへ回転するべきか（≒プレイヤーが正面方向にいるか）
    /// </summary>
    private bool ShouldRotateTowardsPlayer()
    {
        return GetFixedAngleToPlayer() > data.rotateThreshold;
    }
    
    /// <summary>
    /// プレイヤーへ回転する
    /// </summary>
    private EnemyNodes.NodeStatus Rotate()
    {
        var target = Quaternion.LookRotation(GetFixedDirectionToPlayer());
        transform.rotation = Quaternion.Slerp(transform.rotation, target, data.rotateSpeed * Time.deltaTime);
        return EnemyNodes.NodeStatus.Running;
    }
    
    //-------------------------------------------------------------------------------
    // 追跡シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 追跡シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpChaseSequence()
    {
        var chaseSeq = new EnemyNodes.SequenceNode();
        chaseSeq.Add(new EnemyNodes.ConditionNode(ShouldChasePlayer));
        chaseSeq.Add(new EnemyNodes.ActionNode(Chase));
        return chaseSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 追跡シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    /// <summary>
    /// プレイヤーを追跡するべきか（≒プレイヤーが全ての攻撃範囲外にいるか）
    /// </summary>
    private bool ShouldChasePlayer()
    {
        if (ShouldRightFrontAttackPlayer() || ShouldCenterAttackPlayer() || ShouldBackAttackPlayer())
        {
            _animator.SetBool("IsMoving", false);
            return false;
        }
        else
        {
            _animator.SetBool("IsMoving", true);
            return true;
        }
    }
    
    /// <summary>
    /// プレイヤーを追跡する
    /// </summary>
    private EnemyNodes.NodeStatus Chase()
    {
        var target = Quaternion.LookRotation(GetFixedDirectionToPlayer());
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, data.rotateSpeed * Time.deltaTime);
        transform.Translate(Time.deltaTime * data.moveSpeed * GetFixedDirectionToPlayer(), Space.World);
        return EnemyNodes.NodeStatus.Running;
    }
    
    //-------------------------------------------------------------------------------
    // 右前足の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 右前足の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpRightFrontAttackSequence()
    {
        var frontAttackSeq = new EnemyNodes.SequenceNode();
        frontAttackSeq.Add(new EnemyNodes.ConditionNode(ShouldRightFrontAttackPlayer));
        frontAttackSeq.Add(new EnemyNodes.ActionNode(RightFrontAttack));
        return frontAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 右前足の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    /// <summary>
    /// プレイヤーを右前足で攻撃するべきか（≒プレイヤーが右前足の攻撃範囲内にいるか）
    /// </summary>
    private bool ShouldRightFrontAttackPlayer()
    {
        var colliders = Physics.OverlapBox(rightFrontAttackPosition.position, 
            new Vector3(data.frontAttackWidth/4, data.frontAttackHeight/2, data.frontAttackDepth/2));
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// プレイヤーを右前足で攻撃する
    /// </summary>
    private EnemyNodes.NodeStatus RightFrontAttack()
    {
        _animator.SetTrigger(data.frontRightAttackTrigger);
        return EnemyNodes.NodeStatus.Success;
    }
    
    //-------------------------------------------------------------------------------
    // 左前足の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 左前足の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpLeftFrontAttackSequence()
    {
        var frontAttackSeq = new EnemyNodes.SequenceNode();
        frontAttackSeq.Add(new EnemyNodes.ConditionNode(ShouldLeftFrontAttackPlayer));
        frontAttackSeq.Add(new EnemyNodes.ActionNode(LeftFrontAttack));
        return frontAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 左前足の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    /// <summary>
    /// プレイヤーを左前足で攻撃するべきか（≒プレイヤーが左前足の攻撃範囲内にいるか）
    /// </summary>
    private bool ShouldLeftFrontAttackPlayer()
    {
        var colliders = Physics.OverlapBox(leftFrontAttackPosition.position, 
            new Vector3(data.frontAttackWidth/4, data.frontAttackHeight/2, data.frontAttackDepth/2));
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// プレイヤーを左前足で攻撃する
    /// </summary>
    private EnemyNodes.NodeStatus LeftFrontAttack()
    {
        _animator.SetTrigger(data.frontLeftAttackTrigger);
        return EnemyNodes.NodeStatus.Success;
    }
    
    //-------------------------------------------------------------------------------
    // 胴体の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 胴体の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpCenterAttackSequence()
    {
        var centerAttackSeq = new EnemyNodes.SequenceNode();
        centerAttackSeq.Add(new EnemyNodes.ConditionNode(ShouldCenterAttackPlayer));
        centerAttackSeq.Add(new EnemyNodes.ActionNode(CenterAttack));
        return centerAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 胴体の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    /// <summary>
    /// プレイヤーを胴体で攻撃すべきか（≒プレイヤーが胴体の攻撃範囲内にいるか）
    /// </summary>
    private bool ShouldCenterAttackPlayer()
    {
        var colliders = Physics.OverlapBox(centerAttackPosition.position, 
            new Vector3(data.centerAttackWidth/2, data.centerAttackHeight/2, data.centerAttackDepth/2));
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// プレイヤーを胴体で攻撃する
    /// </summary>
    private EnemyNodes.NodeStatus CenterAttack()
    {
        return EnemyNodes.NodeStatus.Success;
    }
    
    //-------------------------------------------------------------------------------
    // 後足の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 後足の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpBackAttackSequence()
    {
        var backAttackSeq = new EnemyNodes.SequenceNode();
        backAttackSeq.Add(new EnemyNodes.ConditionNode(ShouldBackAttackPlayer));
        backAttackSeq.Add(new EnemyNodes.ActionNode(BackAttack));
        return backAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 後足の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    /// <summary>
    /// プレイヤーを後足で攻撃すべきか（≒プレイヤーが後足の攻撃範囲内にいるか）
    /// </summary>
    private bool ShouldBackAttackPlayer()
    {
        var colliders = Physics.OverlapBox(backAttackPosition.position, 
            new Vector3(data.backAttackWidth/2, data.backAttackHeight/2, data.backAttackDepth/2));
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// プレイヤーを後足で攻撃する
    /// </summary>
    private EnemyNodes.NodeStatus BackAttack()
    {
        _animator.SetTrigger(data.backAttackTrigger);
        return EnemyNodes.NodeStatus.Success;
    }
    
    //-------------------------------------------------------------------------------
    // 更新処理
    //-------------------------------------------------------------------------------

    private void Update()
    {
        _rootNode.Execute();
    }
    
    //-------------------------------------------------------------------------------
    // Gizmo
    //-------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        // 右前足の攻撃範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rightFrontAttackPosition.position, 
            new Vector3(data.frontAttackWidth/2, data.frontAttackHeight, data.frontAttackDepth));
        
        // 左前足の攻撃範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(leftFrontAttackPosition.position, 
            new Vector3(data.frontAttackWidth/2, data.frontAttackHeight, data.frontAttackDepth));
        
        // 胴体の攻撃範囲
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerAttackPosition.position, 
            new Vector3(data.centerAttackWidth, data.centerAttackHeight, data.centerAttackDepth));
        
        // 後足の攻撃範囲
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(backAttackPosition.position, 
            new Vector3(data.backAttackWidth, data.backAttackHeight, data.backAttackDepth));
    }
}