using UnityEngine;

public class AudioManager : MonoBehaviour
{
	//our array of sounds
	//public Sound[] sounds;

	//public AudioClip musicClip;
	//public AudioSource audioSource;
	//SoundData music;

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

	//private void Start()
	//{
	//    //music.Play();
	//}

	//internal void ChangeMusic(AudioClip music)
	//{
	//    Instance.musicClip = music;
	//}
}
