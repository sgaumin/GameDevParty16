using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
	[SerializeField] public PieceType type;

	private Character target;

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

	protected override void DoActionBeforeMoving(Cell cell)
    {
		base.DoActionBeforeMoving(cell);
		if(attackPlayer == false)
        {
			attackPlayer = true;
			playerKilledSound.instrument.Play();
			playerKilledSound.voice.Play();
		}
	}

	protected override void DoActionAfterMoving(Cell cell)
	{
		base.DoActionAfterMoving(cell);
		target.Kill();
		attackPlayer = false;
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
