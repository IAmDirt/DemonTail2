using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f, 0.903f, 0.87f)]
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        // hack to set name of clip when you edit the track (need to click track to update name)
        var number = 0;
        foreach (var c in GetClips())
        {
            var name = number;
            number++;
            c.displayName = number.ToString();
        }

        var mixer = ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);    //tells track tp use dialogue behavior
        mixer.GetBehaviour().inkFile = inkFile;
        return mixer;
    }

    [Header("dialogue")]
    public TextAsset inkFile;

    public bool onlyPlayOnce = false;
    private bool _alreadyPlayed;
    public bool alreadyPlayed()
    {
        if (!onlyPlayOnce)
            return false;

        if (!_alreadyPlayed)
        {
            _alreadyPlayed = true;
            return false;
        }
        else
            return true;
    }
}
[System.Serializable]
public class cutSceneDialogue
{
    public string name;

    [TextArea(3, 10)]
    public string[] sentence;
}

