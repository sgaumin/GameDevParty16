using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OnomatopeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image icon;
    [SerializeField] private Sprite[] sprites;

    [Header("Params")]
    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Ease scaleEase = Ease.OutCubic;

    public float ScaleDuration {
        get => scaleDuration;
    }

    public void Display()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = sprites[Random.Range(0, sprites.Length)];
        //icon.gameObject.FadIn(0.5f);
        icon.transform.DOKill();
        icon.transform.DOScale(new Vector3(4, 4, 4) * scaleFactor, scaleDuration).SetEase(scaleEase);
        //icon.gameObject.FadOut(0.5f);
        //icon.gameObject.SetActive(false);
    }
}
