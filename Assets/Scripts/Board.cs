using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.Utils;
using UnityEngine;

public class Board : MonoBehaviour
{
	[Header("Rules")]
	[SerializeField] private int timerCount = 10;
	[SerializeField] private int rowStartDisplayCount = 8;
	[SerializeField, IntRangeSlider(0, 10)] private IntRange rowDeletionAmount = new IntRange(1, 3);
	[SerializeField] private float timeBeforeRowDeletion = 1f;
	[SerializeField] private Cell spawnCell;

	[Header("Animations")]
	[SerializeField] private float fadDestroyingCellDuration = 0.01f;
	[SerializeField] private float offsetYDestroyingCell = -2f;

	[Header("References")]
	[SerializeField] private DialoguesController dialoguesController;
	[SerializeField] private Player piecePrefab;
	[SerializeField] private Transform cellHolder;
	[SerializeField] private Transform cameraTarget;

	private List<Cell> cells = new List<Cell>();
	private List<List<Cell>> rowCells = new List<List<Cell>>();
	private List<Enemy> enemies = new List<Enemy>();
	public Player player;
	private GameObject o;
	private bool hasFinishTurn;
	private Coroutine rowDeletion;
	private BoardStates boardState;
	private bool inverseAnimation;
	private float positionPercentage;
	private float currentRowDeletionAmount;

	public Action OnStartPlayerTurn;
	public Action OnPlayerSelectedCell;
	public Action OnEndPlayerTurn;
	public Action OnEndLevel;
	public Action OnRefreshBoard;

	public List<Enemy> Enemies
	{
		get => enemies;
		set
		{
			enemies = value;
		}
	}

	public BoardStates BoardState
	{
		get => boardState;
		set
		{
			boardState = value;

			switch (value)
			{
				case BoardStates.StartPlayerTurn:
					OnStartPlayerTurn?.Invoke();
					break;
				case BoardStates.PlayerSelecCell:
					OnPlayerSelectedCell?.Invoke();
					break;
				case BoardStates.EndPlayerTurn:
					OnEndPlayerTurn?.Invoke();
					break;
				case BoardStates.EndLevel:
					OnEndLevel?.Invoke();
					break;
			}
		}
	}

	protected void Awake()
	{
		OnEndLevel += StopAutoDeletionRows;
	}

	void Start()
	{
		cells = GetComponentsInChildren<Cell>().ToList();
		ReorganizeCells();
		cells.ForEach(x => x.Init());

		enemies = FindObjectsOfType<Enemy>().ToList();
		EnemyActivationCheck();

		SpawnPiece();
		cells.ForEach(x => x.Piece = player);

		UIManager.Instance.StartTimer(timerCount);
		StartAutoDeletionRows();

		BoardState = BoardStates.StartPlayerTurn;
	}

	private void Update()
	{
		if (player != null)
		{
			cameraTarget.transform.position = player.transform.position;
		}
	}

	private void EnemyActivationCheck()
	{
		enemies.ForEach(x => x.gameObject.SetActive(x.CurrentCell.gameObject.activeSelf));
	}

	private void ReorganizeCells()
	{
		int zMin = Mathf.FloorToInt(cells.Min(x => x.transform.position.z));
		int zMax = Mathf.FloorToInt(cells.Max(x => x.transform.position.z));
		int rowCount = 0;

		for (int i = zMin; i <= zMax; i++)
		{
			o = new GameObject();
			o.name = $"Row {rowCount++}";
			o.transform.SetParent(cellHolder);

			List<Cell> row = new List<Cell>();
			foreach (Cell cell in cells.Where(x => x.transform.position.z == i).OrderBy(x => x.transform.position.z).ThenBy(x => x.transform.position.x).ToList())
			{
				cell.transform.SetParent(o.transform);
				row.Add(cell);
			}

			rowCells.Add(row);
		}

		rowCells.ForEach(x => x.ForEach(y => y.gameObject.SetActive(false)));
		for (int i = 0; i < rowStartDisplayCount; i++)
		{
			rowCells[i].ForEach(x => x.gameObject.SetActive(true));
		}
	}

	public void StartAutoDeletionRows()
	{
		rowDeletion = StartCoroutine(AutoDeletionRowsCore());
	}

	public void StopAutoDeletionRows()
	{
		if (rowDeletion != null)
		{
			StopCoroutine(rowDeletion);
		}
	}

	private IEnumerator AutoDeletionRowsCore()
	{
		while (true)
		{
			yield return new WaitForSeconds(timeBeforeRowDeletion);
			CheckRowDeletion();
		}
	}

