using System;
using UnityEngine;

namespace CommonSystem.Option
{
    [Serializable]
    public class OptionModel
    {
       [SerializeField,] private float _masterVolume;
       [SerializeField] private float _bgmVolume;
       [SerializeField] private float _seVolume;
       [SerializeField] private float _voiceVolume;

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
            PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
            PlayerPrefs.SetFloat("SEVolume", _seVolume);
            PlayerPrefs.SetFloat("VoiceVolume", _voiceVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 設定をデフォルト値にリセットします。
        /// </summary>
        public void ResetToDefault()
        {
            _masterVolume = 1.0f;
            _bgmVolume = 0.8f;
            _seVolume = 0.8f;
            _voiceVolume = 0.8f;
            SaveSettings();
        }

        /// <summary>
        /// 保存済みの設定を読み込みます。
        /// </summary>
        public void LoadSettings()
        {
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
            _bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
            _seVolume = PlayerPrefs.GetFloat("SEVolume", 0.5f);
            _voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.5f);
        }

        // 各種設定値のプロパティ
        public float MasterVolume
        {
            get => _masterVolume;
            set => _masterVolume = Mathf.Clamp01(value);
        }

        public float BGMVolume
        {
            get => _bgmVolume;
            set => _bgmVolume = Mathf.Clamp01(value);
        }

        public float SEVolume
        {
            get => _seVolume;
            set => _seVolume = Mathf.Clamp01(value);
        }


        /// <summary>
        /// 設定値を全て取得します（デバッグ用）。
        /// </summary>
        /// <returns>設定値の文字列表現</returns>
        public override string ToString()
        {
            return $"MasterVolume: {_masterVolume}, BGMVolume: {_bgmVolume}, SEVolume: {_seVolume}";
        }
    }
}