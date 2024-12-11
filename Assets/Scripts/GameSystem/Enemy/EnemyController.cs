using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private PlayerDummy player;
    [SerializeField] private Transform frontAttackPosition;
    [SerializeField] private Transform centerAttackPosition;
    [SerializeField] private Transform backAttackPosition;
    [SerializeField] private float frontAttackRange;
    [SerializeField] private float centerAttackRange;
    [SerializeField] private float backAttackRange;
    [SerializeField] private float rotateThreshold;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float moveSpeed;

    readonly EnemyNodes.SelectorNode _selector = new EnemyNodes.SelectorNode();
    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
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
        
        _selector.Add(rotateSeq);
        _selector.Add(chaseSeq);
        _selector.Add(frontAttackSeq);
        _selector.Add(centerAttackSeq);
        _selector.Add(backAttackSeq);
    }

    private void Update()
    {
        _selector.Execute();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(frontAttackPosition.position, new Vector3(20f, 6f, frontAttackRange));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(centerAttackPosition.position, new Vector3(20f, 6f, centerAttackRange));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(backAttackPosition.position, new Vector3(20f, 6f, backAttackRange));
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
}