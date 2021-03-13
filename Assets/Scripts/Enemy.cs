using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
	[SerializeField] private PieceType type;

	protected override void Start()
	{
		base.Start();

		CheckCurrentCell();
		board = FindObjectOfType<Board>();
		SetIcon(type);
	}

	public void TryToAttackPlayer()
	{
		List<Player> targets = currentCell.GetTargetOnMovements<Player>(type);
		if (!targets.IsEmpty())
		{
			targets[0].Kill();
			board.EndLevel();
		}
	}
}
