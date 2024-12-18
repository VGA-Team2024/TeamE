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
            float normalizedValue = Mathf.Clamp01(value / 100f);
            // Debug.Log($"SoundType {soundType}: スライダーで音量変更 - {normalizedValue}");
            // AudioManagerに音量を反映
            ApplyVolumeChange(soundType, normalizedValue);
        }

        /// <summary>
        /// 音量変更処理（入力フィールド）
        /// </summary>
        public void ChangeVolumeByInput(SoundType soundType, string value)
        {
            if (float.TryParse(value, out float result))
            {
                float normalizedValue = Mathf.Clamp01(result / 100f);
                // Debug.Log($"SoundType {soundType}: 入力で音量変更 - {normalizedValue}");
                // AudioManagerに音量を反映
                ApplyVolumeChange(soundType, normalizedValue);
            }
            else
            {
                Debug.LogWarning($"SoundType {soundType}: 入力が無効です - {value}");
            }
        }

        /// <summary>
        /// 音量変更を反映する
        /// </summary>
        private void ApplyVolumeChange(SoundType soundType, float normalizedValue)
        {
            switch (soundType)
            {
                // case SoundType.MASTER:
                //     CRIAudioManager
                //     break;
                case SoundType.BGM:
                    CRIAudioManager.BGM.SetVolume(normalizedValue);
                    break;
                case SoundType.SE:
                    CRIAudioManager.SE.SetVolume(normalizedValue);
                    break;
                case SoundType.VOICE:
                    CRIAudioManager.VOICE.SetVolume(normalizedValue);
                    break;
                default:
                    Debug.LogWarning($"未対応のSoundType: {soundType}");
                    break;
            }
        }
    }
}