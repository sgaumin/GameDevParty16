using System.Collections;
using Tools;
using UnityEngine;

public class LoadSceneAfterDuration : MonoBehaviour
{
	private Coroutine loader;

	public float Duration { get; set; }

	public void StartLoader()
	{
		loader = StartCoroutine(Core());
	}

	private void Update()
	{
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_EDITOR
		if (Input.GetButtonDown("Quit"))
		{
			LevelLoader.QuitGame();
		}
#endif

		if (Input.anyKeyDown)
		{
			if (loader != null)
			{
				StopCoroutine(loader);
			}
			LevelLoader.LoadNextLevel();
		}
	}

	private IEnumerator Core()
	{
		yield return new WaitForSeconds(Duration);
		LevelLoader.LoadNextLevel();
	}
}
