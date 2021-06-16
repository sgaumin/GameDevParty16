using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Cell : MonoBehaviour
{
	private const float SPHERE_RADIUS = 0.45f;
	private const float CHECK_DISTANCE = 1f;

	[Header("Settings")]
	[SerializeField] private bool isWin;
	[SerializeField] private bool giveShield;
	[SerializeField] private bool canFall;
	[SerializeField] private MarkNames mark = MarkNames.None;
	[SerializeField] private bool eventExecutedOnlyOnce;
	[SerializeField] private UnityEvent cellEvent;

	[Header("References")]
	[SerializeField] private LayerMask cellMask;
	[SerializeField] private MeshRenderer model;
	[SerializeField] private GameObject highlight;
	[SerializeField] private Material highlightMaterial;
	[SerializeField] private Material highlightSelectedMaterial;
	[SerializeField] private Material highlightCliquedMaterial;
	[SerializeField] private Transform characterPosition;
	[SerializeField] private Transform effectPosition;
	[SerializeField] private GameObject winEffect;
	[SerializeField] private Material[] groundMaterials = new Material[2];

	[Header("Animations")]
	[SerializeField] private float timeBeforeRowDeletion = 1.4f;
	[SerializeField] private float fadDestroyingCellDuration = 0.09f;
	[SerializeField] private float offsetYDestroyingCell = -1.5f;

	private GameObject effect;
	private CellState state;
	private PieceType typeGiven;
	private Character characterGiven;
	private List<Cell> nearbyCells = new List<Cell>();
	private List<Tuple<Cell, CellPositionType>> diagonalsInfoCells = new List<Tuple<Cell, CellPositionType>>();
	private List<Tuple<Cell, CellPositionType>> linesInfoCells = new List<Tuple<Cell, CellPositionType>>();
	private List<Cell> knightCells = new List<Cell>();
	private Cell cellTop;
	private Cell cellDown;
	private Cell cellLeft;
	private Cell cellRight;
	private Cell cellTopLeft;
	private Cell cellTopRight;
	private Cell cellDownLeft;
	private Cell cellDownRight;
	private Cell cellKnightTopLeft;
	private Cell cellKnightTopRight;
	private Cell cellKnightRightTop;
	private Cell cellKnightRightDown;
	private Cell cellKnightDownLeft;
	private Cell cellKnightDownRight;
	private Cell cellKnightLeftTop;
	private Cell cellKnightLeftDown;
	private Enemy enemyOnTop;
	private Coroutine falling;

	public UnityEvent CellEvent => cellEvent;
	public MeshRenderer Model => model;
	public GameObject Highlight => highlight;
	public bool GiveShield => giveShield;
	public bool IsWin => isWin;
	public Player Piece { get; set; }
	public MarkNames Mark
	{
		get => mark;
		set
		{
			mark = value;
		}
	}
	public bool CanFall => canFall;
	public CellState State
	{
		get => state;
		set
		{
			if (LevelController.Instance.LevelBoard.CanReceiveInput)
			{
				state = value;
				switch (state)
				{
					case CellState.Unselected:
						if (highlight != null)
						{
							highlight.gameObject.SetActive(false);
						}
						if (enemyOnTop != null)
						{
							enemyOnTop.IsHighlighted = false;
						}
						break;
					case CellState.Highlighted:
						if (highlight != null)
						{
							highlight.gameObject.SetActive(true);
							highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
						}
						if (enemyOnTop != null)
						{
							enemyOnTop.IsHighlighted = true;
						}
						break;
					case CellState.Selected:
						if (highlight != null)
						{
							highlight.gameObject.SetActive(true);
							highlight.GetComponent<MeshRenderer>().material = highlightSelectedMaterial;
						}
						if (enemyOnTop != null)
						{
							enemyOnTop.IsHighlighted = false;
						}
						break;
					case CellState.Cliqued:
						if (highlight != null)
						{
							highlight.gameObject.SetActive(true);
							highlight.GetComponent<MeshRenderer>().material = highlightCliquedMaterial;
						}
						if (enemyOnTop != null)
						{
							enemyOnTop.IsHighlighted = false;
						}
						LevelController.Instance.LevelBoard.OnlySelectCell(this);
						Piece.MoveToCell(this);
						break;
					case CellState.Inactive:
						if (highlight != null)
						{
							highlight.gameObject.SetActive(false);
						}
						if (enemyOnTop != null)
						{
							enemyOnTop.IsHighlighted = false;
						}
						break;
				}
			}
		}
	}

	public Vector3 CharacterPosition => characterPosition.transform.position;
	public Vector3 EffectPosition => effectPosition.transform.position;

	protected void Awake()
	{
		if (eventExecutedOnlyOnce)
		{
			cellEvent.AddListener(() => cellEvent = null);
		}
	}

	public void Init()
	{
		DefineCellLinks();
		State = CellState.Unselected;

		if (isWin)
		{
			model.material = groundMaterials[2];
			effect = Instantiate(winEffect);
			effect.transform.position = EffectPosition;
			effect.transform.SetParent(transform);
		}
		else if (mark != MarkNames.None && LevelController.Instance.LevelBoard.ShowMarkers)
		{
			model.material = LevelController.Instance.LevelBoard.Marks.Where(x => x.Name == mark).FirstOrDefault().Material;
		}
		else
		{
			model.material = (transform.localPosition.x + transform.localPosition.z) % 2 == 0 ? groundMaterials[0] : groundMaterials[1];
		}
	}

	public void DefineCellLinks()
	{
		nearbyCells.Clear();
		knightCells.Clear();

		cellTop = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellTop != null && cellTop.state == CellState.Inactive) cellTop = null;
		cellDown = Physics.OverlapSphere(transform.position + new Vector3(0, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellDown != null && cellDown.state == CellState.Inactive) cellDown = null;
		cellLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 0) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellLeft != null && cellLeft.state == CellState.Inactive) cellLeft = null;
		cellRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 0) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellRight != null && cellRight.state == CellState.Inactive) cellRight = null;
		cellTopLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellTopLeft != null && cellTopLeft.state == CellState.Inactive) cellTopLeft = null;
		cellTopRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellTopRight != null && cellTopRight.state == CellState.Inactive) cellTopRight = null;
		cellDownLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellDownLeft != null && cellDownLeft.state == CellState.Inactive) cellDownLeft = null;
		cellDownRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellDownRight != null && cellDownRight.state == CellState.Inactive) cellDownRight = null;
		cellKnightTopLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightTopLeft != null && cellKnightTopLeft.state == CellState.Inactive) cellKnightTopLeft = null;
		cellKnightTopRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightTopRight != null && cellKnightTopRight.state == CellState.Inactive) cellKnightTopRight = null;
		cellKnightRightTop = Physics.OverlapSphere(transform.position + new Vector3(2, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightRightTop != null && cellKnightRightTop.state == CellState.Inactive) cellKnightRightTop = null;
		cellKnightRightDown = Physics.OverlapSphere(transform.position + new Vector3(2, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightRightDown != null && cellKnightRightDown.state == CellState.Inactive) cellKnightRightDown = null;
		cellKnightDownLeft = Physics.OverlapSphere(transform.position + new Vector3(-2, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightDownLeft != null && cellKnightDownLeft.state == CellState.Inactive) cellKnightDownLeft = null;
		cellKnightDownRight = Physics.OverlapSphere(transform.position + new Vector3(-2, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightDownRight != null && cellKnightDownRight.state == CellState.Inactive) cellKnightDownRight = null;
		cellKnightLeftTop = Physics.OverlapSphere(transform.position + new Vector3(1, 0, -2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightLeftTop != null && cellKnightLeftTop.state == CellState.Inactive) cellKnightLeftTop = null;
		cellKnightLeftDown = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, -2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		if (cellKnightLeftDown != null && cellKnightLeftDown.state == CellState.Inactive) cellKnightLeftDown = null;

		if (cellTop != null)
		{
			nearbyCells.Add(cellTop);
			linesInfoCells.Add(Tuple.Create(cellTop, CellPositionType.Top));
		}
		if (cellDown != null)
		{
			nearbyCells.Add(cellDown);
			linesInfoCells.Add(Tuple.Create(cellDown, CellPositionType.Down));
		}
		if (cellLeft != null)
		{
			nearbyCells.Add(cellLeft);
			linesInfoCells.Add(Tuple.Create(cellLeft, CellPositionType.Left));
		}
		if (cellRight != null)
		{
			nearbyCells.Add(cellRight);
			linesInfoCells.Add(Tuple.Create(cellRight, CellPositionType.Right));
		}
		if (cellTopLeft != null)
		{
			nearbyCells.Add(cellTopLeft);
			diagonalsInfoCells.Add(Tuple.Create(cellTopLeft, CellPositionType.TopLeft));
		}
		if (cellTopRight != null)
		{
			nearbyCells.Add(cellTopRight);
			diagonalsInfoCells.Add(Tuple.Create(cellTopRight, CellPositionType.TopRight));
		}
		if (cellDownLeft != null)
		{
			nearbyCells.Add(cellDownLeft);
			diagonalsInfoCells.Add(Tuple.Create(cellDownLeft, CellPositionType.DownLeft));
		}
		if (cellDownRight != null)
		{
			nearbyCells.Add(cellDownRight);
			diagonalsInfoCells.Add(Tuple.Create(cellDownRight, CellPositionType.DownRight));
		}
		if (cellKnightTopLeft != null)
		{
			knightCells.Add(cellKnightTopLeft);
		}
		if (cellKnightTopRight != null)
		{
			knightCells.Add(cellKnightTopRight);
		}
		if (cellKnightRightTop != null)
		{
			knightCells.Add(cellKnightRightTop);
		}
		if (cellKnightRightDown != null)
		{
			knightCells.Add(cellKnightRightDown);
		}
		if (cellKnightDownLeft != null)
		{
			knightCells.Add(cellKnightDownLeft);
		}
		if (cellKnightDownRight != null)
		{
			knightCells.Add(cellKnightDownRight);
		}
		if (cellKnightLeftTop != null)
		{
			knightCells.Add(cellKnightLeftTop);
		}
		if (cellKnightLeftDown != null)
		{
			knightCells.Add(cellKnightLeftDown);
		}

		enemyOnTop = TargetPresentOnCell<Enemy>();
	}

	public void ShowMovements(PieceType type, Character character)
	{
		List<List<Cell>> paths = new List<List<Cell>>();

		typeGiven = type;
		characterGiven = character;

		switch (type)
		{
			case PieceType.Pawn:
				paths = GetPawnMovements(character);
				break;
			case PieceType.Bishop:
				paths = GetBishopMovements();
				break;
			case PieceType.Knight:
				paths = GetKnightMovements();
				break;
			case PieceType.Rook:
				paths = GetRookMovements();
				break;
		}

		paths.Flatten().Distinct().WithoutNullValues().ForEach(x => x.State = CellState.Highlighted);
	}

	public void RefreshMovements()
	{
		List<List<Cell>> paths = new List<List<Cell>>();

		switch (typeGiven)
		{
			case PieceType.Pawn:
				paths = GetPawnMovements(characterGiven);
				break;
			case PieceType.Bishop:
				paths = GetBishopMovements();
				break;
			case PieceType.Knight:
				paths = GetKnightMovements();
				break;
			case PieceType.Rook:
				paths = GetRookMovements();
				break;
		}

		paths.Flatten().Distinct().WithoutNullValues().ForEach(x => x.State = CellState.Highlighted);
	}

	public List<T> GetTargetOnMovements<T>(PieceType type, Character character) where T : Character
	{
		List<T> targets = new List<T>();
		List<List<Cell>> movements = GetMovements(type, character);
		foreach (Cell cell in movements.Flatten().ToList())
		{
			if (cell.TargetPresentOnCell<T>() != null)
			{
				targets.Add(cell.TargetPresentOnCell<T>());
			}
		}

		return targets;
	}

	public T TargetPresentOnCell<T>() where T : Character
	{
		RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector2.up, Mathf.Infinity);
		foreach (RaycastHit hit in hits)
		{
			if (hit.collider.GetComponentInParent<T>() != null)
			{
				return hit.collider.GetComponentInParent<T>();
			}
		}

		return null;
	}

	private List<List<Cell>> GetMovements(PieceType type, Character character)
	{
		List<List<Cell>> paths = new List<List<Cell>>();

		switch (type)
		{
			case PieceType.Pawn:
				paths = GetPawnMovements(character);
				break;
			case PieceType.Bishop:
				paths = GetBishopMovements();
				break;
			case PieceType.Knight:
				paths = GetKnightMovements();
				break;
			case PieceType.Rook:
				paths = GetRookMovements();
				break;
		}

		return paths;
	}

	private List<Cell> GetRecursivePositionCells(List<Cell> path, CellPositionType position, int? recursionLimit = null, int? currentRecursion = null)
	{
		if (recursionLimit != null)
		{
			if (currentRecursion == null)
			{
				currentRecursion = 0;
			}

			currentRecursion++;

			if (currentRecursion >= recursionLimit)
			{
				return path;
			}
		}

		Cell targetCell = GetCell(position);
		if (targetCell != null)
		{
			path.Add(targetCell);
			targetCell.GetRecursivePositionCells(path, position, recursionLimit, currentRecursion);
		}

		return path;
	}

	private Cell GetCell(CellPositionType position)
	{
		Cell cell = null;

		switch (position)
		{
			case CellPositionType.Top:
				cell = cellTop;
				break;
			case CellPositionType.Down:
				cell = cellDown;
				break;
			case CellPositionType.Left:
				cell = cellLeft;
				break;
			case CellPositionType.Right:
				cell = cellRight;
				break;
			case CellPositionType.TopLeft:
				cell = cellTopLeft;
				break;
			case CellPositionType.TopRight:
				cell = cellTopRight;
				break;
			case CellPositionType.DownLeft:
				cell = cellDownLeft;
				break;
			case CellPositionType.DownRight:
				cell = cellDownRight;
				break;
		}

		return cell;
	}

	private List<List<Cell>> GetPawnMovements(Character character)
	{
		List<List<Cell>> paths = new List<List<Cell>>();

		if (character is Player)
		{
			foreach (Tuple<Cell, CellPositionType> cell in linesInfoCells)
			{
				List<Cell> path = new List<Cell>();
				if (cell.Item1 != null)
				{
					path.Add(cell.Item1);
					cell.Item1.GetRecursivePositionCells(path, cell.Item2, 2);
				}
				paths.Add(path);
			}

			foreach (Tuple<Cell, CellPositionType> cell in diagonalsInfoCells)
			{
				List<Cell> path = new List<Cell>();
				if (cell.Item1 != null)
				{
					path.Add(cell.Item1);
					cell.Item1.GetRecursivePositionCells(path, cell.Item2, 1);
				}
				paths.Add(path);
			}
		}
		else if (character is Enemy)
		{
			foreach (Tuple<Cell, CellPositionType> cell in diagonalsInfoCells)
			{
				List<Cell> path = new List<Cell>();
				if (cell.Item1 != null)
				{
					path.Add(cell.Item1);
					cell.Item1.GetRecursivePositionCells(path, cell.Item2, 1);
				}
				paths.Add(path);
			}
		}

		return paths;
	}

	private List<List<Cell>> GetBishopMovements()
	{
		List<List<Cell>> paths = new List<List<Cell>>();
		foreach (Tuple<Cell, CellPositionType> cell in diagonalsInfoCells)
		{
			List<Cell> path = new List<Cell>();
			if (cell.Item1 != null)
			{
				path.Add(cell.Item1);
				cell.Item1.GetRecursivePositionCells(path, cell.Item2);
			}
			paths.Add(path);
		}

		return paths;
	}

	private List<List<Cell>> GetKnightMovements()
	{
		List<List<Cell>> paths = new List<List<Cell>>();
		foreach (Cell cell in knightCells)
		{
			List<Cell> path = new List<Cell>();
			path.Add(cell);
			paths.Add(path);
		}

		return paths;
	}

	private List<List<Cell>> GetRookMovements()
	{
		List<List<Cell>> paths = new List<List<Cell>>();
		foreach (Tuple<Cell, CellPositionType> cell in linesInfoCells)
		{
			List<Cell> path = new List<Cell>();
			if (cell.Item1 != null)
			{
				path.Add(cell.Item1);
				cell.Item1.GetRecursivePositionCells(path, cell.Item2);
			}
			paths.Add(path);
		}

		return paths;
	}

	public void MakeCellFall()
	{
		this.falling = StartCoroutine(CellFalling());
	}

	private IEnumerator CellFalling()
	{
		yield return new WaitForSeconds(timeBeforeRowDeletion);
		gameObject.SetActive(true);
		Highlight.SetActive(false);
		Model.material.DOColor(Model.material.color.WithAlpha(0f), fadDestroyingCellDuration).SetEase(Ease.OutCubic);
		transform.DOMoveY(-offsetYDestroyingCell, fadDestroyingCellDuration).SetRelative().SetEase(Ease.OutBack);
		yield return new WaitForSeconds(fadDestroyingCellDuration);
	}

	private void OnDestroy()
	{
		if (falling != null)
		{
			StopCoroutine(falling);
		}
	}
}
