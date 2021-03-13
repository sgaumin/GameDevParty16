using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
	private const float SPHERE_RADIUS = 0.2f;
	private const float CHECK_DISTANCE = 1f;

	[Header("References")]
	[SerializeField] private LayerMask cellMask;
	[SerializeField] private GameObject highlight;
	[SerializeField] private Material highlightMaterial;
	[SerializeField] private Material highlightSelectedMaterial;
	[SerializeField] private Material highlightCliquedMaterial;
	[SerializeField] private Transform piecePosition;

	private Board board;
	private CellState state;
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

	public PieceMouvment Piece { get; set; }
	public CellState State
	{
		get => state;

		set
		{
			state = value;
			switch (state)
			{
				case CellState.Unselected:
					highlight.gameObject.SetActive(false);
					break;
				case CellState.Highlighted:
					highlight.gameObject.SetActive(true);
					highlight.GetComponent<MeshRenderer>().material = highlightMaterial;
					break;
				case CellState.Selected:
					highlight.gameObject.SetActive(true);
					highlight.GetComponent<MeshRenderer>().material = highlightSelectedMaterial;
					break;
				case CellState.Cliqued:
					highlight.gameObject.SetActive(true);
					highlight.GetComponent<MeshRenderer>().material = highlightCliquedMaterial;
					board.OnlySelectCell(this);
					Piece.FolowPath(this);
					break;
			}
		}
	}

	public Vector3 PiecePosition => piecePosition.transform.position;

	protected void Start()
	{
		Init();
		State = CellState.Unselected;
	}

	private void Init()
	{
		board = GetComponentInParent<Board>();

		cellTop = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellDown = Physics.OverlapSphere(transform.position + new Vector3(0, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 0) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 0) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellTopLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellTopRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellDownLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellDownRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightTopLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightTopRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightRightTop = Physics.OverlapSphere(transform.position + new Vector3(2, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightRightDown = Physics.OverlapSphere(transform.position + new Vector3(2, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightDownLeft = Physics.OverlapSphere(transform.position + new Vector3(-2, 0, -1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightDownRight = Physics.OverlapSphere(transform.position + new Vector3(-2, 0, 1) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightLeftTop = Physics.OverlapSphere(transform.position + new Vector3(1, 0, -2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		cellKnightLeftDown = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, -2) * CHECK_DISTANCE, SPHERE_RADIUS, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();

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
	}

	public void ShowMovements(PieceType type)
	{
		List<List<Cell>> paths = new List<List<Cell>>();

		switch (type)
		{
			case PieceType.Pion:
				paths = GetPawnMovements();
				break;
			case PieceType.Fou:
				paths = GetBishopMovements();
				break;
			case PieceType.Cavalier:
				paths = GetKnightMovements();
				break;
			case PieceType.Tour:
				paths = GetRookMovements();
				break;
		}

		paths.Flatten().ForEach(x => x.State = CellState.Highlighted);
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

	private List<List<Cell>> GetPawnMovements()
	{
		List<List<Cell>> paths = new List<List<Cell>>();

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

	#region Context Menu Methods
	[ContextMenu("Show Pawn Movements")]
	private void ShowPawnMovements()
	{
		board.UnselectAllCells();
		GetPawnMovements().Flatten().ForEach(x => x.State = CellState.Highlighted);
	}

	[ContextMenu("Show Bishop Movements")]
	private void ShowBishopMovements()
	{
		board.UnselectAllCells();
		GetBishopMovements().Flatten().ForEach(x => x.State = CellState.Highlighted);
	}

	[ContextMenu("Show Rook Movements")]
	private void ShowRookMovements()
	{
		board.UnselectAllCells();
		GetRookMovements().Flatten().ForEach(x => x.State = CellState.Highlighted);
	}

	[ContextMenu("Show Knight Movements")]
	private void ShowKnightMovements()
	{
		board.UnselectAllCells();
		GetKnightMovements().Flatten().ForEach(x => x.State = CellState.Highlighted);
	}

	[ContextMenu("Show Neighbour")]
	private void ShowNearbyCells()
	{
		board.UnselectAllCells();
		nearbyCells.ForEach(x => x.State = CellState.Highlighted);
	}
	#endregion
}
