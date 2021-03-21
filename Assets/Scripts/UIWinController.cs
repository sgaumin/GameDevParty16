using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIWinController : MonoBehaviour
{
    [Header("references")]
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private TextMeshProUGUI textScoreFou;
    [SerializeField] private TextMeshProUGUI textScoreTour;
    [SerializeField] private TextMeshProUGUI textScoreCavalier;
    [SerializeField] private TextMeshProUGUI textScorePion;
    private Dictionary<PieceType, TextMeshProUGUI> textPiecesKIlled;

    public void Init()
    {
        textPiecesKIlled = new Dictionary<PieceType, TextMeshProUGUI>();
        textPiecesKIlled.Add(PieceType.Cavalier, textScoreCavalier);
        textPiecesKIlled.Add(PieceType.Tour, textScoreTour);
        textPiecesKIlled.Add(PieceType.Fou, textScoreFou);
        textPiecesKIlled.Add(PieceType.Pion, textScorePion);
    }

    public void DisplayScore()
    {
        foreach (PieceType piece in (PieceType[])Enum.GetValues(typeof(PieceType)))
        {
            Debug.Log($"{piece} -> {scoreController.GetScoreKillString(piece)}");
            textPiecesKIlled[piece].text = scoreController.GetScoreKillString(piece);
        }
    }
}
