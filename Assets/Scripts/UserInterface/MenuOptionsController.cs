using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuOptionsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    
    Resolution[] resolutions;

    private void Awake()
    {
#if UNITY_WEBGL
#else
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
}
