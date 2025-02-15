using System;
using UnityEngine;

public class BossAreaController : MonoBehaviour
{
    [SerializeField] private EnemyController _enemyController;
    [SerializeField, InspectorVariantName("ボスエリアの地面の高さ")] private float _bossAreaGroundHeight = 48f;
    [SerializeField, InspectorVariantName("地面をすり抜けた際の復帰用のレイキャストの長さ")] private float _returnGroundRaycastLength = 10f;
    [SerializeField, InspectorVariantName("ボスエリア地面のレイヤー")] LayerMask _groundLayerMask;
    private bool _isEntered;
    PlayerController _player;

    private void Awake()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_wind");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_isEntered) return;
        if (other.CompareTag("Player"))
        {
            _isEntered = true;
            _enemyController.Activate();
            CRIAudioManager.BGM.Play("CueSheet_BGM", "BGM_boss");
        }
    }

    private void Update()
    {
        if (_player.Variable.PlayerRoot.position.y < _bossAreaGroundHeight)
        {
            var playerPos = _player.Variable.PlayerRoot.position;
            if (Physics.Raycast(playerPos + Vector3.up * _returnGroundRaycastLength, Vector3.down,
                    out RaycastHit hitInfo, _returnGroundRaycastLength, _groundLayerMask))
            {
                _player.Variable.PlayerRoot.position = hitInfo.point;
            }
        }
    }
}
