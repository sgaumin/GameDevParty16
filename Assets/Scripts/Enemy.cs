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

	private void OnEnable()
	{
		CheckCurrentCell();
		SetIcon(type);
	}

	public void TryToAttackPlayer()
	{
		if (gameObject.activeSelf)
		{
			List<Player> targets = CurrentCell.GetTargetOnMovements<Player>(type, this);
			if (!targets.IsEmpty())
			{
				board.StopAutoDeletionRows();
				target = targets[0];
				MoveToCell(target.CurrentCell);
			}
			else
			{
				End();
			}
		}
	}

	public override void Kill()
	{
		board.Enemies.Remove(this);
		base.Kill();
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
		board.EndingTurn();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		board.OnEndPlayerTurn -= TryToAttackPlayer;
	}
}
