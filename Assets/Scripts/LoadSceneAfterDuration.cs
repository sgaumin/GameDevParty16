using System.Collections;
using Tools;
using UnityEngine;

public class LoadSceneAfterDuration : MonoBehaviour
{
	private Coroutine loader;

	void Start()
	{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
		loader = StartCoroutine(Core());
#else
	LevelLoader.LoadNextLevel();
#endif
	}

	private void Update()
	{
		if (Input.GetButtonDown("Quit"))
		{
			LevelLoader.QuitGame();
		}

		if (Input.anyKeyDown)
		{
			StopCoroutine(loader);
			LevelLoader.LoadNextLevel();
		}
	}

	private IEnumerator Core()
	{
		yield return new WaitForSeconds(45f);
		LevelLoader.LoadNextLevel();
	}
}
