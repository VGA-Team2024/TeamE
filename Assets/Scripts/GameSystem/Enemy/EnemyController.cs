using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyController : MonoBehaviour
{
    [SerializeField, Header("�v���C���[�ւ̒Ǐ]���~���鋗��")] private float _chaseThreshold;

    [SerializeField, Header("�v���C���[�։�]���鑬�x")] private float _rotationSpeed;

    [SerializeField, Header("�v���C���[�֒Ǐ]���鑬�x")] private float _moveSpeed;

    /// <summary>�v���C���[�ւ̉�]�������������Ƃ�\���t���O</summary>
    private bool _isRotationCompleted;

    /// <summary>�v���C���[�N���X</summary>
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
    // �ǐՃA�N�V����
    //-------------------------------------------------------------------------------

    /// <summary>�ǐՃA�N�V����</summary>
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

    /// <summary>�v���C���[���Ǐ]�������������ɂ��邩</summary>
    private bool IsPlayerAway() => CalculateDistanceBetweenPlayer() > _chaseThreshold ? true : false;

    /// <summary>Y���W�𖳎������v���C���[�Ƃ̋������Z�o����</summary>
    private float CalculateDistanceBetweenPlayer()
    {
        Vector3 fixedPlayerPos = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);

        Vector3 fixedEnemyPos = new Vector3(transform.position.x, 0, transform.position.z);

        return Vector3.Distance(fixedPlayerPos, fixedEnemyPos);
    }

    //-------------------------------------------------------------------------------
    // �U���A�N�V����
    //-------------------------------------------------------------------------------


}