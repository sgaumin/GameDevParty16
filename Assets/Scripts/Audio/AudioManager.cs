using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //our array of sounds
    //public Sound[] sounds;

    //public AudioClip musicClip;
    //public AudioSource audioSource;
    //SoundData music;

    public static AudioManager Instance { get; private set; }

    protected void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Start()
    //{
    //    //music.Play();
    //}

    //internal void ChangeMusic(AudioClip music)
    //{
    //    Instance.musicClip = music;
    //}
}
