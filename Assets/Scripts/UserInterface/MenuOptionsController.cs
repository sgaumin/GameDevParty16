using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuOptionsController : MonoBehaviour
{
	public static MenuOptionsController Instance { get; private set; }

	[Header("UI Elements")]
	[SerializeField] private TMP_Dropdown resolutionDropdown;
	[SerializeField] private Slider sliderVolumeMusic;
	[SerializeField] private Slider sliderVolumeSFX;
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private Toggle toggleFr;
	[SerializeField] private Toggle toggleEn;
	[SerializeField] private GameObject holder;


	public Action onLanguageChanged;

	Resolution[] resolutions;

	private void Awake()
	{
		Instance = this;
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

		// Language
		Language lang = (Language)Enum.Parse(typeof(Language), GameData.Language);
		Debug.Log($"{lang}");
		switch (lang)
		{
			case Language.FR:
				toggleFr.Select();
				break;
			case Language.EN:
				toggleEn.Select();
				break;
		}
	}

	public void Show()
	{
		holder.SetActive(true);

		// Language
		Language lang = (Language)Enum.Parse(typeof(Language), GameData.Language);
		switch (lang)
		{
			case Language.FR:
				toggleFr.isOn = true;
				break;
			case Language.EN:
				toggleEn.isOn = true;
				break;
		}
		if (UIManager.Instance != null && UIManager.Instance.ShowTimer)
		{
			UIManager.Instance.StopTimer();
		}
		if (LevelController.Instance != null)
		{
			Board board = LevelController.Instance.LevelBoard;
			board.StopAutoDeletionRows();
			board.DisableInput();
			if (board.Player.CurrentCell.Freeze)
				board.Player.CurrentCell.PauseFreeze();
		}
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
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
	}

	public void SetFullScreen(bool fullSceen)
	{
		Screen.fullScreen = fullSceen;
	}

	public void SetFrenchLanguage()
	{
		SetLanguage(Language.FR);
	}

	public void SetEnglishLanguage()
	{
		SetLanguage(Language.EN);
	}

	private void SetLanguage(Language language)
	{
		GameData.Language = language.ToString();
		I18n.LoadLanguageFromGameData(language);
		onLanguageChanged?.Invoke();
	}

	public void Quit()
	{
		if (UIManager.Instance != null && UIManager.Instance.ShowTimer)
		{
			UIManager.Instance.ContinueTimer();
		}
		if (LevelController.Instance != null)
		{
			Board board = LevelController.Instance.LevelBoard;
			board.StartAutoDeletionRows();
			board.AllowInput();
			if (board.Player.CurrentCell.Freeze)
				board.Player.CurrentCell.ContinueFreeze();
		}

		holder.SetActive(false);
	}
}
