using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private PlayerDummy player;
    [SerializeField] private float attackRange;
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
        var chaseCondition = new EnemyNodes.ConditionNode(IsPlayerOutOfAttackRange);
        var chaseSeq = new EnemyNodes.SequenceNode();
        chaseSeq.Add(chaseCondition);
        chaseSeq.Add(chaseAction);
        
        _selector.Add(rotateSeq);
        _selector.Add(chaseSeq);
    }

    private void Update()
    {
        _selector.Execute();
    }

    private bool IsPlayerOutOfAttackRange()
    {
        if (GetPlayerDistance() > attackRange)
        {
            _animator.SetBool("IsMoving", true);
            return true;
        }
        else
        {
            _animator.SetBool("IsMoving", false);
            return false;
        }
    }

    private float GetPlayerDistance()
    {
        var playerPos = player.transform.position;
        var fixedPlayerPos = new Vector3(playerPos.x, 0, playerPos.z);
        
        var enemyPos = transform.position;
        var fixedEnemyPos = new Vector3(enemyPos.x, 0, enemyPos.z);   
        
        return Vector3.Distance(fixedPlayerPos, fixedEnemyPos);
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * 5f * Time.deltaTime);
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        return EnemyNodes.NodeStatus.Running;
    }
}