using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UnityEngine;
using UnityEngine.Audio;

public class LevelController : GameSystem
{
	public static LevelController Instance { get; private set; }

	private const string MENU_SCENE = "Menu";

	public delegate void GameEventHandler();
	public event GameEventHandler OnStart;
	public event GameEventHandler OnGameOver;
	public event GameEventHandler OnPause;

	[SerializeField] private Board levelBoard;

	[Header("Animation")]
	[SerializeField] private float fadDuration = 0.5f;

	[Header("References")]
	[SerializeField] private FadScreen fader;
	[SerializeField] private Transform levelHolder;
	[SerializeField] private CinemachineVirtualCamera cinemachine;
	[SerializeField] private List<Board> levelPrefabs = new List<Board>();

	private GameStates gameState;
	private Coroutine loadingLevel;

	public AudioClip music;
	public AudioMixerGroup mixer;

	public Board LevelBoard { get; private set; }
	public GameStates GameState
	{
		get => gameState;
		set
		{
			gameState = value;

			switch (value)
			{
				case GameStates.Play:
					OnStart?.Invoke();
					break;

				case GameStates.GameOver:
					OnGameOver?.Invoke();
					break;

				case GameStates.Pause:
					OnPause?.Invoke();
					break;
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		Instance = this;
		if (levelBoard != null)
		{
			LevelBoard = Instantiate(levelBoard, levelHolder);
		}
		else if (!string.IsNullOrEmpty(GameData.LevelNameSelected))
		{
			LevelBoard = Instantiate(levelPrefabs.Where(x => x.name == GameData.LevelNameSelected).FirstOrDefault(), levelHolder);
		}
	}

	protected void Start()
	{
		GameState = GameStates.Play;
		AudioManager.Instance.UpdateMusic(music, mixer);
		fader.FadIn();
	}

	protected override void Update()
	{
		base.Update();
	}

	public void SetCameraTarget(Transform target)
	{
		cinemachine.Follow = target;
		cinemachine.LookAt = target;
	}

	public void ReloadLevel()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.ReloadLevel();
			}));
		}
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

	public void LoadNextLevel()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadNextLevel();
			}));
		}
	}

	public void LoadMenu()
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadLevelByName(MENU_SCENE);
			}));
		}
	}

	public void LoadSceneByName(string sceneName)
	{
		if (loadingLevel == null)
		{
			loadingLevel = StartCoroutine(LoadLevelCore(

			content: () =>
			{
				LevelLoader.LoadLevelByName(sceneName);
			}));
		}
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

	private IEnumerator LoadLevelCore(Action content = null)
	{
		Time.timeScale = 1f;
		yield return fader.FadOutCore(fadDuration: fadDuration);
		content?.Invoke();
	}

}