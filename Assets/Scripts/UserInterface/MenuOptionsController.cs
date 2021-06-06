using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class MenuOptionsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider sliderVolumeMusic;
    [SerializeField] private Slider sliderVolumeSFX;
    [SerializeField] private AudioMixer audioMixer;

    Resolution[] resolutions;

    private void Awake()
    {
#if UNITY_WEBGL
#else
        // Résolutio
        int currentResolutionId = 0;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int index = 0;
        foreach (Resolution resolution in resolutions)
        {
            options.Add($"{resolution.width} x {resolution.height}");
            if(resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionId = index;
            }
            index++;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionId;
        resolutionDropdown.RefreshShownValue();
#endif

        // Volumes
        sliderVolumeSFX.value = GameData.VolumeSFX;
        sliderVolumeMusic.value = GameData.VolumeMusic;
    }

    public void SetVolumeMusic(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
        GameData.VolumeMusic = volume;
    }

    public void SetVolumeSFX(float volume)
    {
        audioMixer.SetFloat("sfxVolume", volume);
        GameData.VolumeSFX = volume;
    }

    public void SetResolution(int resolutionId)
    {

        Resolution resolution = resolutions[resolutionId];
        Debug.Log($"Update resoluition {resolutionId}: {resolution}");
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool fullSceen)
    {
        Screen.fullScreen = fullSceen;
    }

    public void SetLanguage(string language)
    {
        Language lang = (Language)Enum.Parse(typeof(Language), language);
        GameData.Language = Enum.GetName(typeof(Language), lang);
    }
}
