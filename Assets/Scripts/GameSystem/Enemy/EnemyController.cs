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
    private readonly EnemyNodes.SelectorNode _rootNode;
    
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
        SetUpRotateSequence();
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
        rotateSeq.Add(new EnemyNodes.ConditionNode(IsPlayerNotInFront));
        rotateSeq.Add(new EnemyNodes.ActionNode(Rotate));
        return rotateSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 回転シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // 追跡シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 追跡シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpChaseSequence()
    {
        var chaseSeq = new EnemyNodes.SequenceNode();
        chaseSeq.Add(new EnemyNodes.ConditionNode(IsPlayerAway));
        chaseSeq.Add(new EnemyNodes.ActionNode(Chase));
        return chaseSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 追跡シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // 前足の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 前足の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpFrontAttackSequence()
    {
        var frontAttackSeq = new EnemyNodes.SequenceNode();
        frontAttackSeq.Add(new EnemyNodes.ConditionNode(CanFrontAttack));
        frontAttackSeq.Add(new EnemyNodes.ActionNode(FrontAttack));
        return frontAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 前足の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // 胴体の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 胴体の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpCenterAttackSequence()
    {
        var centerAttackSeq = new EnemyNodes.SequenceNode();
        centerAttackSeq.Add(new EnemyNodes.ConditionNode(CanCenterAttack));
        centerAttackSeq.Add(new EnemyNodes.ActionNode(CenterAttack));
        return centerAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 胴体の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // 後足の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 後足の攻撃シーケンスを構築するメソッド
    /// </summary>
    private EnemyNodes.BaseNode SetUpBackAttackSequence()
    {
        var backAttackSeq = new EnemyNodes.SequenceNode();
        backAttackSeq.Add(new EnemyNodes.ConditionNode(CanBackAttack));
        backAttackSeq.Add(new EnemyNodes.ActionNode(BackAttack));
        return backAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 後足の攻撃シーケンスに関連する処理
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // ルートノード
    //-------------------------------------------------------------------------------

    /// <summary>
    /// ルートノードを構築するメソッド
    /// </summary>
    private void SetUpRootNode()
    {
        _rootNode.Add(SetUpRotateSequence());
        _rootNode.Add(SetUpChaseSequence());
        _rootNode.Add(SetUpFrontAttackSequence());
        _rootNode.Add(SetUpCenterAttackSequence());
        _rootNode.Add(SetUpBackAttackSequence());
    }
    
    //-------------------------------------------------------------------------------
    // 更新処理
    //-------------------------------------------------------------------------------

    private void Update()
    {
        _rootNode.Execute();
    }
    
    private bool IsPlayerAway()
    {
        if (CanFrontAttack() || CanCenterAttack() || CanBackAttack())
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

    private bool CanFrontAttack()
    {
        var colliders = Physics.OverlapBox(frontAttackPosition.position, new Vector3(10f, 3f, frontAttackRange/2));
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }

    private bool CanCenterAttack()
    {
        var colliders = Physics.OverlapBox(centerAttackPosition.position, new Vector3(10f, 3f, centerAttackRange/2));
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }

    private bool CanBackAttack()
    {
        var colliders = Physics.OverlapBox(backAttackPosition.position, new Vector3(10f, 3f, backAttackRange/2));
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }

    private bool IsPlayerNotInFront()
    {
        return GetPlayerAngle() > rotateThreshold;
    }

    private float GetPlayerAngle()
    {
        var playerPos = player.transform.position;
        var enemyPos = transform.position;
        var dir = playerPos - enemyPos;
        dir.y = 0;
        dir.Normalize();
        var forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        return Vector3.Angle(forward, dir);
    }

    private EnemyNodes.NodeStatus Rotate()
    {
        var playerPos = player.transform.position;
        var enemyPos = transform.position;
        var dir = playerPos - enemyPos;
        dir.y = 0;
        dir.Normalize();
        var target = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotateSpeed * Time.deltaTime);
        return EnemyNodes.NodeStatus.Running;
    }

    private EnemyNodes.NodeStatus Chase()
    {
        var playerPos = player.transform.position;
        var enemyPos = transform.position;
        var dir = playerPos - enemyPos;
        dir.y = 0;
        dir.Normalize();
        var target = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * Time.deltaTime);
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        return EnemyNodes.NodeStatus.Running;
    }

    private EnemyNodes.NodeStatus FrontAttack()
    {
        Debug.Log("Front Attack.");
        return EnemyNodes.NodeStatus.Success;
    }

    private EnemyNodes.NodeStatus CenterAttack()
    {
        Debug.Log("Center Attack.");
        return EnemyNodes.NodeStatus.Success;
    }

    private EnemyNodes.NodeStatus BackAttack()
    {
        Debug.Log("Back Attack.");
        return EnemyNodes.NodeStatus.Success;
    }
    
    //-------------------------------------------------------------------------------
    // Gizmo
    //-------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        // 前足の攻撃範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(data.frontAttackPosition.position, 
            new Vector3(data.frontAttackWidth, data.frontAttackHeight, data.frontAttackDepth));
        
        // 胴体の攻撃範囲
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(data.centerAttackPosition.position, 
            new Vector3(data.centerAttackWidth, data.centerAttackHeight, data.centerAttackDepth));
        
        // 後足の攻撃範囲
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(data.backAttackPosition.position, 
            new Vector3(data.backAttackWidth, data.backAttackHeight, data.backAttackDepth));
    }
}