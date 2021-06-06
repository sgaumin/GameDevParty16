using System.IO;
using UnityEngine;


public static class GameData
{
	private const string VOLUME_MUSIC = "VolumeMusic";
	private const string VOLUME_SFX = "VolumeSFX";
	private const string LANGUAGE = "language";

	public static string LevelNameSelected { get; set; }

	public static float VolumeMusic
	{
		get
		{
			return PlayerPrefs.HasKey(VOLUME_MUSIC) ? PlayerPrefs.GetFloat(VOLUME_MUSIC) : 0f;
		}
		set
		{
			PlayerPrefs.SetFloat(VOLUME_MUSIC, value);
			PlayerPrefs.Save();
		}
	}

	public static float VolumeSFX
	{
		get
		{
			return PlayerPrefs.HasKey(VOLUME_SFX) ? PlayerPrefs.GetFloat(VOLUME_SFX) : 0f;
		}
		set
		{
			PlayerPrefs.SetFloat(VOLUME_SFX, value);
			PlayerPrefs.Save();
		}
	}

	public static string Language
	{
		get
		{
			return PlayerPrefs.HasKey(LANGUAGE) ? PlayerPrefs.GetString(LANGUAGE) : "FR";
		}
		set
		{
			PlayerPrefs.SetString(LANGUAGE, value);
			PlayerPrefs.Save();
		}
	}

	public static void DeleteAllSave()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
