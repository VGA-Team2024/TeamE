using UnityEngine;
using UnityEngine.Playables;
using CriWare.Assets;


public class CRIAudioPlayableBehaviour : PlayableBehaviour
{
    public CriAtomCueReference CueReference;
    public bool StopOnEnd;

    private bool _hasPlayed = false;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!_hasPlayed && Application.isPlaying && CueReference.AcbAsset != null)
        {
            // CRIAudioManagerでサウンドを再生
            CRIAudioManager.BGM.Play(CueReference.AcbAsset.name, CueReference.CueId);
            _hasPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (StopOnEnd && Application.isPlaying)
        {
            CRIAudioManager.BGM.Stop();
        }
    }
}