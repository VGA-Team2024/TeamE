using UnityEngine;

namespace CommonSystem.Option
{
    public class OptionWindowModel
    {
        // 各種設定値（音量、難易度、その他の設定）
        private float _masterVolume;
        private float _bgmVolume;
        private float _seVolume;
        private float _voiceVolume;

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
            _masterVolume = 0.8f;
            _bgmVolume = 0.5f;
            _seVolume = 0.5f;
            _voiceVolume = 0.5f;
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