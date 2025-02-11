using UnityEngine;

/// <summary>敵を制御するクラス</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// プレイヤーを制御するクラス
    /// </summary>
    [SerializeField] private Transform player;
    
    /// <summary>
    /// 敵のデータを管理するクラス
    /// </summary>
    [SerializeField] private EnemyData data;

    /// <summary>
    /// BehaviourTreeのRootノード
    /// </summary>
    private readonly EnemyNodes.SelectorNode _rootNode = new EnemyNodes.SelectorNode();
    
    [Header("右前脚の攻撃範囲の中心")] 
    [SerializeField] private Transform rightFrontAttackPosition;
    
    [Header("左前脚の攻撃範囲の中心")] 
    [SerializeField] private Transform leftFrontAttackPosition;
    
    [Header("胴体の攻撃範囲の中心")] 
    [SerializeField] private Transform centerAttackPosition;
    
    [Header("右後脚の攻撃範囲の位置")] 
    [SerializeField] private Transform rightBackAttackPosition;
    
    [Header("左後脚の攻撃範囲の位置")] 
    [SerializeField] private Transform leftBackAttackPosition;
    
    [Header("後脚の攻撃コライダー")]

    [SerializeField] private EnemyAttacker rightBackAttackCollider1;
    
    [SerializeField] private EnemyAttacker rightBackAttackCollider2;
    
    [SerializeField] private EnemyAttacker leftBackAttackCollider1;
    
    [SerializeField] private EnemyAttacker leftBackAttackCollider2;
    
    [Header("前脚の攻撃コライダー")]

    [SerializeField] private EnemyAttacker rightFrontAttackCollider1;
    
    [SerializeField] private EnemyAttacker rightFrontAttackCollider2;

    [SerializeField] private EnemyAttacker leftFrontAttackCollider1;
    
    [SerializeField] private EnemyAttacker leftFrontAttackCollider2;

    [Header("胴体の攻撃コライダー")] 
    
    [SerializeField] private EnemyAttacker centerAttackCollider;

    private bool _isDown;
    private bool _isAttacking;
    private bool _isActive;
    
    Animator _animator;

    public void Activate()
    {
        if (_isActive) return;
        _animator.enabled = true;
        _isActive = true;
    }
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
        _animator.enabled = false;
    }

    /// <summary>
    /// BehaviourTreeの構築を行うメソッド
    /// </summary>
    private void SetUpBehaviourTree()
    {
        _rootNode.Add(SetUpRightFrontAttackSequence());
        _rootNode.Add(SetUpLeftFrontAttackSequence());
        _rootNode.Add(SetUpCenterAttackSequence());
        _rootNode.Add(SetUpRightBackAttackSequence());
        _rootNode.Add(SetUpLeftBackAttackSequence());
        _rootNode.Add(SetUpRotateSequence());
        _rootNode.Add(SetUpChaseSequence());
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
        if (ShouldRightFrontAttackPlayer() || ShouldLeftFrontAttackPlayer() || ShouldCenterAttackPlayer() || 
            ShouldRightBackAttackPlayer() || ShouldLeftBackAttackPlayer())
        {
            return false;
        }

        return true;
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
        _animator.SetTrigger(data.rightFrontAttackTrigger);
        _isAttacking = true;
        return EnemyNodes.NodeStatus.Success;
    }
    
    /// <summary>
    /// 右前脚の攻撃コライダーを一時的に有効化する
    /// </summary>
    public void TemporarilyActivateRightFrontAttackCollider()
    {
        rightFrontAttackCollider1.TemporarilyActivateCollider();
        rightFrontAttackCollider2.TemporarilyActivateCollider();
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
        _animator.SetTrigger(data.leftFrontAttackTrigger);
        _isAttacking = true;
        return EnemyNodes.NodeStatus.Success;
    }
    
    /// <summary>
    /// 左前脚の攻撃コライダーを一時的に有効化する
    /// </summary>
    public void TemporarilyActivateLeftFrontAttackCollider()
    {
        leftFrontAttackCollider1.TemporarilyActivateCollider();
        leftFrontAttackCollider2.TemporarilyActivateCollider();
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
        _animator.SetTrigger(data.centerAttackTrigger);
        _isAttacking = true;
        return EnemyNodes.NodeStatus.Success;
    }
    
    /// <summary>
    /// 胴体の攻撃コライダーを一時的に有効化する
    /// </summary>
    public void TemporarilyActivateCenterAttackCollider()
    {
        centerAttackCollider.TemporarilyActivateCollider();
    }
    
    //-------------------------------------------------------------------------------
    // 後脚の攻撃シーケンス
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 右後脚の攻撃シーケンスを構築する
    /// </summary>
    private EnemyNodes.BaseNode SetUpRightBackAttackSequence()
    {
        var rightBackAttackSeq = new EnemyNodes.SequenceNode();
        rightBackAttackSeq.Add(new EnemyNodes.ConditionNode(ShouldRightBackAttackPlayer));
        rightBackAttackSeq.Add(new EnemyNodes.ActionNode(RightBackAttack));
        return rightBackAttackSeq;
    }

    /// <summary>
    /// 左後脚の攻撃シーケンスを構築する
    /// </summary>
    private EnemyNodes.BaseNode SetUpLeftBackAttackSequence()
    {
        var leftBackAttackSeq = new EnemyNodes.SequenceNode();
        leftBackAttackSeq.Add(new EnemyNodes.ConditionNode(ShouldLeftBackAttackPlayer));
        leftBackAttackSeq.Add(new EnemyNodes.ActionNode(LeftBackAttack));
        return leftBackAttackSeq;
    }
    
    //-------------------------------------------------------------------------------
    // 後脚の攻撃シーケンスの処理
    //-------------------------------------------------------------------------------
    
    /// <summary>
    /// プレイヤーを右後脚で攻撃すべきか（≒プレイヤーが後足の攻撃範囲内にいるか）
    /// </summary>
    private bool ShouldRightBackAttackPlayer()
    {
        var colliders = Physics.OverlapBox(rightBackAttackPosition.position, 
            new Vector3(data.backAttackWidth/4, data.backAttackHeight/2, data.backAttackDepth/2));
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// プレイヤーを左後脚で攻撃すべきか（≒プレイヤーが後足の攻撃範囲内にいるか）
    /// </summary>
    private bool ShouldLeftBackAttackPlayer()
    {
        var colliders = Physics.OverlapBox(leftBackAttackPosition.position, 
            new Vector3(data.backAttackWidth/4, data.backAttackHeight/2, data.backAttackDepth/2));
        
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player")) return true;
        }
        return false;
    }
    
    /// <summary>
    /// プレイヤーを右後脚で攻撃する
    /// </summary>
    private EnemyNodes.NodeStatus RightBackAttack()
    {
        _animator.SetTrigger(data.rightBackAttackTrigger);
        _isAttacking = true;
        return EnemyNodes.NodeStatus.Success;
    }
    
    /// <summary>
    /// プレイヤーを左後脚で攻撃する
    /// </summary>
    private EnemyNodes.NodeStatus LeftBackAttack()
    {
        _animator.SetTrigger(data.leftBackAttackTrigger);
        _isAttacking = true;
        return EnemyNodes.NodeStatus.Success;
    }

    /// <summary>
    /// 右後脚の攻撃コライダーを一時的に有効化する
    /// </summary>
    public void TemporarilyActivateRightBackAttackCollider()
    {
        rightBackAttackCollider1.TemporarilyActivateCollider();
        rightBackAttackCollider2.TemporarilyActivateCollider();
    }

    /// <summary>
    /// 左後脚の攻撃コライダーを一時的に有効化する
    /// </summary>
    public void TemporarilyActivateLeftBackAttackCollider()
    {
        leftBackAttackCollider1.TemporarilyActivateCollider();
        leftBackAttackCollider2.TemporarilyActivateCollider();
    }

    /// <summary>
    /// 攻撃フラグをオフにする（遅延あり）
    /// </summary>
    public void ResetAttackFlagWithDelay()
    {
        Invoke(nameof(ResetAttackFlag), data.attackWaitTime);
    }

    /// <summary>
    /// 攻撃フラグをオフにする
    /// </summary>
    public void ResetAttackFlag()
    {
        _isAttacking = false;
    }
    
    //-------------------------------------------------------------------------------
    // 弱点2に関連する処理
    //-------------------------------------------------------------------------------

    /// <summary>
    /// 右方向にダウンする
    /// </summary>
    public void GetDownRight()
    {
        _isDown = true;
        _animator.SetBool(data.rightDownFlag, true);
        Invoke(nameof(RecoverDownRight), data.recoveryTime);
    }

    /// <summary>
    /// 右方向のダウンから回復する
    /// </summary>
    public void RecoverDownRight()
    {
        _isDown = false;
        _animator.SetBool(data.rightDownFlag, false);
    }

    /// <summary>
    /// 左方向にダウンする
    /// </summary>
    public void GetDownLeft()
    {
        _isDown = true;
        _animator.SetBool(data.leftDownFlag, true);
        Invoke(nameof(RecoverDownLeft), data.recoveryTime);
    }

    /// <summary>
    /// 左方向のダウンから回復する
    /// </summary>
    public void RecoverDownLeft()
    {
        _isDown = false;
        _animator.SetBool(data.leftDownFlag, false);
    }
    
    //-------------------------------------------------------------------------------
    // 更新処理
    //-------------------------------------------------------------------------------

    private void Update()
    {
        if (!_isDown && !_isAttacking && _isActive) _rootNode.Execute();
    }
    
    //-------------------------------------------------------------------------------
    // Gizmo
    //-------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        // 右前脚の攻撃範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(rightFrontAttackPosition.position, 
            new Vector3(data.frontAttackWidth/2, data.frontAttackHeight, data.frontAttackDepth));
        
        // 左前脚の攻撃範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(leftFrontAttackPosition.position, 
            new Vector3(data.frontAttackWidth/2, data.frontAttackHeight, data.frontAttackDepth));
        
        // 胴体の攻撃範囲
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerAttackPosition.position, 
            new Vector3(data.centerAttackWidth, data.centerAttackHeight, data.centerAttackDepth));
        
        // 右後脚の攻撃範囲
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(rightBackAttackPosition.position, 
            new Vector3(data.backAttackWidth/2, data.backAttackHeight, data.backAttackDepth));
        
        // 左後脚の攻撃範囲
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(leftBackAttackPosition.position, 
            new Vector3(data.backAttackWidth/2, data.backAttackHeight, data.backAttackDepth));
    }
}