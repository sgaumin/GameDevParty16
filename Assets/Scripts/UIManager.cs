using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("References")]
	[SerializeField] private Board board;
	[SerializeField] private TextMeshProUGUI piecesList;
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
		int i = 1;
		piecesList.text = "";
		foreach (PieceType type in types)
		{
			piecesList.text += i.ToString() + "- " + type.ToString() + "\n";
			i++;
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