	private void ResetBoard()
	{
		cells.ForEach(x => x.DefineCellLinks());
		EnemyActivationCheck();
		OnRefreshBoard?.Invoke();
	}

	private void CheckRowDeletion()
	{
		CheckPlayerPositionPercentage();
		DeleteFirstRow();

		for (int i = 0; i < currentRowDeletionAmount; i++)
		{
			inverseAnimation = !inverseAnimation;
			ShowNewRow(0);
			ResetBoard();
		}
	}

	private void CheckPlayerPositionPercentage()
	{
		int zMin = Mathf.FloorToInt(cells.Min(x => x.transform.position.z));
		int zMax = Mathf.FloorToInt(cells.Max(x => x.transform.position.z));

		positionPercentage = (player.transform.position.z - zMin) / (zMax - zMin);
		currentRowDeletionAmount = Mathf.FloorToInt((rowDeletionAmount.Max - rowDeletionAmount.Min) * positionPercentage + rowDeletionAmount.Min);
	}

	private void DeleteFirstRow()
	{
		List<Cell> firstRow = rowCells.First();
		rowCells.RemoveAt(0);

		foreach (Cell cell in firstRow)
		{
			cell.State = CellState.Inactive;
		}

		firstRow.ForEach(x => cells.Remove(x));
		StartCoroutine(RowAnimation(firstRow, true));
	}

	private IEnumerator RowAnimation(List<Cell> row, bool isFirstRow)
	{
		if (inverseAnimation)
		{
			row.Reverse();
		}

		foreach (Cell cell in row)
		{
			cell.gameObject.SetActive(true);
			cell.Highlight.SetActive(false);

			if (!isFirstRow)
			{
				cell.Model.material.color = cell.Model.material.color.WithAlpha(0f);
				cell.transform.position = cell.transform.position.plusY(offsetYDestroyingCell);
			}

			cell.Model.material.DOColor(cell.Model.material.color.WithAlpha(isFirstRow ? 0f : 1f), fadDestroyingCellDuration).SetEase(Ease.OutCubic);
			cell.transform.DOMoveY(-offsetYDestroyingCell, fadDestroyingCellDuration).SetRelative().SetEase(Ease.OutBack);

			if (isFirstRow)
			{
				Character currentCharacter = cell.TargetPresentOnCell<Character>();
				if (currentCharacter != null)
				{
					currentCharacter.Kill();
					if (currentCharacter is Player)
					{
						EndLevel();
					}
				}
			}

			yield return new WaitForSeconds(fadDestroyingCellDuration);
		}

		if (isFirstRow)
		{
			row.ForEach(x => Destroy(x.gameObject));
		}

		ResetBoard();
	}

	private void ShowNewRow(int index)
	{
		if (index < rowCells.Count())
		{
			List<Cell> lastRow = rowCells[index];
			if (lastRow.First().gameObject.activeSelf)
			{
				ShowNewRow(index + 1);
			}
			else
			{
				StartCoroutine(RowAnimation(rowCells[index], false));
			}
		}
	}

	private void SpawnPiece()
	{
		player = Instantiate(piecePrefab, transform);
		player.Init(spawnCell);
		dialoguesController.Init();
	}

	public void PlayerSelectedCell()
	{
		BoardState = BoardStates.PlayerSelecCell;
	}

	public void EndTurnPlayer()
	{
		hasFinishTurn = false;
		BoardState = BoardStates.EndPlayerTurn;

		if (enemies.IsEmpty())
		{
			EndingTurn();
		}
	}

	public void EndingTurn()
	{
		if (BoardState != BoardStates.EndLevel && !hasFinishTurn)
		{
			if (enemies.IsEmpty() || enemies.Where(x => x.gameObject.activeSelf).All(x => x.HasFinishTurn))
			{
				hasFinishTurn = true;
				BoardState = BoardStates.StartPlayerTurn;
			}
		}
	}

	public void EndLevel(bool hasWon = false)
	{
		BoardState = BoardStates.EndLevel;
		UnselectAllCells();
		if (!hasWon)
		{
			UIManager.Instance.DisplayGameOver();
		}
		else
		{
			UIManager.Instance.DisplayWin();
		}
	}

	public void UnselectAllCells()
	{
		cells.ForEach(x => x.State = CellState.Unselected);
	}

	public void OnlySelectCell(Cell cell)
	{
		cells.Where(x => x != cell).ForEach(x => x.State = CellState.Unselected);
	}

	private void OnDestroy()
	{
		OnEndLevel -= StopAutoDeletionRows;
	}
}
