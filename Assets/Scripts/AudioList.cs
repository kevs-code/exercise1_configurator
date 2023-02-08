using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "AudioList", menuName = "New AudioList", order = 0)]
public class AudioList : ScriptableObject
{
    public AudioClip click;
    public AudioClip hover;
    public AudioClip start;
    public AudioClip buy;
    public AudioClip background;
}