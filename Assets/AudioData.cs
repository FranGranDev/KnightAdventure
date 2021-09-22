using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioData : MonoBehaviour
{
    public SoundGroup[] Sound;

    public Sound GetSound(string Name)
    {
        for (int i = 0; i < Sound.Length; i++)
        {
            for (int a = 0; a < Sound[i].sounds.Length; a++)
            {
                if(Sound[i].sounds[a].name == Name)
                {
                    return Sound[i].sounds[a];
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class SoundGroup
{
    public string name;
    public Sound[] sounds;
    [Range(0, 1f)]
    public float Volume;
    [Range(0.3f, 3f)]
    public float pitch;
}
[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    [Range(0,1f)]
    public float volume;
    [Range(0.3f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource Source;
}