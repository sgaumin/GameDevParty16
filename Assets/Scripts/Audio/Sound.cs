using UnityEngine.Audio;
using UnityEngine;


//A lightweight version of the Class AudioSource, used by AudioManager
//It can be expanded according to our needs
[System.Serializable]
public class Sound
{
    //The actual audiosource
    [HideInInspector]
    public AudioSource source;

    //The sound effect
    public AudioClip clip;
    //the name
    public string name;
    //if we want it to loop or not
    public bool loop;

    //for the next variables, we put first a [Range(min, max)] so it appear as a slider in the Unity editor

    //the volume
    [Range(0.0f, 1.0f)]
    public float volume;
    //the pitch, higher->faster and squirrel-like
    [Range(0.1f, 3.0f)]
    public float pitch;
    //the 3D effect 
    //0 meaning it'll be reproduced at full sound, 
    //1 meaning it'll be reproduced following a logarithmic falloff
    //this is how the 8D music is made basically
    [Range(0.0f, 1.0f)]
    public float spatialBlend;

}
