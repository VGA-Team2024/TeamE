using System;
using System.Collections.Generic;
using CriWare;
using R3;
using UnityEngine;

namespace HikanyanLaboratory.Audio
{
    public abstract class CriAudioPlayerService : ICriAudioPlayerService
    {
        private readonly CriAtomExPlayer _criAtomExPlayer; // 複数の音声を再生するためのプレイヤー
        private readonly CriAtomEx3dSource _criAtomEx3dSource; // 3D音源
        protected readonly Dictionary<int, CriAtomExPlayback> _playbacks; // 再生中の音声を管理
        private readonly CriAtomListener _criAtomListener; // リスナー
        private readonly string _cueSheetName; // ACBファイルの名前
        private const float MasterVolume = 1f; // マスターボリューム
        public ReactiveProperty<float> Volume { get; private set; } = new ReactiveProperty<float>(1f); // ボリューム

        public CriAudioPlayerService(string cueSheetName, CriAtomListener criAtomListener)
        {
            _cueSheetName = cueSheetName;
            _criAtomListener = criAtomListener;
            _criAtomExPlayer = new CriAtomExPlayer();
            _criAtomEx3dSource = new CriAtomEx3dSource();
            _playbacks = new Dictionary<int, CriAtomExPlayback>();

            Volume.Subscribe(SetVolume);
        }

        ~CriAudioPlayerService()
        {
            Dispose();
        }

        private void SetVolumeInternal(float volume)
        {
            _criAtomExPlayer.SetVolume(volume * MasterVolume);
        }

        public virtual int Play(string cueName, float volume = 1f, bool isLoop = false)
        {
            if (!CheckCueSheet())
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return -1;
            }

            var tempAcb = CriAtom.GetCueSheet(_cueSheetName).acb;
            tempAcb.GetCueInfo(cueName, out var cueInfo);

            PrePlayCheck(cueName);
            _criAtomExPlayer.SetCue(tempAcb, cueName);
            _criAtomExPlayer.SetVolume(volume * Volume.Value * MasterVolume);
            _criAtomExPlayer.Loop(isLoop);

            var playback = _criAtomExPlayer.Start();
            int id = (int)playback.id;
            _playbacks[id] = playback;
            return id;
        }

        public virtual int Play3D(Transform transform, string cueName, float volume = 1f, bool isLoop = false)
        {
            if (!CheckCueSheet())
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return -1;
            }

            var tempAcb = CriAtom.GetCueSheet(_cueSheetName).acb;
            tempAcb.GetCueInfo(cueName, out var cueInfo);

            PrePlayCheck(cueName);

            _criAtomEx3dSource.SetPosition(transform.position.x, transform.position.y, transform.position.z);
            _criAtomEx3dSource.Update();

            _criAtomExPlayer.Set3dSource(_criAtomEx3dSource);
            _criAtomExPlayer.Set3dListener(_criAtomListener.nativeListener);
            _criAtomExPlayer.SetCue(tempAcb, cueName);
            _criAtomExPlayer.SetVolume(volume * Volume.Value * MasterVolume);
            _criAtomExPlayer.Loop(isLoop);

            var playback = _criAtomExPlayer.Start();
            int id = (int)playback.id;
            _playbacks[id] = playback;

            return id;
        }

        public void Stop(int id)
        {
            if (_playbacks.ContainsKey(id))
            {
                _playbacks[id].Stop();
                _playbacks.Remove(id);
            }
        }

        public void Pause(int id)
        {
            if (_playbacks.ContainsKey(id))
            {
                _playbacks[id].Pause();
            }
        }

        public void Resume(int id)
        {
            if (_playbacks.ContainsKey(id))
            {
                _playbacks[id].Resume(CriAtomEx.ResumeMode.PausedPlayback);
            }
        }

        public void StopAll()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Stop();
            }

            _playbacks.Clear();
        }

        public void PauseAll()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Pause();
            }
        }

        public void ResumeAll()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
            }
        }

        public void SetVolume(float volume)
        {
            _criAtomExPlayer.SetVolume(volume * MasterVolume);
            _criAtomExPlayer.UpdateAll();
        }

        public void Dispose()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Stop();
            }

            _criAtomExPlayer.Dispose();
            _criAtomEx3dSource.Dispose();
        }

        public bool CheckCueSheet()
        {
            var tempAcb = CriAtom.GetCueSheet(_cueSheetName)?.acb;
            if (tempAcb == null)
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return false;
            }

            return true;
        }

        public void CheckPlayerStatus()
        {
            var idsToRemove = new List<int>();

            foreach (var kvp in _playbacks)
            {
                if (kvp.Value.GetStatus() == CriAtomExPlayback.Status.Removed)
                {
                    idsToRemove.Add(kvp.Key);
                }
            }

            foreach (var id in idsToRemove)
            {
                _playbacks.Remove(id);
            }
        }

        protected virtual void PrePlayCheck(string cueName)
        {
        }
    }
}