using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [SerializeField, Header("プレイヤーへの追従を停止する距離")] private float _chaseThreshold;

    [SerializeField, Header("プレイヤーへ回転する速度")] private float _rotationSpeed;

    [SerializeField, Header("プレイヤーへ追従する速度")] private float _moveSpeed;

    /// <summary>プレイヤーへの回転が完了したことを表すフラグ</summary>
    private bool _isRotationCompleted;

    /// <summary>プレイヤークラス</summary>
    [SerializeField] private PlayerDummy _player;

    private Rigidbody _rb;


    void Start()
    {
        if (_player == null)
        {
            Debug.LogError("Player Not Found.");
        }

        _rb = GetComponent<Rigidbody>();

        RotateTowardsPlayer();
    }

    void FixedUpdate()
    {
        if (IsPlayerAway())
        {
            HandleChase();
        }
    }

    //-------------------------------------------------------------------------------
    // 追跡アクション
    //-------------------------------------------------------------------------------

    /// <summary>追跡アクション</summary>
    private void HandleChase()
    {
        if (!_isRotationCompleted) return;

        Vector3 dir = (_player.transform.position - transform.position).normalized;
        dir.y = 0f;
        _rb.MovePosition(transform.position +  dir * _moveSpeed * Time.deltaTime);
    }

    private void RotateTowardsPlayer()
    {
        Vector3 dir = (_player.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.DORotateQuaternion(rotation, _rotationSpeed).OnComplete(() =>
        {
            _isRotationCompleted = true;
        });
    }

    /// <summary>プレイヤーが追従距離よりも遠くにいるか</summary>
    private bool IsPlayerAway() => CalculateDistanceBetweenPlayer() > _chaseThreshold ? true : false;

    /// <summary>Y座標を無視したプレイヤーとの距離を算出する</summary>
    private float CalculateDistanceBetweenPlayer()
    {
        Vector3 fixedPlayerPos = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);

        Vector3 fixedEnemyPos = new Vector3(transform.position.x, 0, transform.position.z);

        return Vector3.Distance(fixedPlayerPos, fixedEnemyPos);
    }

    //-------------------------------------------------------------------------------
    // 攻撃アクション
    //-------------------------------------------------------------------------------


}