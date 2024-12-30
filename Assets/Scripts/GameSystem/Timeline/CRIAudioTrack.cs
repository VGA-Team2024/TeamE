using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.1f, 0.5f, 0.8f)]
[TrackClipType(typeof(CRIAudioPlayableAsset))]
public class CRIAudioTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CRIAudioPlayableBehaviour>.Create(graph, inputCount);
    }
}