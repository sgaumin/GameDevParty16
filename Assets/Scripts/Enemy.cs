using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
	[SerializeField] public PieceType type;

	private Player playerTarget;

	public static bool attackPlayer = false;

	public SoundData playerKilledSound;

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
				playerTarget = targets[0];
                if (!playerTarget.HasShield)
                {
					board.StopAutoDeletionRows();
                }
				MoveToCell(playerTarget.CurrentCell);
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

	protected override void DoActionBeforeMoving(Cell cell)
	{
		base.DoActionBeforeMoving(cell);
		if (attackPlayer == false)
		{
			attackPlayer = true;
			playerKilledSound.instrument.Play();
			playerKilledSound.voice.Play();
		}
	}

	protected override void DoActionAfterMoving(Cell cell)
	{
		base.DoActionAfterMoving(cell);

		if (playerTarget.HasShield)
		{
			playerTarget.HasShield = false;
			Kill();
		}
		else
		{
			playerTarget.Kill();
			attackPlayer = false;
			board.EndLevel();
		}

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
