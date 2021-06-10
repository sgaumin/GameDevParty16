using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tools;
using UnityEngine.Audio;

public class MainMenuController : GameSystem
{
	[SerializeField] private MainMenuState state = MainMenuState.StartScreen;

	[Header("Animation")]
	[SerializeField] private float fadDuration = 0.5f;

	[Header("Audio")]
	[SerializeField] private AudioClip music;
	[SerializeField] private AudioMixerGroup mixer;

	[Header("References")]
	[SerializeField] private GameObject main;
	[SerializeField] private GameObject level;
	[SerializeField] private GameObject OptionUi;
	[SerializeField] private FadScreen fader;

	private Dictionary<MainMenuState, GameObject> statesObjects;
	private GameObject activeGameObject;
	private Coroutine loadingLevel;

	protected override void Awake()
	{
		base.Awake();

		statesObjects = new Dictionary<MainMenuState, GameObject>();
		statesObjects.Add(MainMenuState.StartScreen, main);
		statesObjects.Add(MainMenuState.SelectionLevel, level);
		activeGameObject = statesObjects[state];
		SwitchState((int)state);
	}

	protected void Start()
	{
		fader.FadIn();
		AudioManager.Instance.UpdateMusic(music, mixer);
	}

	public void SwitchState(int stateId)
	{
		state = (MainMenuState)stateId;
		activeGameObject.SetActive(false);
		activeGameObject = statesObjects[state];
		activeGameObject.SetActive(true);
	}

	public void SelectLevel(string levelPrefab)
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				GameData.LevelNameSelected = levelPrefab;
				LevelLoader.LoadNextLevel();
			}));
		}
	}

	private IEnumerator LoadLevelCore(Action content = null)
	{
		Time.timeScale = 1f;
		yield return fader.FadOutCore(fadDuration: fadDuration);
		content?.Invoke();
	}

	public void QuitGame()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.QuitGame();
			}));
		}
	}

	public void OpenOptions()
    {
		this.OptionUi.gameObject.SetActive(true);
    }
}
