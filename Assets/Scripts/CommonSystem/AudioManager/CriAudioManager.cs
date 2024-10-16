using System;
using System.Collections.Generic;
using System.Linq;
using CriWare;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory.CommonSystem;
using UnityEngine;
using R3;

namespace HikanyanLaboratory.Audio
{
    public class CriAudioManager : SingletonMonoBehaviour<CriAudioManager>
    {
        private Dictionary<CriAudioType, ICriAudioPlayerService> _audioPlayers; // 各音声の再生を管理するクラス

        private CriAtomListener _listener; // リスナー
        protected override bool UseDontDestroyOnLoad => true;

        public ReactiveProperty<float> MasterVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<float> BgmVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<float> SeVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<float> VoiceVolume { get; private set; } = new ReactiveProperty<float>(1f);


        protected override async void OnAwake()
        {
            // ACF設定
            string path = Application.streamingAssetsPath + $"/{_audioSetting.StreamingAssetsPathAcf}.acf";
            CriAtomEx.RegisterAcf(null, path);

            // CriAtom作成
            var criAtom = gameObject.AddComponent<CriAtom>();
            await UniTask.WaitUntil(() => criAtom.cueSheets.All(cs => cs.IsLoading == false));

            _listener = FindObjectOfType<CriAtomListener>();
            if (_listener == null)
            {
                _listener = gameObject.AddComponent<CriAtomListener>();
            }

            _audioPlayers = new Dictionary<CriAudioType, ICriAudioPlayerService>();

            foreach (var cueSheet in _audioSetting.AudioCueSheet)
            {
                CriAtom.AddCueSheet(cueSheet.CueSheetName, $"{cueSheet.AcbPath}.acb",
                    !string.IsNullOrEmpty(cueSheet.AwbPath) ? $"{cueSheet.AwbPath}.awb" : null, null);
                if (cueSheet.CueSheetName == CriAudioType.CueSheet_BGM.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_BGM, new BGMPlayer(cueSheet.CueSheetName, _listener));
                }
                else if (cueSheet.CueSheetName == CriAudioType.CueSheet_SE.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_SE, new SEPlayer(cueSheet.CueSheetName, _listener));
                }
                else if (cueSheet.CueSheetName == CriAudioType.CueSheet_Voice.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_Voice, new VoicePlayer(cueSheet.CueSheetName, _listener));
                }

