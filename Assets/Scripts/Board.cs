using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools.Utils;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct MarkType
{
	public MarkNames Name;
	public Material Material;
}

[System.Serializable]
public struct BoardEvent
{
	[IntRangeSlider(0, 100)] public IntRange RowIndex;
	public UnityEvent Event;
}

public class Board : MonoBehaviour
{
	[Header("Rules")]
	[SerializeField] private Cell spawnCell;
	[SerializeField] private bool rowAutoDeletionOnStart;
	[SerializeField] private int timerCount = 10;
	[SerializeField] private float timeBeforeRowDeletion = 1f;
	[SerializeField] private int rowStartDisplayCount = 8;
	[SerializeField, IntRangeSlider(0, 10)] private IntRange rowDeletionAmount = new IntRange(1, 3);
	[Space]
	[SerializeField] private List<BoardEvent> events = new List<BoardEvent>();
	[Space]
	[SerializeField] private bool showMarkers;
	[SerializeField] private List<MarkType> marks = new List<MarkType>();

	[Header("Animations")]
	[SerializeField] private float fadDestroyingCellDuration = 0.01f;
	[SerializeField] private float offsetYDestroyingCell = -2f;

	[Header("References")]
	[SerializeField] private Player piecePrefab;
	[SerializeField] private Transform cellHolder;

	private List<Cell> cells = new List<Cell>();
	private List<List<Cell>> rowCells = new List<List<Cell>>();
	private List<Enemy> enemies = new List<Enemy>();
	private bool hasFinishTurn;
	private Coroutine rowDeletion;
	private BoardStates boardState;
	private bool inverseAnimation;
	private float positionPercentage;
	private float currentRowDeletionAmount;

	public SoundData killSound;
	public SoundData playerKilledSound;
	public SoundData winSound;

	public Action OnTriggerEvents;
	public Action OnStartPlayerTurn;
	public Action OnPlayerSelectedCell;
	public Action OnEndPlayerTurn;
	public Action OnEndLevel;
	public Action OnRefreshBoard;

	public Player Player { get; set; }
	public List<Enemy> Enemies
	{
		get => enemies;
		set
		{
			enemies = value;
		}
	}
	public bool ShowMarkers => showMarkers;
	public List<MarkType> Marks => marks;
	public BoardStates BoardState
	{
		get => boardState;
		set
		{
			boardState = value;

			switch (value)
			{
				case BoardStates.TriggerEvents:
					OnTriggerEvents?.Invoke();
					break;
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
	public bool CanReceiveInput { get; set; }

	public void EnemyKilled(PieceType type)
	{
		killSound.instrument.Play();
		killSound.voice.Play();
		ScoreController.Instance.SetScoreKill(type);
	}

	protected void Awake()
	{
		OnTriggerEvents += TriggerEvents;
		OnStartPlayerTurn += AllowInput;
		OnPlayerSelectedCell += DisableInput;
		OnEndLevel += StopAutoDeletionRows;
	}

	void Start()
	{
		cells = GetComponentsInChildren<Cell>().ToList();
		ReorganizeCells();
		cells.ForEach(x => x.Init());

		SpawnPiece();
		cells.ForEach(x => x.Piece = Player);

		enemies = FindObjectsOfType<Enemy>().ToList();
		ActivateEnemiesChecks();

		BoardState = BoardStates.TriggerEvents;

		StartAutoDeletionRows();
		UIManager.Instance.StartTimer(timerCount);
	}

	private void TriggerEvents()
	{
		List<BoardEvent> eventsToTrigger = events.Where(x => x.RowIndex.Contains((int)Player.CurrentCell.transform.position.z)).ToList();
		foreach (BoardEvent b in eventsToTrigger)
		{
			b.Event?.Invoke();
		}
		BoardState = BoardStates.StartPlayerTurn;
	}

	public void ForcePawn()
	{
		Player.ClearPiecePool();
		Player.PiecePoolType = PiecePoolType.OnlyPawn;
	}

	public void ForceKnight()
	{
		Player.ClearPiecePool();
		Player.PiecePoolType = PiecePoolType.OnlyKnight;
	}

	public void ForceAll()
	{
		Player.ClearPiecePool();
		Player.PiecePoolType = PiecePoolType.All;
	}

	public void HidePieceQueue()
	{
		UIManager.Instance.ShowPieceQueue = false;
	}

	public void ShowPieceQueue()
	{
		UIManager.Instance.ShowPieceQueue = true;
	}

	public void HideScore()
	{
		UIManager.Instance.ShowScore = false;
	}

	public void ShowScore()
	{
		UIManager.Instance.ShowScore = true;
	}

	public void HideTimer()
	{
		UIManager.Instance.ShowTimer = false;
	}

	public void ShowTimer()
	{
		UIManager.Instance.ShowTimer = true;
	}

	public void AllowInput()
	{
		CanReceiveInput = true;
	}

	public void DisableInput()
	{
		CanReceiveInput = false;
	}

	private void ActivateEnemiesChecks()
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
			GameObject o = new GameObject();
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

	public void ResetBoard()
	{
		cells.ForEach(x => x.DefineCellLinks());
		ActivateEnemiesChecks();
		SafeCheckOnPlayer();
		OnRefreshBoard?.Invoke();
	}

	private void CheckRowDeletion()
	{
		CheckPlayerPositionPercentage();
		if (rowAutoDeletionOnStart)
		{
			DeleteFirstRow();
		}

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

		positionPercentage = (Player.transform.position.z - zMin) / (zMax - zMin);
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
						playerKilledSound.instrument.Play();
						playerKilledSound.voice.Play();
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
			if (lastRow.IsEmpty() || lastRow.First().gameObject.activeSelf)
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
		Player = Instantiate(piecePrefab, transform);
		Player.Init(spawnCell);

		LevelController.Instance.SetCameraTarget(Player.transform);
		UIManager.Instance.Dialogues.Init();
	}

	public void PlayerSelectedCell()
	{
		BoardState = BoardStates.PlayerSelecCell;
	}

	private void SafeCheckOnPlayer()
	{
		if (Player == null || Player.CurrentCell == null)
		{
			EndLevel();
		}
	}

	public void EndTurnPlayer()
	{
		hasFinishTurn = false;
		BoardState = BoardStates.EndPlayerTurn;
		SafeCheckOnPlayer();
		EndingTurn();
	}

	public void EndingTurn()
	{
		if (BoardState != BoardStates.EndLevel && !hasFinishTurn)
		{
			if (enemies.IsEmpty() || enemies.Where(x => x.gameObject.activeSelf).All(x => x.HasFinishTurn))
			{
				hasFinishTurn = true;
				BoardState = BoardStates.TriggerEvents;
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
			winSound.instrument.Play();
			winSound.voice.Play();
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

	[ContextMenu("Reset Mark")]
	public void ResetMarks()
	{
		GetComponentsInChildren<Cell>().ForEach(x => x.Mark = MarkNames.None);
	}

	private void OnDestroy()
	{
		OnTriggerEvents -= TriggerEvents;
		OnStartPlayerTurn -= AllowInput;
		OnPlayerSelectedCell -= DisableInput;
		OnEndLevel -= StopAutoDeletionRows;
	}
}
