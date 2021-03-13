using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("References")]
	[SerializeField] private TextMeshProUGUI piecesList;
	[SerializeField] private TextMeshProUGUI score;
	[SerializeField] private string scoreTitle = "Score : ";

	protected void Awake() => Instance = this;

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
}
