using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuLevelController : MonoBehaviour
{
	[Header("Params")]
	[SerializeField] private float scaleFactor = 1.2f;
	[SerializeField] private float scaleDuration = 0.2f;
	[SerializeField] private Ease scaleEase = Ease.OutSine;

	public void ScaleUp(GameObject button)
	{
		button.transform.DOKill();
		button.transform.DOScale(new Vector3(1, 1, 1) * scaleFactor, scaleDuration).SetEase(scaleEase);
	}

	public void ScaleDown(GameObject button)
	{
		button.transform.DOKill();
		button.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), scaleDuration).SetEase(scaleEase);
	}
}
