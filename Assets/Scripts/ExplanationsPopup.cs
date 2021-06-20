using DG.Tweening;
using UnityEngine;

public class ExplanationsPopup : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private CanvasGroup group;

	public void Show()
	{
		group.blocksRaycasts = true;
		group.interactable = true;
		group.alpha = 0f;
		group.DOFade(1f, 0.2f);

		if (UIManager.Instance != null && UIManager.Instance.ShowTimer)
		{
			UIManager.Instance.StopTimer();
		}
		if (LevelController.Instance != null)
		{
			Board board = LevelController.Instance.LevelBoard;
			board.StopAutoDeletionRows();
			board.DisableInput();
			if (board.Player.CurrentCell.Freeze)
				board.Player.CurrentCell.PauseFreeze();
		}
	}

	public void Close()
	{
		group.blocksRaycasts = false;
		group.interactable = false;
		group.DOFade(0f, 0.2f);

		if (UIManager.Instance != null && UIManager.Instance.ShowTimer)
		{
			UIManager.Instance.ContinueTimer();
		}
		if (LevelController.Instance != null)
		{
			Board board = LevelController.Instance.LevelBoard;
			board.StartAutoDeletionRows();
			board.AllowInput();
			if (board.Player.CurrentCell.Freeze)
				board.Player.CurrentCell.ContinueFreeze();
		}
	}
}
