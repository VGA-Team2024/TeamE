using System;
using UnityEngine;

namespace HikanyanLaboratory.Audio
{
    public interface ICriAudioPlayerService : ICriVolume, IDisposable
    {
        Guid Play(string cueName, float volume = 1f, bool isLoop = false);
        Guid Play3D(Transform transform, string cueName, float volume = 1f, bool isLoop = false);
        void Stop(Guid id);
        void Pause(Guid id);
        void Resume(Guid id);
        void StopAll();
        void PauseAll();
        void ResumeAll();
    }
}