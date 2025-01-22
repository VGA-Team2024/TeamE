using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerVariable
{
    [Header("プレイヤーの共有変数")]
    [SerializeField, InspectorVariantName("壁のレイヤー")] private LayerMask _wallLayer;
    [SerializeField, InspectorVariantName("地面のレイヤー")] private LayerMask _groundLayer;
    [SerializeField, InspectorVariantName("プレイヤーのレイヤー")] private LayerMask _playerLayer;
    [SerializeField, InspectorVariantName("プレイヤーのAnimator")] private Animator _animator;
    [SerializeField, InspectorVariantName("プレイヤーのRigidbody")] private Rigidbody _rigidbody;
    [SerializeField, InspectorVariantName("プレイヤーのCapsuleCollider(Raycastで半径の値を利用するため)")] private CapsuleCollider _capsuleCollider;
    [SerializeField, InspectorVariantName("プレイヤーの全てのコライダー(オンオフを切り替えるため)")] private List<Collider> _colliders;
    [SerializeField, InspectorVariantName("プレイヤーのTransform")] private Transform _playerRoot;
    [SerializeField, InspectorVariantName("カメラのTransform")] private Transform _cameraTransform;
    [SerializeField, InspectorVariantName("壁の角度")] private float _wallAngle;
    [Header("プレイヤーのコンポーネント")]
    [SerializeField] private MoveController _moveController;
    [SerializeField] private PlayerBowController _bowController;
    [SerializeField] private PlayerCameraController _cameraController;
    [SerializeField] private ClimbController _climbController;
    [SerializeField] private PlayerAttackController _attackController;
    [SerializeField] private PlayerHealthCalculator _healthCalculator;
    [SerializeField] private PlayerEventReceiver _playerEventReceiver;
    [SerializeField] private RagdollController _ragdollController;
    public LayerMask WallLayer => _wallLayer;
    public LayerMask GroundLayer => _groundLayer;
    public LayerMask PlayerLayer => _playerLayer;
    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public CapsuleCollider CapsuleCollider => _capsuleCollider;
    public List<Collider> Colliders => _colliders;
    public Transform PlayerRoot => _playerRoot;
    public Transform CameraTransform => _cameraTransform;
    public float WallAngle => _wallAngle;
    public MoveController MoveController => _moveController;
    public PlayerBowController BowController => _bowController;
    public PlayerCameraController CameraController => _cameraController;
    public ClimbController ClimbController => _climbController;
    public PlayerAttackController AttackController => _attackController;
    public PlayerHealthCalculator HealthCalculator => _healthCalculator;
    public PlayerEventReceiver PlayerEventReceiver => _playerEventReceiver;
    public bool IsAttack { get; set; }
    public bool IsLanding { get; set; }
    public bool IsArrowCharging { get; set; }
    public bool IsClimbing { get; set; }
    public bool CanWalk { get; set; }
    public bool IsKnockBack { get; set; }
    public Vector3 GroundNormal { get; set; } = Vector3.up;
    public float ArrowChargeRate { get; set; }

    public void InjectSelf()
    {
        _attackController?.InjectVariable(this);
        _climbController?.InjectVariable(this);
        _moveController?.InjectVariable(this);
        _bowController?.InjectVariable(this);
        _cameraController?.InjectVariable(this);
        _ragdollController?.InjectVariable(this);
    }
}