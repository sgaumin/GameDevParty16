using UnityEngine;

public class CellUserInteractions : MonoBehaviour
{
	[SerializeField] private Cell cell;

	private void OnMouseOver()
	{
		if (cell.State == CellState.Highlighted && LevelController.Instance.GameState == GameStates.Play)
		{
			cell.State = CellState.Selected;
		}
	}

	private void OnMouseExit()
	{
		if (cell.State == CellState.Selected && LevelController.Instance.GameState == GameStates.Play)
		{
			cell.State = CellState.Highlighted;
		}
	}

	private void OnMouseUp()
	{
		if (cell.State != CellState.Unselected && LevelController.Instance.GameState == GameStates.Play)
		{
			cell.State = CellState.Cliqued;
		}
	}
}
