using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
	private const float sphereRadius = 0.2f;
	private const float checkDistance = 1f;

	[Header("References")]
	[SerializeField] private LayerMask cellMask;

	public Cell CellTop { get; private set; }
	public Cell CellDown { get; private set; }
	public Cell CellLeft { get; private set; }
	public Cell CellRight { get; private set; }
	public Cell CellTopLeft { get; private set; }
	public Cell CellTopRight { get; private set; }
	public Cell CellDownLeft { get; private set; }
	public Cell CellDownRight { get; private set; }

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
	}
}
