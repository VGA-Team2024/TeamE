using System;
using UnityEngine;

namespace HikanyanLaboratory.Audio
{
    public interface ICriAudioPlayerService : ICriVolume, IDisposable
    {
        int Play(string cueName, float volume = 1f, bool isLoop = false);
        int Play3D(Transform transform, string cueName, float volume = 1f, bool isLoop = false);
        void Stop(int id);
        void Pause(int id);
        void Resume(int id);
        void StopAll();
        void PauseAll();
        void ResumeAll();
    }
}