using DG.Tweening;
using Tools.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "SoundData", order = 2)]

public class SoundData : ScriptableObject
{
    public AudioExpress instrument;
    public AudioExpress voice;
    [ContextMenu("PlaySounds")]
    public void PlaySounds()
    {
        PlayInstrumentSound();
        PlayVoiceSound();
    }

    public void PlayInstrumentSound()
    {
        instrument.Play();
    }

    public void PlayVoiceSound()
    {
        voice.Play();
    }
}
