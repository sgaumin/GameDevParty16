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

	[Header("References")]
	[SerializeField] private Board board;
	[SerializeField] private Image portrait;
	[SerializeField] private Image[] piecesList = new Image[4];
	[SerializeField] private TextMeshProUGUI score;
	[SerializeField] private GameObject gameOverUI;
	[SerializeField] private string scoreTitle = "SCORE = {0} pts";
	[SerializeField] private TextMeshProUGUI timerText;



	private int timerValue;
	private Coroutine timer;

	protected void Awake()
	{
		Instance = this;

		board.OnEndLevel += StopTimer;
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
	}

	public void DisplayScore(float val)
	{
		score.text = string.Format(scoreTitle, val.ToString());
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
				board.EndLevel();
				break;
			}
		}
	}

	public void DisplayGameOver()
	{
		gameOverUI.SetActive(true);
	}

	private void OnDestroy()
	{
		board.OnEndLevel -= StopTimer;
	}
}
