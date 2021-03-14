using UnityEngine;

public class ScoreController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] protected Board board;
	[SerializeField] protected UIManager ui;
	static float bestScore;
	private float multiplier;
	private int nbTurns = 0;
	private System.DateTime startTime;
	private int score = 0;

	public int Score
	{
		get => score;
		set
		{
			score = value;
			ui.DisplayScore(score);
		}
	}

	protected virtual void Awake()
	{
		board.OnStartPlayerTurn += PlayerTurnStart;
		board.OnPlayerSelectedCell += PlayerTurnEnd;
		board.OnEndLevel += EndLevelReached;

		score = 0;
		ui.DisplayScore(score, false);
	}

	private void PlayerTurnStart()
	{
		startTime = System.DateTime.Now;
		//Debug.Log($"PlayerTurnStart {startTime}");
	}

	private void PlayerTurnEnd()
	{
		nbTurns++;
		System.DateTime endTime = System.DateTime.Now;
		System.TimeSpan delta = (endTime - startTime);
		int val = SetScore((float)delta.TotalSeconds);
		//Debug.Log($"PlayerTurnEnd {endTime}, {delta.TotalSeconds}, {val}");
		Score += val;
	}

	private void EndLevelReached()
	{
		Score += SetScore(nbTurns);
	}

	public int SetScore(float delta)
	{
		return (int)(100.0f * Mathf.Exp(-0.1f * delta)); // 100*e^(-0.1x)
	}

	private void OnDestroy()
	{
		board.OnStartPlayerTurn -= PlayerTurnStart;
		board.OnPlayerSelectedCell -= PlayerTurnEnd;
		board.OnEndLevel -= EndLevelReached;
	}
}
