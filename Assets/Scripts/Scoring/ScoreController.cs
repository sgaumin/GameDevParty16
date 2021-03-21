using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] protected Board board;
	[SerializeField] protected UIManager ui;
	[SerializeField] private TextMeshProUGUI winScore;
	public SoundData mouveSounds;

	// Tour: 900
	// Fou: 600
	// Cavalier: 350
	// Pion: 100
	[Header("rules")]
	[SerializeField] private int valKillTour = 900;
	[SerializeField] private int valKillFou = 600;
	[SerializeField] private int valKillCavalier = 350;
	[SerializeField] private int valKillPion = 100;

	private int nbTourKilled = 0;
	private int nbFouKilled = 0;
	private int nbCavalierKilled = 0;
	private int nbPionKilled = 0;

	static float bestScore;
	private float multiplier;
	public int nbTurns = 0;
	private System.DateTime startTime;
	public int score = 0;
	public int scoreMove = 0;
	public int scoreEndLevel = 0;
	public int scoreKill = 0;
	public int scoreKillFou = 0;
	public int scoreKillCavalier = 0;
	public int scoreKillTour = 0;
	public int scoreKillPion = 0;

	public int Score
	{
		get => score;
		set
		{
			score = value;
			ui.DisplayScore(score);
			winScore.text = score.ToString();
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
	}

	private void PlayerTurnEnd()
	{
		nbTurns++;
		if (!board.player.IsAlive)
			return;
		System.DateTime endTime = System.DateTime.Now;
		System.TimeSpan delta = (endTime - startTime);
		int val = GetScoreTime((float)delta.TotalMilliseconds);

		// Son si score > 100
		if (val >= 100)
		{
			mouveSounds.voice.Play();
		}

		scoreMove += val;
		Score += val;
	}

	private void EndLevelReached()
	{
		if (!board.player.IsAlive)
			return;
		int val = GetScoreCoups(nbTurns);
		scoreEndLevel += val;
		Score += val;
	}


	public int GetScoreTime(float delta)
	{
		//int score = (int)((10000.0f * Mathf.Exp(-0.001f * delta)) / 10.0f); //((10000*e^(-0.001x)) + 0) / 10

		int val = (int)(((-0.5f * delta) + 450.0f) - 150.0f); // ((-0.5x) +450) -150
		val = Mathf.Clamp(val, 10, 300); // min 10 pts
		return val;
	}

	public string GetTextScoreTime()
    {
		return $"{scoreMove}";
    }

	public int GetScoreCoups(int nbCoups)
	{
		return ((-10 * nbCoups) + 2000) * 10; // ((-10x) + 2000) * 10
	}

	public string GetTextScoreNbCoups()
    {
		return $"{nbTurns} tours = {scoreEndLevel}";
    }

	// Tour: 900
	// Fou: 600
	// Cavalier: 350
	// Pion: 100
	public int SetScoreKill(PieceType type)
	{
		int val = 0;
		switch (type)
		{
			case PieceType.Tour:
				val = valKillTour;
				nbTourKilled++;
				scoreKillTour += val;
				break;
			case PieceType.Fou:
				val = valKillFou;
				nbFouKilled++;
				scoreKillFou += val;
				break;
			case PieceType.Cavalier:
				val = valKillCavalier;
				nbCavalierKilled++;
				scoreKillCavalier += val;
				break;
			case PieceType.Pion:
				val = valKillPion;
				nbPionKilled++;
				scoreKillPion += val;
				break;
			default:
				val = 0;
				break;
		}
		//val *= 10;
		scoreKill += val;
		Score += val;
		return val;
	}

	public string GetScoreKillString(PieceType piece)
    {
        switch (piece)
        {
            case PieceType.Pion:
				return $"x {nbPionKilled} = {scoreKillPion}";
                break;
            case PieceType.Fou:
				return $"x {nbFouKilled} = {scoreKillFou}";
                break;
            case PieceType.Cavalier:
				return $"x {nbCavalierKilled} = {scoreKillCavalier}";
                break;
            case PieceType.Tour:
				return $"x {nbTourKilled} = {scoreKillTour}";
                break;
            default:
				return "";
                break;
        }
    }

	private void OnDestroy()
	{
		board.OnStartPlayerTurn -= PlayerTurnStart;
		board.OnPlayerSelectedCell -= PlayerTurnEnd;
		board.OnEndLevel -= EndLevelReached;
	}
}
