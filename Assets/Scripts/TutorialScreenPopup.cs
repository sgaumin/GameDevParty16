using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tools.Utils;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenPopup : MonoBehaviour
{
	[Header("Animations")]
	[SerializeField] private float fadeDuration = 0.2f;
	[SerializeField] private float delayCharacterDisplay = 0.01f;

	[Header("Audio")]
	[SerializeField] private AudioExpress showSound;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI description;
	[SerializeField] private Image image;
	[SerializeField] private Button button;

	public Action OnClose = delegate { };

	private List<string> keysToTranslate = new List<string>();

	public void SetDialogueText(string[] keys)
	{
		LevelController.Instance.GameState = GameStates.Pause;
		if (LevelController.Instance.LevelBoard.Player.CurrentCell.Freeze)
			LevelController.Instance.LevelBoard.Player.CurrentCell.PauseFreeze();

		image.raycastTarget = true;
		keysToTranslate = keys.ToList();
		ShowNextPage();
	}

	private void ShowNextPage()
	{
		button.interactable = false;
		string firstKey = keysToTranslate.First();
		keysToTranslate.Remove(firstKey);

		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		StopAllCoroutines();
		StartCoroutine(DialogueCoroutine(I18n.Fields[firstKey]));

		showSound.Play();
	}

	private IEnumerator DialogueCoroutine(string text)
	{
		description.gameObject.FadIn(fadeDuration);
		description.text = "";

		StartCoroutine(DisplayLetterCore(text, description));

		yield return null;

		button.interactable = true;
	}

	private IEnumerator DisplayLetterCore(string text, TextMeshProUGUI target)
	{
		for (int i = 0; i < text.Length; i++)
		{
			target.text += "" + text[i];
			yield return new WaitForSeconds(delayCharacterDisplay);
		}
	}

	public void Next()
	{
		if (!keysToTranslate.IsEmpty())
		{
			ShowNextPage();
		}
		else
		{
			OnClose?.Invoke();
			gameObject.FadOut(fadeDuration);
			image.raycastTarget = false;
			LevelController.Instance.GameState = GameStates.Play;
			if (LevelController.Instance.LevelBoard.Player.CurrentCell.Freeze)
				LevelController.Instance.LevelBoard.Player.CurrentCell.ContinueFreeze();
		}
	}
}
