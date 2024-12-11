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
        // プレイヤーの初期設定を行う
        Initialize();
        
        var rotateAction = new EnemyNodes.ActionNode(Rotate);
        var rotateCondition = new EnemyNodes.ConditionNode(IsPlayerNotInFront);
        var rotateSeq = new EnemyNodes.SequenceNode();
        rotateSeq.Add(rotateCondition);
        rotateSeq.Add(rotateAction);
        
        var chaseAction = new EnemyNodes.ActionNode(Chase);
        var chaseCondition = new EnemyNodes.ConditionNode(IsPlayerAway);
        var chaseSeq = new EnemyNodes.SequenceNode();
        chaseSeq.Add(chaseCondition);
        chaseSeq.Add(chaseAction);

        var frontAttackAciton = new EnemyNodes.ActionNode(FrontAttack);
        var frontAttackCondition = new EnemyNodes.ConditionNode(CanFrontAttack);
        var frontAttackSeq = new EnemyNodes.SequenceNode();
        frontAttackSeq.Add(frontAttackCondition);
        frontAttackSeq.Add(frontAttackAciton);

        var centerAttackAction = new EnemyNodes.ActionNode(CenterAttack);
        var centerAttackCondition = new EnemyNodes.ConditionNode(CanCenterAttack);
        var centerAttackSeq = new EnemyNodes.SequenceNode();
        centerAttackSeq.Add(centerAttackCondition);
        centerAttackSeq.Add(centerAttackAction);

        var backAttackAction = new EnemyNodes.ActionNode(BackAttack);
        var backAttackCondition = new EnemyNodes.ConditionNode(CanBackAttack);
        var backAttackSeq = new EnemyNodes.SequenceNode();
        backAttackSeq.Add(backAttackCondition);
        backAttackSeq.Add(backAttackAction);
        
        _rootNode.Add(rotateSeq);
        _rootNode.Add(chaseSeq);
        _rootNode.Add(frontAttackSeq);
        _rootNode.Add(centerAttackSeq);
        _rootNode.Add(backAttackSeq);
    }

    /// <summary>
    /// 敵の初期化処理を行うメソッド
    /// </summary>
    private void Initialize()
    {
        // コンポーネントを取得する
        GetAllNecessaryComponents();
        
        // BehaviourTreeを構築する
        SetUpBehaviourTree();
    }

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
        
    }
    
    //-------------------------------------------------------------------------------
    // 更新処理
    //-------------------------------------------------------------------------------

    private void Update()
    {
        _rootNode.Execute();
    }
    
    //-------------------------------------------------------------------------------
    // 
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // 
    //-------------------------------------------------------------------------------
    
    //-------------------------------------------------------------------------------
    // 
    //-------------------------------------------------------------------------------
    
    

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