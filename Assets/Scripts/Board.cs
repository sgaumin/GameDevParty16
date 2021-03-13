using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	private List<Cell> allCells = new List<Cell>();

	void Start()
	{
		allCells = GetComponentsInChildren<Cell>().ToList();
	}

	public void UnselectAllCells()
	{
		allCells.ForEach(x => x.Unselect());
	}
}
