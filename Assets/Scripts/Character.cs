using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
	[Header("Character Sprites")]
	[SerializeField] protected Sprite pionCharacterSprite;
	[SerializeField] private Sprite pionCharacterhighlightSprite;
	[SerializeField] protected Sprite fouCharacterSprite;
	[SerializeField] protected Sprite fouCharacterhighlightSprite;
	[SerializeField] protected Sprite cavalierCharacterSprite;
	[SerializeField] protected Sprite cavalierCharacterhighlightSprite;
	[SerializeField] protected Sprite tourCharacterSprite;
	[SerializeField] protected Sprite tourCharacterhighlightSprite;

	[Header("Icon Sprites")]
	[SerializeField] protected Sprite pionSprite;
	[SerializeField] protected Sprite fouSprite;
	[SerializeField] protected Sprite cavalierSprite;
	[SerializeField] protected Sprite tourSprite;

	[Header("Animations")]
	[SerializeField] private CharacterAnimationData animationData;
	[Space]
	[SerializeField] protected float shieldMovementY;
	[SerializeField] protected float shieldMovementDuration;
	[SerializeField] protected Ease shieldMovementEase;

	[Header("Effects")]
	[SerializeField] private Color effectColor;
	[SerializeField] private ParticleSystem leavingCellEffect;
	[SerializeField] private ParticleSystem arrivingOnCellEffect;
	[SerializeField] private ParticleSystem attackingEffect;

	[Header("References")]
	[SerializeField] protected LayerMask cellMask;
	[SerializeField] protected Image icon;
	[SerializeField] protected Image character;
	[SerializeField] protected Image shieldVfx;
	[SerializeField] protected Mask shieldMask;
	[SerializeField] protected Image shieldImageMask;

	protected Board board;
	private Coroutine moving;
	private Sprite currentSprite;
	private Sprite highlightSprite;
	private bool isHighlighted;

	public bool IsAlive { get; set; }
	public bool HasFinishTurn { get; set; }
	public Cell CurrentCell { get; protected set; }
	public bool IsHighlighted
	{
		get => isHighlighted;
		set
		{
			isHighlighted = value;
			character.sprite = isHighlighted ? highlightSprite : currentSprite;
		}
	}
	public PieceType CurrentType { get; protected set; }

	protected virtual void Awake()
	{
		board = FindObjectOfType<Board>();
		board.OnStartPlayerTurn += ResetTurn;
	}

	protected virtual void Start()
	{
		IsAlive = true;

		character.transform.DOScaleY(transform.localScale.y * animationData.strechYFactor.RandomValue, animationData.strechYFrequency.RandomValue).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);
	}

	protected void ResetTurn()
	{
		HasFinishTurn = false;
	}

	public void ShowMovements()
	{
		CurrentCell.ShowMovements(CurrentType, this);
	}

	public virtual void Kill()
	{
		IsAlive = false;

		ParticleSystem currentAttackEffect = Instantiate(attackingEffect);
		currentAttackEffect.transform.position = CurrentCell.CharacterPosition;

		Camera.main.DOShakePosition(animationData.cameraShakeDuration.RandomValue, animationData.cameraShakeStrenght.RandomValue, animationData.cameraShakeVibrato.RandomValue);

		if (this is Player)
		{
#if ENABLE_CLOUD_SERVICES_ANALYTICS
			string customEventName = "PlayerKilled";
			AnalyticsResult ar = Analytics.CustomEvent(customEventName, new Dictionary<string, object>
			{
				{ "Level", board.name}
			});
			Debug.Log($"Analytics {customEventName}: {ar}");
#endif
		}

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

		ParticleSystem leavingEffect = Instantiate(leavingCellEffect);
		//var main = leavingEffect.main;
		//main.startColor = effectColor;
		leavingEffect.transform.position = CurrentCell.EffectPosition;

		float currentRaiseDuration = animationData.raiseDuration.RandomValue;
		float currentMoveDuration = animationData.moveDuration.RandomValue;

		// Raising phase
		//transform.DOLookAt(cell.PiecePosition, animationData.lookAtDuration.RandomValue).SetEase(animationData.lookAtEase);
		transform.DOMoveY(animationData.raiseHeightValue.RandomValue, currentRaiseDuration).SetRelative().SetEase(animationData.raiseEase);
		yield return new WaitForSeconds(currentRaiseDuration);

		// Move phase
		transform.DOMove(cell.CharacterPosition, currentMoveDuration).SetEase(animationData.moveAnimation);
		yield return new WaitForSeconds(currentMoveDuration * animationData.cameraShakeAdjustmentTiming);
		Camera.main.DOShakePosition(animationData.cameraShakeDuration.RandomValue, animationData.cameraShakeStrenght.RandomValue, animationData.cameraShakeVibrato.RandomValue);
		yield return new WaitForSeconds(currentMoveDuration * (1 - animationData.cameraShakeAdjustmentTiming));

		ParticleSystem arrivingEffect = Instantiate(arrivingOnCellEffect);
		//main = arrivingEffect.main;
		//main.startColor = effectColor;
		arrivingEffect.transform.position = cell.EffectPosition;

		DoActionAfterMoving(cell);
	}

	protected virtual void DoActionBeforeMoving(Cell cell)
	{
		if (CurrentCell.CanFall)
		{
			CurrentCell.MakeCellFall();
		}
	}

	protected virtual void DoActionAfterMoving(Cell cell)
	{
		if (CurrentCell.CanFall)
		{
			Destroy(CurrentCell.gameObject);
			board.ResetBoard();
		}
	}

	public void CheckCurrentCell()
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
			case PieceType.Pawn:
				icon.sprite = pionSprite;
				character.sprite = pionCharacterSprite;
				shieldImageMask.sprite = pionCharacterSprite;
				highlightSprite = pionCharacterhighlightSprite;
				break;

			case PieceType.Bishop:
				icon.sprite = fouSprite;
				character.sprite = fouCharacterSprite;
				shieldImageMask.sprite = fouCharacterSprite;
				highlightSprite = fouCharacterhighlightSprite;
				break;

			case PieceType.Knight:
				icon.sprite = cavalierSprite;
				character.sprite = cavalierCharacterSprite;
				shieldImageMask.sprite = cavalierCharacterSprite;
				highlightSprite = cavalierCharacterhighlightSprite;
				break;

			case PieceType.Rook:
				icon.sprite = tourSprite;
				character.sprite = tourCharacterSprite;
				shieldImageMask.sprite = tourCharacterSprite;
				highlightSprite = tourCharacterhighlightSprite;
				break;
		}

		currentSprite = character.sprite;
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