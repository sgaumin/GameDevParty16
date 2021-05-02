using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class IntroductionManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private VideoPlayer video;
	[SerializeField] private LoadSceneAfterDuration sceneLoader;

	protected void Start()
	{
		video.url = System.IO.Path.Combine(Application.streamingAssetsPath, "introduction_chess-runner.mp4");
		StartCoroutine(StartLoader());
	}

	private IEnumerator StartLoader()
	{
		while (video.length == 0)
		{
			yield return new WaitForEndOfFrame();
		}

		sceneLoader.Duration = (float)video.length;
		sceneLoader.StartLoader();
	}
}
