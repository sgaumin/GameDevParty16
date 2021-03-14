using System.Collections;
using Tools;
using UnityEngine;

public class LoadSceneAfterDuration : MonoBehaviour
{
	void Start()
	{
		StartCoroutine(Core());
	}

	private IEnumerator Core()
	{
		yield return new WaitForSeconds(45f);
		LevelLoader.LoadNextLevel();
	}
}
