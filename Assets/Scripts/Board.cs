using System;
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

	public Action OnStartPlayerTurn;
	public Action OnEndPlayerTurn;

	void Start()
	{
		game = FindObjectOfType<Game>();
		cells = GetComponentsInChildren<Cell>().ToList();
		enemies = FindObjectsOfType<Enemy>().ToList();

		cells.ForEach(x => x.Init());
		SpawnPiece();
		cells.ForEach(x => x.Piece = piece);

		OnStartPlayerTurn?.Invoke();
	}

	private void SpawnPiece()
	{
		piece = Instantiate(piecePrefab, transform);
		piece.Init(spawnCell);
	}

	public void EndTurnPlayer()
	{
		OnEndPlayerTurn?.Invoke();
	}

	public void EndTurnEnemies()
	{
		if (enemies.All(x => x.HasFinishTurn))
		{
			OnStartPlayerTurn?.Invoke();
		}
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
