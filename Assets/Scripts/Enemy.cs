using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
	[SerializeField] private PieceType type;

	private Character target;

	protected override void Awake()
	{
		base.Awake();
		board.OnEndPlayerTurn += TryToAttackPlayer;

	}

	protected override void Start()
	{
		base.Start();

		CheckCurrentCell();
		SetIcon(type);
	}

	public void TryToAttackPlayer()
	{
		List<Player> targets = CurrentCell.GetTargetOnMovements<Player>(type);
		if (!targets.IsEmpty())
		{
			target = targets[0];
			MoveToCell(target.CurrentCell);
		}
		else
		{
			End();
		}
	}

	protected override void DoActionAfterMoving(Cell cell)
	{
		base.DoActionAfterMoving(cell);
		target.Kill();
		board.EndLevel();

		End();
	}

	private void End()
	{
		HasFinishTurn = true;
		board.EndTurnEnemies();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		board.OnEndPlayerTurn -= TryToAttackPlayer;
	}
}
