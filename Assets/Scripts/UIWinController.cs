using System;
using TMPro;
using UnityEngine;

public class UIWinController : MonoBehaviour
{
	[Header("references")]
	[SerializeField] private ScoreController scoreController;
	[SerializeField] private TextMeshProUGUI textScoreNbTurns;
	[SerializeField] private TextMeshProUGUI textScoretime;
	[SerializeField] private TextMeshProUGUI textScoreFou;
	[SerializeField] private TextMeshProUGUI textScoreTour;
	[SerializeField] private TextMeshProUGUI textScoreCavalier;
	[SerializeField] private TextMeshProUGUI textScorePion;

	public void DisplayScore()
	{
		foreach (PieceType piece in (PieceType[])Enum.GetValues(typeof(PieceType)))
		{
			switch (piece)
			{
				case PieceType.Pion:
					textScorePion.text = scoreController.GetScoreKillString(piece);
					break;
				case PieceType.Fou:
					textScoreFou.text = scoreController.GetScoreKillString(piece);
					break;
				case PieceType.Cavalier:
					textScoreCavalier.text = scoreController.GetScoreKillString(piece);
					break;
				case PieceType.Tour:
					textScoreTour.text = scoreController.GetScoreKillString(piece);
					break;
			}
		}
		textScoreNbTurns.text = scoreController.GetTextScoreNbCoups();
		textScoretime.text = scoreController.GetTextScoreTime();
	}
}
