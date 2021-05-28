using System;
using TMPro;
using UnityEngine;

public class UIScoreController : MonoBehaviour
{
	[Header("references")]
	[SerializeField] private ScoreController scoreController;
	[SerializeField] private TextMeshProUGUI textScoreNbTurns;
	[SerializeField] private TextMeshProUGUI textScoretime;
	[SerializeField] private TextMeshProUGUI textScoreFou;
	[SerializeField] private TextMeshProUGUI textScoreTower;
	[SerializeField] private TextMeshProUGUI textScoreCavalier;
	[SerializeField] private TextMeshProUGUI textScorePion;
	[SerializeField] private TextMeshProUGUI textScoreTotal;


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
					textScoreTower.text = scoreController.GetScoreKillString(piece);
					break;
			}
		}
		textScoreNbTurns.text = scoreController.GetTextScoreNbCoups();
		textScoretime.text = $"= {scoreController.GetTextScoreTime()}";
		textScoreTotal.text = $"{scoreController.score}";
	}
}
