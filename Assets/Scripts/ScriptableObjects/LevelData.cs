using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "Data", menuName = "LevelData", order = 1)]
public class LevelData : ScriptableObject
{
	public string Name;
	public Board Prefab;

	[Header("Audio")]
	public AudioClip clip;
	public AudioMixerGroup mixer;
}