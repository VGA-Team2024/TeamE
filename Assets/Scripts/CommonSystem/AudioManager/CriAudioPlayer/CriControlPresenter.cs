using System.Collections.Generic;
using UnityEngine;
using CommonSystem.AudioManager.CriAudioPlayer;

public class CriControlPresenter : MonoBehaviour
{
    [SerializeField] private CriControlPanelView _controlPanelView;

    private CriControlPanelModel _model;

    // TODO: セーブデータからロードする
    public void Initialize(Dictionary<SoundType, float> initialVolumes)
    {
        _model = new CriControlPanelModel();
        // コントロールパネルの初期化
        _controlPanelView.Initialize(
            initialVolumes,
            onSliderChanged: (soundType, value) => _model.ChangeVolumeBySlider(soundType, value),
            onInputChanged: (soundType, value) => _model.ChangeVolumeByInput(soundType, value)
        );
    }

    public Dictionary<SoundType, float> GetCurrentVolumes()
    {
        return _model.GetCurrentVolumes();
    }
}