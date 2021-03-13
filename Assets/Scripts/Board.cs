using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	[SerializeField] private Cell spawnCell;

	[Header("References")]
	[SerializeField] private Player piecePrefab;

	private List<Cell> cells = new List<Cell>();
	private List<Enemy> enemies = new List<Enemy>();
	private Player piece;
	private Game game;

	void Start()
	{
		game = FindObjectOfType<Game>();
		cells = GetComponentsInChildren<Cell>().ToList();
		cells.ForEach(x => x.Init());
		SpawnPiece();
		cells.ForEach(x => x.Piece = piece);
		enemies = FindObjectsOfType<Enemy>().ToList();
	}

	private void SpawnPiece()
	{
		piece = Instantiate(piecePrefab);
		piece.Init(spawnCell, this);
	}

	public void EndTurn()
	{
		enemies.ForEach(x => x.TryToAttackPlayer());
	}

	public void EndLevel()
	{
		UnselectAllCells();
		game.ReloadLevel();
	}

	public void UnselectAllCells()
	{
		cells.ForEach(x => x.State = CellState.Unselected);
	}

	public void OnlySelectCell(Cell cell)
	{
		cells.Where(x => x != cell).ForEach(x => x.State = CellState.Unselected);
	}
}
