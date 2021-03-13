using DG.Tweening;
using System;
using System.Collections;
using Tools.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class PieceMouvment : MonoBehaviour
{
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

	private Board board;
	private Cell currentCell;
	private PieceType type;
	private Coroutine moving;

	public void Init(Cell spawnCell, Board board)
	{
		this.board = board;
		currentCell = spawnCell;
		transform.position = spawnCell.PiecePosition;
		AssignRandomType();
	}

	private void AssignRandomType()
	{
		type = (PieceType)Random.Range(0, Enum.GetNames(typeof(PieceType)).Length);
		currentCell.ShowMovements(type);
	}

	// Move cell by cell
	public void FolowPath(Cell cell)
	{
		moving = StartCoroutine(MovePath(cell));
	}

	private IEnumerator MovePath(Cell cell)
	{
		float currentLookAtDuration = lookAtDuration.RandomValue;
		float currentRaiseDuration = raiseDuration.RandomValue;
		float currentMoveDuration = moveDuration.RandomValue;

		transform.DOLookAt(cell.PiecePosition, lookAtDuration.RandomValue).SetEase(lookAtEase);
		transform.DOMoveY(raiseHeightValue.RandomValue, raiseDuration.RandomValue).SetRelative().SetEase(raiseEase);
		yield return new WaitForSeconds(Math.Max(currentLookAtDuration, currentRaiseDuration));


		transform.DOMove(cell.PiecePosition, currentMoveDuration).SetEase(moveAnimation);
		yield return new WaitForSeconds(currentMoveDuration);

		board.UnselectAllCells();
		currentCell = cell;
		AssignRandomType();
	}

	//// Move to one cell
	//public void PlaceAt(Cell cell)
	//{
	//	currentCell = cell;
	//	Transform cellTransform = cell.transform;
	//	Vector3 heightOffset = Vector3.zero;
	//	//heightOffset.y = height;

	//	StartCoroutine(MovePiece(cellTransform));
	//}

	//private IEnumerator MovePiece(Transform cellTransform)
	//{

	//	Tweener looker = transform.DOLookAt(cellTransform.position, raiseDuration, AxisConstraint.Y);
	//	yield return looker.WaitForCompletion();

	//	Vector3 heightOffset = Vector3.zero;
	//	//heightOffset.y = height;
	//	Tweener mover = transform.DOMove(cellTransform.position + heightOffset, moveDuration);
	//	yield return mover.WaitForCompletion();

	//	Tweener rotater = transform.DORotate(cellTransform.forward, moveDuration);
	//	yield return rotater.WaitForCompletion();


	//}
}
