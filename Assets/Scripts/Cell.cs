using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
	private const float sphereRadius = 0.2f;
	private const float checkDistance = 1f;

	[Header("References")]
	[SerializeField] private LayerMask cellMask;
	[SerializeField] private GameObject highlight;

	private List<Cell> nearbyCells = new List<Cell>();

	public Cell CellTop { get; private set; }
	public Cell CellDown { get; private set; }
	public Cell CellLeft { get; private set; }
	public Cell CellRight { get; private set; }
	public Cell CellTopLeft { get; private set; }
	public Cell CellTopRight { get; private set; }
	public Cell CellDownLeft { get; private set; }
	public Cell CellDownRight { get; private set; }
	public CellStates Status { get; private set; }
	public List<Cell> NearbyCells => nearbyCells;


	protected void Start()
	{
		Init();
	}

	private void Init()
	{
		CellTop = Physics.OverlapSphere(transform.position + new Vector3(0, 0, 1) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellDown = Physics.OverlapSphere(transform.position + new Vector3(0, 0, -1) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 0) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 0) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellTopLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, 1) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellTopRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, 1) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellDownLeft = Physics.OverlapSphere(transform.position + new Vector3(-1, 0, -1) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();
		CellDownRight = Physics.OverlapSphere(transform.position + new Vector3(1, 0, -1) * checkDistance, sphereRadius, cellMask, QueryTriggerInteraction.Ignore).FirstOrDefault()?.GetComponentInParent<Cell>();

		if (CellTop != null)
		{
			nearbyCells.Add(CellTop);
		}
		if (CellDown != null)
		{
			nearbyCells.Add(CellDown);
		}
		if (CellLeft != null)
		{
			nearbyCells.Add(CellLeft);
		}
		if (CellRight != null)
		{
			nearbyCells.Add(CellRight);
		}
		if (CellTopLeft != null)
		{
			nearbyCells.Add(CellTopLeft);
		}
		if (CellTopRight != null)
		{
			nearbyCells.Add(CellTopRight);
		}
		if (CellDownLeft != null)
		{
			nearbyCells.Add(CellDownLeft);
		}
		if (CellDownRight != null)
		{
			nearbyCells.Add(CellDownRight);
		}
	}

	[ContextMenu("Show neighbour")]
	private void ShowNearbyCells()
	{
		nearbyCells.ForEach(x => x.Select());
	}

	[ContextMenu("Hide Neighbour")]
	private void HideNearbyCells()
	{
		nearbyCells.ForEach(x => x.Unselect());
	}

	public void Select()
	{
		highlight.gameObject.SetActive(true);
	}

	public void Unselect()
	{
		highlight.gameObject.SetActive(false);
	}
}
