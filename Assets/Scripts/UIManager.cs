using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("Icon Sprites")]
	[SerializeField] protected Sprite pionSprite;
	[SerializeField] protected Sprite fouSprite;
	[SerializeField] protected Sprite cavalierSprite;
	[SerializeField] protected Sprite tourSprite;

	[Header("Portrait Sprites")]
	[SerializeField] protected Sprite pionPortraitSprite;
	[SerializeField] protected Sprite fouPortraitSprite;
	[SerializeField] protected Sprite cavalierPortraitSprite;
	[SerializeField] protected Sprite tourPortraitSprite;

	[Header("Animations")]
	[SerializeField] private float shakeScoreDuration = 0.6f;
	[SerializeField] private float shakeScoreStrenght = 1f;
	[SerializeField] private int shakeScoreVibrato = 15;

	[Header("References")]
	[SerializeField] private Image portrait;
	[SerializeField] private GameObject hud;
	[SerializeField] private Image[] piecesList = new Image[4];
	[SerializeField] private TextMeshProUGUI score;
	[SerializeField] private GameObject gameOverUI;
	[SerializeField] private GameObject winUI;
	[SerializeField] private GameObject scoreUI;
	[SerializeField] private string scoreTitle = "SCORE = {0} pts";
	[SerializeField] private TextMeshProUGUI timerText;
	[SerializeField] private UIScoreController winController;
	[SerializeField] private DialoguesController dialoguesController;

	private PieceType previousType;
	private int timerValue;
	private Coroutine timer;
	private Coroutine waitScore;

	public DialoguesController Dialogues => dialoguesController;

	protected void Awake()
	{
		Instance = this;
		LevelController.Instance.LevelBoard.OnEndLevel += StopTimer;
	}

	public void DisplayPieces(List<PieceType> types)
	{
		int i = 0;
		foreach (PieceType type in types)
		{
			piecesList[i++].sprite = GetSpriteByType(type);
		}
	}

	private Sprite GetSpriteByType(PieceType type)
	{
		Sprite selectedSprite = null;

		switch (type)
		{
			case PieceType.Pion:
				return pionSprite;
			case PieceType.Fou:
				return fouSprite;
			case PieceType.Cavalier:
				return cavalierSprite;
			case PieceType.Tour:
				return tourSprite;
		}

		return selectedSprite;
	}

	public void DisplayPortrait(PieceType currentType)
	{
		if (previousType != currentType)
		{
			previousType = currentType;
			switch (currentType)
			{
				case PieceType.Pion:
					portrait.sprite = pionPortraitSprite;
					break;
				case PieceType.Fou:
					portrait.sprite = fouPortraitSprite;
					break;
				case PieceType.Cavalier:
					portrait.sprite = cavalierPortraitSprite;
					break;
				case PieceType.Tour:
					portrait.sprite = tourPortraitSprite;
					break;
			}

			portrait.GetComponent<Animator>().SetTrigger("change");
		}
	}

	public void DisplayScore(float val, bool shake = true)
	{
		score.text = string.Format(scoreTitle, val.ToString());

		//winController.DisplayScore();

		if (shake)
		{
			score.transform.DOShakePosition(shakeScoreDuration, shakeScoreStrenght, shakeScoreVibrato);
		}
	}

	public void StartTimer(int value)
	{
		timerValue = value;
		timer = StartCoroutine(Timer());
	}

	public void StopTimer()
	{
		StopCoroutine(timer);
	}

	private IEnumerator Timer()
	{
		while (true)
		{
			int minutes = timerValue / 60;
			int secondes = timerValue - minutes * 60;

			string minutesPrefix = minutes >= 10 ? "" : "0";
			string secondesPrefix = secondes >= 10 ? "" : "0";

			timerText.text = $"{minutesPrefix}{minutes}:{secondesPrefix}{Mathf.Max(secondes, 0)}";
			yield return new WaitForSeconds(1f);
			timerValue--;

			if (timerValue < 0)
			{
				LevelController.Instance.LevelBoard.EndLevel();
				break;
			}
		}
	}

	public void DisplayGameOver()
	{
		StartCoroutine(DisplayScreenCore(gameOverUI));
	}

	public void DisplayWin()
	{
		StartCoroutine(DisplayScreenCore(winUI));
		waitScore = StartCoroutine(DisplayScoreScreen());
		winController.DisplayScore();
	}

	private IEnumerator DisplayScreenCore(GameObject prefab)
	{
		yield return new WaitForSeconds(0.65f);

		hud.gameObject.FadOut(0.1f);

		yield return new WaitForSeconds(0.2f);

		prefab.gameObject.SetActive(true);
		prefab.FadIn(0.5f);
	}

	private IEnumerator DisplayScoreScreen()
	{
		yield return new WaitForSeconds(2f);
		this.winUI.FadOut(0.1f);
		this.winUI.gameObject.SetActive(false);
		//yield return new WaitForSeconds(0.2f);
		scoreUI.gameObject.SetActive(true);
		scoreUI.FadIn(0.5f);
	}

	private void Update()
	{
		if (winUI.active)
		{
			if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0))
			{
				StopCoroutine(this.waitScore);
				this.winUI.FadOut(0.1f);
				this.winUI.gameObject.SetActive(false);
				scoreUI.gameObject.SetActive(true);
				scoreUI.FadIn(0.5f);
			}
		}
	}

	private void OnDestroy()
	{
		LevelController.Instance.LevelBoard.OnEndLevel -= StopTimer;
	}
}
