using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialoguesController : MonoBehaviour
{
	[Header("References")]
    //[SerializeField] private GameObject dialogueBox;
    [SerializeField] private Board board;
    [SerializeField] private TextMeshProUGUI dialogueText;
	Coroutine dialogueCoroutine;
	Coroutine autoSwitchDialogue;

	public int timeBetweenDialogues;

	public DialogueData pion;
	public DialogueData fou;
	public DialogueData cavalier;
	public DialogueData tour;

    private void Start()
    {
        //board.piece.OnKillEnemy += SetDialogue;
    }

    private void Awake()
    {
		//autoSwitchDialogue = StartCoroutine(AutoSwitchDialogue());
	}

    public void Init()
    {
        board.piece.OnKillEnemy += SetDialogue;
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
        switch (piece)
        {
            case PieceType.Pion:
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
		SetDialogueText(dialogue);
    }

	public void SetDialogueText(string text)
    {
		dialogueCoroutine = StartCoroutine(DialogueCoroutine(text));
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
		while(true)
        {
			SetDialogue(DialogueType.Normal);
			yield return new WaitForSeconds(timeBetweenDialogues);
        }
	}

	private IEnumerator DialogueCoroutine(string text)
    {
		//dialogueBox.gameObject.SetActive(true);
		dialogueText.text = text;
        yield return new WaitForSeconds(3);
        //dialogueBox.gameObject.SetActive(false);
        //dialogueText.text = "";
    }

	protected virtual void OnDestroy()
	{
        board.piece.OnKillEnemy -= SetDialogue;
        if (dialogueCoroutine != null)
		{
			StopCoroutine(dialogueCoroutine);
		}
	}
}
