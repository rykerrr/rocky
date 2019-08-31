using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;
    public bool loop;

    [Range(0f, 1f)]
    public float vol;
    [Range(0.1f, 3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource audioSource;
}
