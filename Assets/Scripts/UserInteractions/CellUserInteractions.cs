using UnityEngine;

public class CellUserInteractions : MonoBehaviour
{
	[SerializeField] private Cell cell;

	private void OnMouseOver()
	{
		if (cell.State == CellState.Highlighted)
		{
			cell.State = CellState.Selected;
		}
	}

	private void OnMouseExit()
	{
		if (cell.State == CellState.Selected)
		{
			cell.State = CellState.Highlighted;
		}
	}

	private void OnMouseUp()
	{
		if (cell.State != CellState.Unselected)
		{
			cell.State = CellState.Cliqued;
		}
	}
}
