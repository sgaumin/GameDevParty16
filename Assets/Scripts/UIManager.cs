using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static UIManager Instance { get; private set; }

	[Header("References")]
	[SerializeField] private TextMeshProUGUI piecesList;

	protected void Awake() => Instance = this;

	public void DisplayPieces(List<PieceType> types)
	{
		piecesList.text = "";
		foreach (PieceType type in types)
		{
			piecesList.text += type.ToString() + "\n";
		}
	}
}
