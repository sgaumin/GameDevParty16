using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : Character
{
	[Header("Config")]
	[SerializeField] private int piecesToShowInAdvance = 3;

	[Header("Animations")]
	[SerializeField, FloatRangeSlider(0f, 2f)] private FloatRange lookAtDuration = new FloatRange(0.8f, 1.2f);
	public Ease lookAtEase = Ease.OutSine;
	[Space]
	[SerializeField, FloatRangeSlider(0f, 5f)] private FloatRange raiseHeightValue = new FloatRange(1f, 1.5f);
	[SerializeField, FloatRangeSlider(0f, 2f)] private FloatRange raiseDuration = new FloatRange(0.8f, 1.2f);
	public Ease raiseEase = Ease.OutBounce;
	[Space]
	[SerializeField, FloatRangeSlider(0f, 2f)] private FloatRange moveDuration = new FloatRange(1f, 1.5f);
	public AnimationCurve moveAnimation;
	[Space]
	[SerializeField, FloatRangeSlider(0f, 1f)] private FloatRange cameraShakeDuration = new FloatRange(0.25f, 0.35f);
	[SerializeField, FloatRangeSlider(0f, 10f)] private FloatRange cameraShakeStrenght = new FloatRange(2.6f, 3.2f);
	[SerializeField, IntRangeSlider(5, 15)] private IntRange cameraShakeVibrato = new IntRange(9, 11);
	[SerializeField] private float cameraShakeAdjustmentTiming = 0.8f;

	private List<PieceType> types = new List<PieceType>();
	private Coroutine moving;

	public void Init(Cell spawnCell, Board board)
	{
		Start();

		this.board = board;
		currentCell = spawnCell;
		transform.position = spawnCell.PiecePosition;
		AssignRandomType();
	}

	private void AssignRandomType()
	{
		if (isAlive)
		{
			while (types.Count <= piecesToShowInAdvance)
			{
				types.Add((PieceType)Random.Range(0, Enum.GetNames(typeof(PieceType)).Length));
			}

			currentCell.ShowMovements(types.First());
			SetIcon(types.First());

			types.RemoveAt(0);
			UIManager.Instance.DisplayPieces(types);
		}
	}

	// Move cell by cell
	public void FolowPath(Cell cell)
	{
		moving = StartCoroutine(MovePath(cell));
	}

	private IEnumerator MovePath(Cell cell)
	{
		float currentRaiseDuration = raiseDuration.RandomValue;
		float currentMoveDuration = moveDuration.RandomValue;

		transform.DOLookAt(cell.PiecePosition, lookAtDuration.RandomValue).SetEase(lookAtEase);
		transform.DOMoveY(raiseHeightValue.RandomValue, currentRaiseDuration).SetRelative().SetEase(raiseEase);
		yield return new WaitForSeconds(currentRaiseDuration);

		transform.DOMove(cell.PiecePosition, currentMoveDuration).SetEase(moveAnimation);
		yield return new WaitForSeconds(currentMoveDuration * cameraShakeAdjustmentTiming);
		Camera.main.DOShakePosition(cameraShakeDuration.RandomValue, cameraShakeStrenght.RandomValue, cameraShakeVibrato.RandomValue);
		yield return new WaitForSeconds(currentMoveDuration * (1 - cameraShakeAdjustmentTiming));

		Enemy enemy = cell.TargetPresentOnCell<Enemy>();
		if (enemy != null)
		{
			enemy.Kill();
		}

		board.UnselectAllCells();
		board.EndTurn();
		currentCell = cell;
		AssignRandomType();
	}

	private void OnDestroy()
	{
		if (moving != null)
		{
			StopCoroutine(moving);
		}
	}
}
