using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{

	[Header("Sprites")]
	[SerializeField] protected Sprite pionSprite;
	[SerializeField] protected Sprite fouSprite;
	[SerializeField] protected Sprite cavalierSprite;
	[SerializeField] protected Sprite tourSprite;

	[Header("References")]
	[SerializeField] protected LayerMask cellMask;
	[SerializeField] protected Image icon;

	protected Board board;
	protected Cell currentCell;

	public bool isAlive { get; set; }

	protected virtual void Start()
	{
		isAlive = true;
	}

	public void Kill()
	{
		isAlive = false;
		gameObject.SetActive(false);
	}

	protected void CheckCurrentCell()
	{
		Ray ray = new Ray(transform.position, -Vector3.up);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, cellMask))
		{
			currentCell = hit.collider.GetComponentInParent<Cell>();
		}
	}

	protected void SetIcon(PieceType type)
	{
		switch (type)
		{
			case PieceType.Pion:
				icon.sprite = pionSprite;
				break;
			case PieceType.Fou:
				icon.sprite = fouSprite;
				break;
			case PieceType.Cavalier:
				icon.sprite = cavalierSprite;
				break;
			case PieceType.Tour:
				icon.sprite = tourSprite;
				break;
		}
	}
}
