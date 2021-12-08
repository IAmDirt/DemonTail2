using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onEnablePlaySound : MonoBehaviour
{
    public RandomAudioPlayer audioThing;
    private void OnEnable()
    {
        audioThing.Init();
        audioThing.PlayRandomClip();
    }
}
