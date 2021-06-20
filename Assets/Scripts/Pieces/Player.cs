using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : Character
{
	public Action<PieceType, DialogueType> OnKillEnemy;

	[Header("Config")]
	[SerializeField] private int piecesToShowInAdvance = 3;
	[SerializeField] private bool hasShieldAtStart;

	[Header("References")]
	[SerializeField] private Image[] piecesList = new Image[4];
	[SerializeField] private GameObject nextMoves;

	private List<PieceType> types = new List<PieceType>();
	private bool hasShield;

	public bool HasShield
	{
		get => hasShield;
		set
		{
			hasShield = value;
			if (shieldMask != null)
			{
				shieldMask.gameObject.SetActive(hasShield);
				shieldMask.GetComponent<Image>().sprite = character.sprite;
			}
			if (shieldVfx != null)
			{
				shieldVfx.transform?.DOKill();
				shieldVfx.transform.DOLocalMoveY(shieldVfx.transform.localPosition.y + shieldMovementY, shieldMovementDuration).SetLoops(-1, LoopType.Yoyo).SetEase(shieldMovementEase);
			}
		}
	}
	public PieceType CurrentType { get; private set; }
	public bool HasKilled { get; set; } = false;
	public PiecePoolType PiecePoolType { get; set; }

	public void Init(Cell spawnCell)
	{
		Start();
		CurrentCell = spawnCell;
		transform.position = spawnCell.CharacterPosition;

		board.OnStartPlayerTurn += AssignRandomType;
		board.OnRefreshBoard += RefreshMovements;

		HasShield = hasShieldAtStart;
	}

	private void AssignRandomType()
	{
		if (IsAlive)
		{
			while (types.Count <= piecesToShowInAdvance)
			{
				switch (PiecePoolType)
				{
					case PiecePoolType.All:
						types.Add((PieceType)Random.Range(0, Enum.GetNames(typeof(PieceType)).Length));
						break;
					case PiecePoolType.OnlyPawn:
						types.Add(PieceType.Pawn);
						break;
					case PiecePoolType.OnlyKnight:
						types.Add(PieceType.Knight);
						break;
				}
			}

			CurrentType = types.First();
			CurrentCell.ShowMovements(CurrentType, this);
			SetIcon(CurrentType);
			types.RemoveAt(0);
			DisplayPieces(types);
			UIManager.Instance.DisplayPortrait(CurrentType);
		}
	}

	public void ClearPiecePool()
	{
		types.Clear();
	}

	private void RefreshMovements()
	{
		if (board.BoardState == BoardStates.StartPlayerTurn)
		{
			CurrentCell.RefreshMovements();
		}
	}

	protected override void DoActionBeforeMoving(Cell cell)
	{
		base.DoActionBeforeMoving(cell);
		board.PlayerSelectedCell();
        if (CurrentCell.Freeze)
        {
			CurrentCell.StopFreeze();
        }
	}

	protected override void DoActionAfterMoving(Cell cell)
	{
		base.DoActionAfterMoving(cell);

		Enemy enemy = cell.TargetPresentOnCell<Enemy>();
		if (enemy != null)
		{
			board.EnemyKilled(enemy.type);
			HasKilled = true;
			OnKillEnemy?.Invoke(CurrentType, DialogueType.Attaque);
			enemy.Kill();
		}

		CurrentCell = cell;
		board.UnselectAllCells();
		if (CurrentCell.GiveShield && !HasShield)
		{
			HasShield = true;
			CurrentCell.GiveShield = false;
		}
        if (CurrentCell.Freeze)
        {
			CurrentCell.BeginFreeze();
        }
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
		board.OnRefreshBoard -= RefreshMovements;
	}

	public void DisplayPieces(List<PieceType> types)
	{
		bool ShowPieceQueue = UIManager.Instance.ShowPieceQueue;
		nextMoves.SetActive(ShowPieceQueue);
		if (!ShowPieceQueue)
			return;

		int i = 0;
		foreach (PieceType type in types)
		{
			piecesList[i++].sprite = UIManager.Instance.GetSpriteByType(type);
		}
	}
}