                // 他のCriAudioTypeも同様に追加可能
            }

            // MasterVolumeの変更を監視して、各Playerに反映
            MasterVolume.Subscribe(OnMasterVolumeChanged).AddTo(this);
            BgmVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_BGM, volume)).AddTo(this);
            SeVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_SE, volume)).AddTo(this);
            VoiceVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_Voice, volume)).AddTo(this);
        }

        private void OnMasterVolumeChanged(float volume)
        {
            // MasterVolumeの変更に伴い、各プレイヤーのボリュームを更新する
            foreach (var player in _audioPlayers.Values)
            {
                player.SetVolume(Math.Min(volume, volume * VoiceVolume.Value));
            }
        }

        private void OnVolumeChanged(CriAudioType type, float volume)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                // 各プレイヤーのボリュームがMasterVolumeを超えないように制御する
                player.SetVolume(MasterVolume.Value * volume);
            }
        }


        /// <summary>
        /// Enumで指定されたキュー名を使用して音声を再生するメソッド
        /// </summary>
        public Guid Play<TEnum>(TEnum cue, float volume = 1f, bool isLoop = false) where TEnum : Enum
        {
            // Enumの型から名前空間を取得し、自動でCriAudioTypeを判別する
            var enumType = typeof(TEnum);
            string namespaceName = enumType.Namespace;

            CriAudioType audioType = DetermineAudioType(namespaceName);

            if (audioType == CriAudioType.Other)
            {
                Debug.LogWarning($"Unable to determine CriAudioType for namespace {namespaceName}");
                return Guid.Empty;
            }

            // CueのEnum名を文字列として取得
            string cueName = cue.ToString();

            // 対応するプレイヤーが存在するか確認
            if (_audioPlayers.TryGetValue(audioType, out var player))
            {
                float adjustedVolume = Math.Min(volume, MasterVolume.Value * volume);
                Debug.Log($"Playing AudioType: {audioType}, CueName: {cueName}, Volume: {adjustedVolume}");
                return player.Play(cueName, adjustedVolume, isLoop);
            }
            else
            {
                Debug.LogWarning($"Audio player for {audioType} not available.");
                return Guid.Empty;
            }
        }

        public Guid Play3D<TEnum>(Transform transform, TEnum cue, float volume = 1f,
            bool isLoop = false) where TEnum : Enum
        {
            // Enumの型から名前空間を取得し、自動でCriAudioTypeを判別する
            var enumType = typeof(TEnum);
            string namespaceName = enumType.Namespace;

            CriAudioType audioType = DetermineAudioType(namespaceName);

            if (audioType == CriAudioType.Other)
            {
                Debug.LogWarning($"Unable to determine CriAudioType for namespace {namespaceName}");
                return Guid.Empty;
            }

            // CueのEnum名を文字列として取得
            string cueName = cue.ToString();

            // 対応するプレイヤーが存在するか確認
            if (_audioPlayers.TryGetValue(audioType, out var player))
            {
                float adjustedVolume = Math.Min(volume, MasterVolume.Value * volume);
                Debug.Log($"Playing AudioType: {audioType}, CueName: {cueName}, Volume: {adjustedVolume}");
                return player.Play3D(transform, cueName, adjustedVolume, isLoop);
            }
            else
            {
                Debug.LogWarning($"Audio player for {audioType} not available.");
                return Guid.Empty;
            }
        }

        public void Stop(CriAudioType type, Guid id)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Stop(id);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Pause(CriAudioType type, Guid id)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Pause(id);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Resume(CriAudioType type, Guid id)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Resume(id);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void SetVolume(CriAudioType type, float volume)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                float adjustedVolume = Math.Min(volume, MasterVolume.Value * volume);
                player.SetVolume(adjustedVolume);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }


        public void StopAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.StopAll();
            }
        }

        public void PauseAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.PauseAll();
            }
        }

        public void ResumeAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.ResumeAll();
            }
        }

        public ICriAudioPlayerService GetPlayer(CriAudioType type)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                return player;
            }

            return null;
        }

        public float GetPlayerVolume(CriAudioType type)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                return player.Volume.Value;
            }

            return 1f;
        }

        /// <summary>
        /// 名前空間から自動的にCriAudioTypeを判定するメソッド
        /// </summary>
        private CriAudioType DetermineAudioType(string namespaceName)
        {
            if (namespaceName.Contains("CueSheet_BGM"))
            {
                return CriAudioType.CueSheet_BGM;
            }
            else if (namespaceName.Contains("CueSheet_SE"))
            {
                return CriAudioType.CueSheet_SE;
            }
            else if (namespaceName.Contains("CueSheet_Voice"))
            {
                return CriAudioType.CueSheet_Voice;
            }
            else if (namespaceName.Contains("CueSheet_ME"))
            {
                return CriAudioType.CueSheet_ME;
            }
            else
            {
                return CriAudioType.Other; // その他のタイプとして処理
            }
        }
    }


    public class BGMPlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public BGMPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        protected override void PrePlayCheck(string cueName)
        {
            // BGM 再生時には既存の BGM を止める
            StopAllBGM();
        }

        private void StopAllBGM()
        {
            var idsToStop = new List<Guid>(_playbacks.Keys);
            foreach (var id in idsToStop)
            {
                Stop(id);
            }
        }

        ~BGMPlayer()
        {
            _disposables.Dispose();
        }
    }

    public class SEPlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public SEPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        ~SEPlayer()
        {
            _disposables.Dispose();
        }
    }
}