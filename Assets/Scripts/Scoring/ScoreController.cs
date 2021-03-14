using UnityEngine;

public class ScoreController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] protected Board board;
	[SerializeField] protected UIManager ui;
	static float bestScore;
	private float multiplier;
	public int nbTurns = 0;
	private System.DateTime startTime;
	public int score = 0;
	public int scoreMove = 0;
	public int scoreEndLevel = 0;
	public int scoreKill = 0;
	public int scoreKillPion = 0;
    public int scoreKillFou = 0;
    public int scoreKillCavalier = 0;
    public int scoreKillTour = 0;

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
	}

	private void PlayerTurnEnd()
	{
		nbTurns++;
		System.DateTime endTime = System.DateTime.Now;
		System.TimeSpan delta = (endTime - startTime);
		int val = SetScoreTime((float)delta.TotalMilliseconds);

		// Son si score > 100
		if (val >= 100)
		{
			// TODO: Play Sound
		}

		scoreMove += val;
		Score += val;
	}

	private void EndLevelReached()
	{
		int val = SetScoreCoups(nbTurns);
		scoreEndLevel += val;
		Score += val;
	}

	public int SetScoreTime(float delta)
	{
		//int score = (int)((10000.0f * Mathf.Exp(-0.001f * delta)) / 10.0f); //((10000*e^(-0.001x)) + 0) / 10

		int val = (int)(((-0.5f * delta) + 450.0f) - 150.0f); // ((-0.5x) +450) -150
		val = Mathf.Clamp(val, 10, 300); // min 10 pts
		return val;
	}

	public int SetScoreCoups(int nbCoups)
    {
		return ((-10 * nbCoups) + 2000) *10; // ((-10x) + 2000) * 10
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
				val = 900;
				scoreKillTour += val;
                break;
            case PieceType.Fou:
				val = 600;
				scoreKillFou += val;
                break;
            case PieceType.Cavalier:
				val = 350;
				scoreKillCavalier += val;
                break;
            case PieceType.Pion:
				val = 100;
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

	private void OnDestroy()
	{
		board.OnStartPlayerTurn -= PlayerTurnStart;
		board.OnPlayerSelectedCell -= PlayerTurnEnd;
		board.OnEndLevel -= EndLevelReached;
	}
}
