using Tools.Utils;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{
	[SerializeField] private AudioExpress sound;

	public void PlaySound()
	{
		sound.Play();
	}
}
