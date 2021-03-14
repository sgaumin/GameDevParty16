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
	[SerializeField] private string scoreTitle = "Score : ";
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
		score.text = scoreTitle + val.ToString();
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
			timerText.text = $"{Mathf.Max(timerValue, 0)}";
			yield return new WaitForSeconds(1f);
			timerValue--;

			if (timerValue < 0)
			{
				board.EndLevel();
				break;
			}
		}
	}

	private void OnDestroy()
	{
		board.OnEndLevel -= StopTimer;
	}
}
