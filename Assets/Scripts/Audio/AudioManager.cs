using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private AudioSource audioSource;

	public AudioSource Source => audioSource;

	public static AudioManager Instance { get; private set; }

	protected void Awake()
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void UpdateMusic(AudioClip clip, AudioMixerGroup mixer)
	{
		if (Source.clip != null && clip.name == Source.clip.name)
			return;

		Source.clip = clip;
		Source.outputAudioMixerGroup = mixer;

		if (!Source.isPlaying)
		{
			Instance.Source.Play();
		}
	}
}
