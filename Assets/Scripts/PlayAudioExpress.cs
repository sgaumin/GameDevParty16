using Tools.Utils;
using UnityEngine;

public class PlayAudioExpress : MonoBehaviour
{
	[SerializeField] private AudioExpress sound;

	public void Play()
	{
		sound.Play();
	}
}
