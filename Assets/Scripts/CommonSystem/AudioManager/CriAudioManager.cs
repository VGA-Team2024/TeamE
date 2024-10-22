using System;
using System.Collections.Generic;
using System.Linq;
using CriWare;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory.CommonSystem;
using UnityEngine;
using R3;
using UnityEngine.AddressableAssets;

namespace HikanyanLaboratory.Audio
{
    public class CriAudioManager : SingletonMonoBehaviour<CriAudioManager>
    {
        private Dictionary<CriAudioType, ICriAudioPlayerService> _audioPlayers; // 各音声の再生を管理するクラス

        private CriAtomListener _listener; // リスナー
        protected override bool UseDontDestroyOnLoad => true;
        bool _isReady = false;
        public bool IsReady => _isReady;
        public ReactiveProperty<float> MasterVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<float> BgmVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<float> SeVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public ReactiveProperty<float> VoiceVolume { get; private set; } = new ReactiveProperty<float>(1f);


        protected override async void OnAwake()
        {
            var criAtom = FindObjectOfType<CriAtom>();
            if (criAtom == null)
            {
                _isReady = false;

                var obj = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/CRI.prefab").WaitForCompletion();
                Utility.Instantiate(obj);
                Addressables.Release(obj);

                criAtom = FindObjectOfType<CriAtom>();
            }

            if (_isReady) return;

            // キューシートファイルのロード待ち
            await UniTask.WaitUntil(() => criAtom.cueSheets.All(cs => cs.IsLoading == false));
            _isReady = true;

            // CriAtomListenerの設定
            _listener = FindObjectOfType<CriAtomListener>();
            if (_listener == null)
            {
                _listener = gameObject.AddComponent<CriAtomListener>();
            }

            // プレイヤーの初期化
            _audioPlayers = new Dictionary<CriAudioType, ICriAudioPlayerService>();

            foreach (var cueSheet in criAtom.cueSheets)
            {
                switch (cueSheet.name)
                {
                    case nameof(CriAudioType.Cuesheet_BGM):
                        _audioPlayers.Add(CriAudioType.Cuesheet_BGM, new BGMPlayer(cueSheet.name, _listener));
                        break;
                    case nameof(CriAudioType.Cuesheet_SE):
                        _audioPlayers.Add(CriAudioType.Cuesheet_SE, new SEPlayer(cueSheet.name, _listener));
                        break;
                    case nameof(CriAudioType.Cuesheet_VOICE):
                        _audioPlayers.Add(CriAudioType.Cuesheet_VOICE, new VoicePlayer(cueSheet.name, _listener));
                        break;
                    default:
                        Debug.LogWarning($"Unknown CueSheet: {cueSheet.name}");
                        break;
                }
            }

            // 音量監視の設定
            MasterVolume.Subscribe(OnMasterVolumeChanged).AddTo(this);
            BgmVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.Cuesheet_BGM, volume)).AddTo(this);
            SeVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.Cuesheet_SE, volume)).AddTo(this);
            VoiceVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.Cuesheet_VOICE, volume)).AddTo(this);
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
                //Debug.Log($"Playing AudioType: {audioType}, CueName: {cueName}, Volume: {adjustedVolume}");
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
            return _audioPlayers.GetValueOrDefault(type);
        }

        public float GetPlayerVolume(CriAudioType type)
        {
            return _audioPlayers.TryGetValue(type, out var player) ? player.Volume.Value : 1f;
        }

        /// <summary>
        /// 名前空間から自動的にCriAudioTypeを判定するメソッド
        /// </summary>
        private CriAudioType DetermineAudioType(string namespaceName)
        {
            if (namespaceName.Contains(CriAudioType.Cuesheet_BGM.ToString()))
            {
                return CriAudioType.Cuesheet_BGM;
            }
            else if (namespaceName.Contains(CriAudioType.Cuesheet_SE.ToString()))
            {
                return CriAudioType.Cuesheet_SE;
            }
            else if (namespaceName.Contains(CriAudioType.Cuesheet_VOICE.ToString()))
            {
                return CriAudioType.Cuesheet_VOICE;
            }
            else if (namespaceName.Contains(CriAudioType.Cuesheet_ME.ToString()))
            {
                return CriAudioType.Cuesheet_ME;
            }
            else
            {
                return CriAudioType.Other; // その他のタイプとして処理
            }
        }

        protected override void OnDestroy()
        {
            // 音声プレイヤーを全て停止してリソースを解放
            if (_audioPlayers != null)
            {
                foreach (var player in _audioPlayers.Values)
                {
                    player.StopAll(); // すべての音声再生を停止
                    player.Dispose(); // リソース解放
                }

                _audioPlayers.Clear();
            }

            // CRI関連のリスナーやキューシートのリソースも解放
            if (_listener != null)
            {
                Destroy(_listener.gameObject); // リスナーを明示的に破棄
                _listener = null;
            }

            // CriAtomの解放 (必要に応じて)
            var criAtom = FindObjectOfType<CriAtom>();
            if (criAtom != null)
            {
                Destroy(criAtom.gameObject);
            }

            Debug.Log("CRI Audio Manager and resources have been destroyed.");
            base.OnDestroy();
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

    public class VoicePlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public VoicePlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        ~VoicePlayer()
        {
            _disposables.Dispose();
        }
    }
}