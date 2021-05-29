using System.Collections;
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
		Game.Instance.LevelBoard.player.OnKillEnemy += SetNotifications;
		Game.Instance.LevelBoard.OnStartPlayerTurn += MoveDialogue;
	}

	public void MoveDialogue()
	{
		if (nbMoves % nbMovesBeforeDialogue == 0)
		{
			SetDialogue(Game.Instance.LevelBoard.player.currentType, DialogueType.Normal);
		}

		Game.Instance.LevelBoard.player.hasKilled = false;
		nbMoves++;
	}

	public void SetNotifications(PieceType piece, DialogueType dialogueType)
	{
		string[] dialogueBase = { "NONE" };
		switch (piece)
		{
			case PieceType.Pion:
				notificationIcon.sprite = pionSprite;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = pion.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = pion.dialogue;
						break;
				}
				break;
			case PieceType.Fou:
				notificationIcon.sprite = fouSprite;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = fou.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = fou.dialogue;
						break;
				}
				break;
			case PieceType.Cavalier:
				notificationIcon.sprite = cavalierSprite;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = cavalier.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = cavalier.dialogue;
						break;
				}
				break;
			case PieceType.Tour:
				notificationIcon.sprite = tourSprite;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = tour.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = tour.dialogue;
						break;
				}
				break;
		}

		string dialogue = dialogueBase[Random.Range(0, dialogueBase.Length)];
		notificationText.gameObject.FadIn(fadeDuration);
		notificationText.text = "";
		notificationAnim.SetTrigger("show");

		StartCoroutine(DisplayLetterCore(dialogue, notificationText));
	}

	public void SetDialogue(DialogueType dialogueType)
	{
		PieceType type = (PieceType)Random.Range(0, System.Enum.GetValues(typeof(PieceType)).Length);
		SetDialogue(type, dialogueType);
	}

	// Dialogue choisi aléatoirement dans la base des dialogues de la pièce choisie
	public void SetDialogue(PieceType piece, DialogueType dialogueType)
	{
		string[] dialogueBase = { "NONE" };
		string name = "NONE";
		switch (piece)
		{
			case PieceType.Pion:
				name = pion.name;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = pion.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = pion.dialogue;
						break;
				}
				break;
			case PieceType.Fou:
				name = fou.name;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = fou.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = fou.dialogue;
						break;
				}
				break;
			case PieceType.Cavalier:
				name = cavalier.name;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = cavalier.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = cavalier.dialogue;
						break;
				}
				break;
			case PieceType.Tour:
				name = tour.name;
				switch (dialogueType)
				{
					case DialogueType.Attaque:
						dialogueBase = tour.dialogueAttaque;
						break;
					case DialogueType.Normal:
						dialogueBase = tour.dialogue;
						break;
				}
				break;
		}
		string dialogue = dialogueBase[Random.Range(0, dialogueBase.Length)];
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
		Game.Instance.LevelBoard.player.OnKillEnemy -= SetNotifications;
		Game.Instance.LevelBoard.OnStartPlayerTurn -= MoveDialogue;

		if (dialogueCoroutine != null)
		{
			StopCoroutine(dialogueCoroutine);
		}
	}
}
