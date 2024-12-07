using UnityEngine;

namespace CommonSystem.AudioManager.CriAudioPlayer
{
    public class CriControlPanelModel
    {
        /// <summary>
        /// 音量変更処理（スライダー）
        /// </summary>
        public void ChangeVolumeBySlider(SoundType soundType, float value)
        {
            float normalizedValue = value / 100f;
            Debug.Log($"SoundType {soundType}: スライダーで音量変更 - {normalizedValue}");
            //TODO: AudioManagerに音量を反映させる処理をここに追加
        }

        /// <summary>
        /// 音量変更処理（入力フィールド）
        /// </summary>
        public void ChangeVolumeByInput(SoundType soundType, string value)
        {
            if (float.TryParse(value, out float result))
            {
                float normalizedValue = result / 100f;
                Debug.Log($"SoundType {soundType}: 入力で音量変更 - {normalizedValue}");
                //TODO:  AudioManagerに音量を反映させる処理をここに追加
            }
            else
            {
                Debug.LogWarning($"SoundType {soundType}: 入力が無効です - {value}");
            }
        }
    }
}