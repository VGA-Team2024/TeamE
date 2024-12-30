using UnityEngine;
using UnityEngine.Playables;
using CriWare.Assets;

[System.Serializable]
public class CRIAudioPlayableAsset : PlayableAsset
{
    public CriAtomCueReference CueReference;
    public bool StopOnEnd;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CRIAudioPlayableBehaviour>.Create(graph);
        var behaviour = playable.GetBehaviour();

        behaviour.CueReference = CueReference;
        behaviour.StopOnEnd = StopOnEnd;

        return playable;
    }
}