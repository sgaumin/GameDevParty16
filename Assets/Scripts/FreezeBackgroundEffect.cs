using DG.Tweening;
using UnityEngine;

public class FreezeBackgroundEffect : MonoBehaviour
{
	public static FreezeBackgroundEffect Instance { get; private set; }

	[SerializeField] private float fadeDuration = 0.2f;

	[Header("References")]
	[SerializeField] private CanvasGroup group;

	private bool isActive;

	protected void Awake() => Instance = this;

	public void Show()
	{
		if (isActive)
			return;

		isActive = true;
		group.alpha = 0f;
		group.DOFade(1f, fadeDuration);
	}

	public void Stop()
	{
		group.DOFade(0f, fadeDuration);
		isActive = false;
	}
}
