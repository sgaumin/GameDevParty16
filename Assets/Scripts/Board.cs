using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	[Header("Rules")]
	[SerializeField] private int timerCount = 10;
	[SerializeField] private int rowDisplayCount = 8;
	[SerializeField] private int rowDeletionFrequency = 1;
	[SerializeField] private float timeBeforeRowDeletion = 1f;
	[SerializeField] private Cell spawnCell;

	[Header("References")]
	[SerializeField] private DialoguesController dialoguesController;
	[SerializeField] private Player piecePrefab;
	[SerializeField] private Transform cellHolder;
	[SerializeField] private Transform cameraTarget;

	private List<Cell> cells = new List<Cell>();
	private List<List<Cell>> rowCells = new List<List<Cell>>();
	private List<Enemy> enemies = new List<Enemy>();
	public Player piece;
	private Game game;
	private GameObject o;
	private bool hasFinishTurn;
	private Coroutine rowDeletion;

	public Action OnStartPlayerTurn;
	public Action OnEndPlayerTurn;
	public Action OnPlayerSelectedCell;
	public Action OnEndLevel;

	public List<Enemy> Enemies
	{
		get => enemies;
		set
		{
			enemies = value;
		}
	}

	protected void Awake()
	{
		OnEndLevel += StopAutoDeletionRows;
	}

	void Start()
	{
		game = FindObjectOfType<Game>();
		cells = GetComponentsInChildren<Cell>().ToList();
		ReorganizeCells();
		cells.ForEach(x => x.Init());

		enemies = FindObjectsOfType<Enemy>().ToList();

		SpawnPiece();
		cells.ForEach(x => x.Piece = piece);

		UIManager.Instance.StartTimer(timerCount);
		StartAutoDeletionRows();

		OnStartPlayerTurn?.Invoke();
	}

	private void Update()
	{
		if (piece != null)
		{
			cameraTarget.transform.position = piece.transform.position;
		}
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
		for (int i = 0; i < rowDisplayCount; i++)
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
			for (int i = 0; i < rowDeletionFrequency; i++)
			{
				CheckRowDeletion();
			}
		}
	}

	private void CheckRowDeletion()
	{
		for (int i = 0; i < rowDeletionFrequency; i++)
		{
			DeleteFirstRow();
			ShowNewRow();
			cells.ForEach(x => x.DefineCellLinks());
		}

	}

	private void DeleteFirstRow()
	{
		List<Cell> firstRow = rowCells.First();
		rowCells.RemoveAt(0);

		Character currentCharacter;
		foreach (Cell cell in firstRow)
		{
			currentCharacter = cell.TargetPresentOnCell<Character>();
			if (currentCharacter != null)
			{
				currentCharacter.Kill();
				if (currentCharacter is Player)
				{
					EndLevel();
				}
			}
		}

		firstRow.ForEach(x => cells.Remove(x));
		firstRow.ForEach(x => Destroy(x.gameObject));
	}

	private void ShowNewRow()
	{
		int i = 1;
		while (i <= rowCells.Count)
		{
			if (rowCells.Count > rowDisplayCount - i)
			{
				rowCells[rowDisplayCount - i].ForEach(x => x.gameObject.SetActive(true));
				break;
			}
			else
			{
				i++;
			}
		}
	}

	private void SpawnPiece()
	{
		piece = Instantiate(piecePrefab, transform);
		piece.Init(spawnCell);
		dialoguesController.Init();
	}

	public void PlayerSelectedCell()
	{
		OnPlayerSelectedCell?.Invoke();

	}

	public void EndTurnPlayer()
	{
		hasFinishTurn = false;
		OnEndPlayerTurn?.Invoke();

		if (enemies.IsEmpty())
		{
			EndingTurn();
		}
	}

	public void EndingTurn()
	{
		if (game.GameState != GameStates.GameOver && !hasFinishTurn)
		{
			if (enemies.IsEmpty() || enemies.Where(x => x.gameObject.activeSelf).All(x => x.HasFinishTurn))
			{
				hasFinishTurn = true;
				OnStartPlayerTurn?.Invoke();
			}
		}
	}

	public void EndLevel(bool hasWon = false)
	{
		game.GameState = GameStates.GameOver;

		OnEndLevel?.Invoke();
		UnselectAllCells();
		if(!hasWon)
        {
			UIManager.Instance.DisplayGameOver();
        } else
        {
			ReloadLevel();
        }
    }

	public void ReloadLevel()
    {
        game.ReloadLevel();
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
