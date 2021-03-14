using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : Character
{
	[Header("Config")]
	[SerializeField] private int piecesToShowInAdvance = 3;

	private List<PieceType> types = new List<PieceType>();
	public PieceType currentType;

	public Action<PieceType, DialogueType> OnKillEnemy;

	public bool hasKilled = false;

	public void Init(Cell spawnCell)
	{
		Start();
		CurrentCell = spawnCell;
		transform.position = spawnCell.PiecePosition;

		board.OnStartPlayerTurn += AssignRandomType;
	}

	private void AssignRandomType()
	{
		if (IsAlive)
		{
			while (types.Count <= piecesToShowInAdvance)
			{
				types.Add((PieceType)Random.Range(0, Enum.GetNames(typeof(PieceType)).Length));
			}

			currentType = types.First();
			CurrentCell.ShowMovements(currentType, this);
			SetIcon(currentType);
			types.RemoveAt(0);
			UIManager.Instance.DisplayPieces(types);
		}
	}

	protected override void DoActionBeforeMoving(Cell cell)
	{
		base.DoActionBeforeMoving(cell);

		board.PlayerSelectedCell();
	}

	protected override void DoActionAfterMoving(Cell cell)
	{
		base.DoActionAfterMoving(cell);

		Enemy enemy = cell.TargetPresentOnCell<Enemy>();
		if (enemy != null)
		{
			hasKilled = true;
			OnKillEnemy?.Invoke(currentType, DialogueType.Attaque);
			enemy.Kill();
		}

		CurrentCell = cell;
		board.UnselectAllCells();

		if (CurrentCell.IsWin)
		{
			board.EndLevel(true);
		}
		else
		{
			board.EndTurnPlayer();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		board.OnStartPlayerTurn -= AssignRandomType;
	}
}
