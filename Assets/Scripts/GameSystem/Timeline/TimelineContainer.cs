using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineContainer : MonoBehaviour
{
    [SerializeField] private List<PlayableDirector> _playableDirectors;
    public List<PlayableDirector> PlayableDirectors => _playableDirectors;
}
