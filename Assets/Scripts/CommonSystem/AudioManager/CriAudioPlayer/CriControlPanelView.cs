using System.Collections.Generic;
using UnityEngine;

public class CriControlPanelView : MonoBehaviour
{
    [SerializeField] private Transform _volumeControlsParent; // 親Transform
    [SerializeField] private Transform _buttonControlsParent; // 親Transform
    [SerializeField] private CriVolumeControl _volumeControlPrefab; // VolumeControlのPrefab
    [SerializeField] private AudioButtonControl _audioButtonControlPrefab; // AudioButtonControlのPrefab
    [SerializeField] private List<VolumeControlSettings> _volumeControlSettings; // 設定リスト

    private List<CriVolumeControl> _volumeControls = new List<CriVolumeControl>(); // 生成されたボリュームコントロールのリスト
    private List<AudioButtonControl> _audioButtonControls = new List<AudioButtonControl>(); // 生成されたボタンコントロールのリスト
    private CRIAudioManager _criAudioManager;

    private void Awake()
    {
        // CRIAudioManagerのインスタンスを取得
        CRIAudioManager.Initialize(); //初期化
        _criAudioManager = CRIAudioManager.Instance;

        // 設定に従いPrefabを生成
        foreach (var setting in _volumeControlSettings)
        {
            CreateControlPair(setting);
        }
    }

    /// <summary>
    /// ボリュームコントロールとボタンコントロールのペアを生成
    /// </summary>
    /// <param name="setting">生成に使用する設定</param>
    private void CreateControlPair(VolumeControlSettings setting)
    {
        // CriVolumeControlの生成と初期化
        var volumeControl = Instantiate(_volumeControlPrefab, _volumeControlsParent);
        volumeControl.Initialize(
            label: setting.Label,
            initialValue: setting.InitialValue,
            soundType: setting.SoundType,
            onSliderChanged: (value) => OnVolumeSliderChanged(setting.SoundType, value),
            onInputChanged: (value) => OnVolumeInputChanged(setting.SoundType, value)
        );
        _volumeControls.Add(volumeControl);
        
        // AudioButtonControlの生成と初期化
        var audioButtonControl = Instantiate(_audioButtonControlPrefab, _buttonControlsParent);
        audioButtonControl.Initialize(
            soundType: setting.SoundType,
            cueSheet: setting.CueSheet,
            cueName: setting.CueName
        );
        _audioButtonControls.Add(audioButtonControl);
    }

    /// <summary>
    /// スライダー変更時のコールバック
    /// </summary>
    private void OnVolumeSliderChanged(SoundType soundType, float value)
    {
        _criAudioManager.SetVolume(soundType, value);
        Debug.Log($"SoundType {soundType}: スライダーで音量変更 - {value}");
    }

    /// <summary>
    /// 入力フィールド変更時のコールバック
    /// </summary>
    private void OnVolumeInputChanged(SoundType soundType, string value)
    {
        if (float.TryParse(value, out float result))
        {
            _criAudioManager.SetVolume(soundType, result / 100f);
            Debug.Log($"SoundType {soundType}: 入力で音量変更 - {result}");
        }
        else
        {
            Debug.LogWarning($"SoundType {soundType}: 入力が無効です - {value}");
        }
    }
}