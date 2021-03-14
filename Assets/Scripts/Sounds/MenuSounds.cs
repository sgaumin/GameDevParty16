using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSounds : MonoBehaviour
{
    public SoundData startSound;

    public void PlayStarSound()
    {
        startSound.instrument.Play();
        startSound.voice.Play();
    }
}
