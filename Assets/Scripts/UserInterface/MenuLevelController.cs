using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class MenuLevelController : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float scaleFactor = 1.2f;

    public void ScaleUp(GameObject button)
    {
        button.transform.DOScale(new Vector3(1, 1, 1) * scaleFactor, 0.5f);
    }

    public void ScaleDown(GameObject button)
    {
        button.transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.5f);
    }
}
