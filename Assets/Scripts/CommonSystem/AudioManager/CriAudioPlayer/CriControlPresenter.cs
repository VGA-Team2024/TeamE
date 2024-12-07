using System.Collections.Generic;
using UnityEngine;
using CommonSystem.AudioManager.CriAudioPlayer;

public class CriControlPresenter : MonoBehaviour
{
    [SerializeField] private CriControlPanelView _controlPanelView;

    private CriControlPanelModel _model;

    // TODO: セーブデータからロードする
    private void Start()
    {
        _model = new CriControlPanelModel();

        // 初期音量値の設定
        var initialVolumes = new Dictionary<SoundType, float>
        {
            { SoundType.MASTER, 0.8f },
            { SoundType.BGM, 0.5f },
            { SoundType.SE, 0.5f },
            { SoundType.VOICE, 0.5f }
        };

        // コントロールパネルの初期化
        _controlPanelView.Initialize(
            initialVolumes,
            onSliderChanged: (soundType, value) => _model.ChangeVolumeBySlider(soundType, value),
            onInputChanged: (soundType, value) => _model.ChangeVolumeByInput(soundType, value)
        );
    }
}