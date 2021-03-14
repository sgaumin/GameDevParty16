using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
	[Header("Character Sprites")]
	[SerializeField] protected Sprite pionCharacterSprite;
	[SerializeField] protected Sprite fouCharacterSprite;
	[SerializeField] protected Sprite cavalierCharacterSprite;
	[SerializeField] protected Sprite tourCharacterSprite;

	[Header("Icon Sprites")]
	[SerializeField] protected Sprite pionSprite;
	[SerializeField] protected Sprite fouSprite;
	[SerializeField] protected Sprite cavalierSprite;
	[SerializeField] protected Sprite tourSprite;

	[Header("Animations")]
	[SerializeField] private CharacterAnimationData animationData;

	[Header("References")]
	[SerializeField] protected LayerMask cellMask;
	[SerializeField] protected Image icon;
	[SerializeField] protected Image character;

	protected Board board;
	private Coroutine moving;

	public bool IsAlive { get; set; }
	public bool HasFinishTurn { get; set; }
	public Cell CurrentCell { get; protected set; }

	protected virtual void Awake()
	{
		board = FindObjectOfType<Board>();
		board.OnStartPlayerTurn += ResetTurn;
	}

	protected virtual void Start()
	{
		IsAlive = true;
		transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
	}

	protected void ResetTurn()
	{
		HasFinishTurn = false;
	}

	public virtual void Kill()
	{
		IsAlive = false;
		Destroy(gameObject);
	}

	public void MoveToCell(Cell cell)
	{
		if (cell != null)
		{
			moving = StartCoroutine(MovePath(cell));
		}
	}

	private IEnumerator MovePath(Cell cell)
	{
		DoActionBeforeMoving(cell);

		float currentRaiseDuration = animationData.raiseDuration.RandomValue;
		float currentMoveDuration = animationData.moveDuration.RandomValue;

		// Raising phase
		//transform.DOLookAt(cell.PiecePosition, animationData.lookAtDuration.RandomValue).SetEase(animationData.lookAtEase);
		transform.DOMoveY(animationData.raiseHeightValue.RandomValue, currentRaiseDuration).SetRelative().SetEase(animationData.raiseEase);
		yield return new WaitForSeconds(currentRaiseDuration);

		// Move phase
		transform.DOMove(cell.PiecePosition, currentMoveDuration).SetEase(animationData.moveAnimation);
		yield return new WaitForSeconds(currentMoveDuration * animationData.cameraShakeAdjustmentTiming);
		Camera.main.DOShakePosition(animationData.cameraShakeDuration.RandomValue, animationData.cameraShakeStrenght.RandomValue, animationData.cameraShakeVibrato.RandomValue);
		yield return new WaitForSeconds(currentMoveDuration * (1 - animationData.cameraShakeAdjustmentTiming));

		DoActionAfterMoving(cell);
	}

	protected virtual void DoActionBeforeMoving(Cell cell) { }
	protected virtual void DoActionAfterMoving(Cell cell) { }

	protected void CheckCurrentCell()
	{
		Ray ray = new Ray(transform.position, -Vector3.up);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, cellMask))
		{
			CurrentCell = hit.collider.GetComponentInParent<Cell>();
		}
	}

	protected void SetIcon(PieceType type)
	{
		switch (type)
		{
			case PieceType.Pion:
				icon.sprite = pionSprite;
				character.sprite = pionCharacterSprite;
				break;
			case PieceType.Fou:
				icon.sprite = fouSprite;
				character.sprite = fouCharacterSprite;
				break;
			case PieceType.Cavalier:
				icon.sprite = cavalierSprite;
				character.sprite = cavalierCharacterSprite;
				break;
			case PieceType.Tour:
				icon.sprite = tourSprite;
				character.sprite = tourCharacterSprite;
				break;
		}
	}

	protected virtual void OnDestroy()
	{
		if (moving != null)
		{
			StopCoroutine(moving);
		}

		board.OnStartPlayerTurn -= ResetTurn;
	}
}
