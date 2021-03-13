using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField] private Cell spawnCell;

	[Header("References")]
	[SerializeField] private PieceMouvment piecePrefab;

	private List<Cell> allCells = new List<Cell>();
	PieceMouvment piece;

	void Start()
	{
		SpawnPiece();
		allCells = GetComponentsInChildren<Cell>().ToList();
		allCells.ForEach(x => x.Piece = piece);
	}

	private void SpawnPiece()
	{
		piece = Instantiate(piecePrefab);
		piece.Init(spawnCell, this);
	}

	public void UnselectAllCells()
	{
		allCells.ForEach(x => x.State = CellState.Unselected);
	}

	public void OnlySelectCell(Cell cell)
	{
		allCells.Where(x => x != cell).ForEach(x => x.State = CellState.Unselected);
	}
}
