using System;
using TMPro;
using UnityEngine;

public class UIScoreController : MonoBehaviour
{
	[Header("references")]
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
				case PieceType.Pawn:
					textScorePion.text = ScoreController.Instance.GetScoreKillString(piece);
					break;
				case PieceType.Bishop:
					textScoreFou.text = ScoreController.Instance.GetScoreKillString(piece);
					break;
				case PieceType.Knight:
					textScoreCavalier.text = ScoreController.Instance.GetScoreKillString(piece);
					break;
				case PieceType.Rook:
					textScoreTower.text = ScoreController.Instance.GetScoreKillString(piece);
					break;
			}
		}
		textScoreNbTurns.text = ScoreController.Instance.GetTextScoreNbCoups();
		textScoretime.text = $"= {ScoreController.Instance.GetTextScoreTime()}";
		textScoreTotal.text = $"{ScoreController.Instance.score}";
	}
}
