using System.Collections.Generic;
using UnityEngine;

public class CriControlPanelView : MonoBehaviour
{
    [SerializeField] private List<CriVolumeControl> _volumeControls = new List<CriVolumeControl>();

    /// <summary>
    /// ボリュームコントロールの初期化
    /// </summary>
    public void Initialize(Dictionary<SoundType, float> initialVolumes,
        System.Action<SoundType, float> onSliderChanged,
        System.Action<SoundType, string> onInputChanged)
    {
        foreach (var volumeControl in _volumeControls)
        {
            // 各 CriVolumeControl から SoundType を取得
            var soundType = volumeControl.SoundType;

            if (initialVolumes.TryGetValue(soundType, out float initialValue))
            {
                // 各音量コントロールの初期化
                volumeControl.Initialize(
                    label: soundType.ToString(),
                    initialValue: initialValue,
                    onSliderChanged: value => onSliderChanged(soundType, value),
                    onInputChanged: value => onInputChanged(soundType, value)
                );
            }
            else
            {
                Debug.LogWarning($"SoundType {soundType} の初期値が設定されていません。");
            }
        }
    }
}