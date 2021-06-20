using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguesController : MonoBehaviour
{
	[SerializeField] private float fadeDuration = 0.2f;
	[SerializeField] private float delayCharacterDisplay = 0.01f;

	[Header("Notification Icons")]
	[SerializeField] protected Sprite pionSprite;
	[SerializeField] protected Sprite fouSprite;
	[SerializeField] protected Sprite cavalierSprite;
	[SerializeField] protected Sprite tourSprite;

	[Header("References")]
	[SerializeField] private TextMeshProUGUI nameText;
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private Animator notificationAnim;
	[SerializeField] private TextMeshProUGUI notificationText;
	[SerializeField] private Image notificationIcon;

	[Header("params")]
	[SerializeField] private int nbAtack = 5;
	[SerializeField] private int nbNormal = 10;
	private Dictionary<DialogueType, int> nbDialogues;

	Coroutine dialogueCoroutine;
	Coroutine autoSwitchDialogue;

	public int timeBetweenDialogues;
	public int nbMovesBeforeDialogue;
	private int nbMoves = 0;

	public DialogueData pion;
	public DialogueData fou;
	public DialogueData cavalier;
	public DialogueData tour;

	public void Init()
	{
		nbDialogues = new Dictionary<DialogueType, int>();
		nbDialogues.Add(DialogueType.Normal, nbNormal);
		nbDialogues.Add(DialogueType.Attaque, nbAtack);

		LevelController.Instance.LevelBoard.Player.OnKillEnemy += SetNotifications;
		LevelController.Instance.LevelBoard.OnStartPlayerTurn += MoveDialogue;
	}

	public void MoveDialogue()
	{
		if (nbMoves % nbMovesBeforeDialogue == 0)
		{
			SetDialogue(LevelController.Instance.LevelBoard.Player.CurrentType, DialogueType.Normal);
		}

		LevelController.Instance.LevelBoard.Player.HasKilled = false;
		nbMoves++;
	}

	public void SetNotifications(PieceType piece, DialogueType dialogueType)
	{
		switch (piece)
		{
			case PieceType.Pawn:
				notificationIcon.sprite = pionSprite;
				break;
			case PieceType.Bishop:
				notificationIcon.sprite = fouSprite;
				break;
			case PieceType.Knight:
				notificationIcon.sprite = cavalierSprite;
				break;
			case PieceType.Rook:
				notificationIcon.sprite = tourSprite;
				break;
		}

		string dialogueId = $"{Enum.GetName(typeof(DialogueType), dialogueType)}" +
			$"{Enum.GetName(typeof(PieceType), piece)}" +
			$"{UnityEngine.Random.Range(1, nbDialogues[dialogueType] + 1)}";
		string dialogue = I18n.Fields[dialogueId];
		notificationText.gameObject.FadIn(fadeDuration);
		notificationText.text = "";
		notificationAnim.SetTrigger("show");

		StartCoroutine(DisplayLetterCore(dialogue, notificationText));
	}

	public void SetDialogue(DialogueType dialogueType)
	{
		PieceType type = (PieceType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(PieceType)).Length);
		SetDialogue(type, dialogueType);
	}

	// Dialogue choisi aléatoirement dans la base des dialogues de la pièce choisie
	public void SetDialogue(PieceType piece, DialogueType dialogueType)
	{
		string name = "NONE";
		switch (piece)
		{
			case PieceType.Pawn:
				name = pion.name;
				break;
			case PieceType.Bishop:
				name = fou.name;
				break;
			case PieceType.Knight:
				name = cavalier.name;
				break;
			case PieceType.Rook:
				name = tour.name;
				break;
		}

		string dialogueId = $"{Enum.GetName(typeof(DialogueType), dialogueType)}" +
			$"{Enum.GetName(typeof(PieceType), piece)}" +
			$"{UnityEngine.Random.Range(1, nbDialogues[dialogueType] + 1)}";

		string dialogue = I18n.Fields[dialogueId];
		SetDialogueText(dialogue, name);
	}

	public void SetDialogueText(string text, string name)
	{
		dialogueCoroutine = StartCoroutine(DialogueCoroutine(text, name));
	}

	public void ResetAutoSwitchDialogue()
	{
		if (autoSwitchDialogue != null)
		{
			StopCoroutine(autoSwitchDialogue);
		}
	}

	private IEnumerator AutoSwitchDialogue()
	{
		while (true)
		{
			SetDialogue(DialogueType.Normal);
			yield return new WaitForSeconds(timeBetweenDialogues);
		}
	}

	private IEnumerator DialogueCoroutine(string text, string name)
	{
		if (name != nameText.text)
		{
			nameText.gameObject.FadIn(fadeDuration);
		}
		dialogueText.gameObject.FadIn(fadeDuration);

		dialogueText.text = "";
		nameText.text = "";

		StartCoroutine(DisplayLetterCore(text, dialogueText));
		StartCoroutine(DisplayLetterCore(name, nameText));

		yield return null;
	}

	private IEnumerator DisplayLetterCore(string text, TextMeshProUGUI target)
	{
		for (int i = 0; i < text.Length; i++)
		{
			target.text += "" + text[i];
			yield return new WaitForSeconds(delayCharacterDisplay);
		}
	}

	protected virtual void OnDestroy()
	{
		LevelController.Instance.LevelBoard.Player.OnKillEnemy -= SetNotifications;
		LevelController.Instance.LevelBoard.OnStartPlayerTurn -= MoveDialogue;

		if (dialogueCoroutine != null)
		{
			StopCoroutine(dialogueCoroutine);
		}
	}
}
