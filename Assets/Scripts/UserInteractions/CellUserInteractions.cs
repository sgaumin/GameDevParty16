using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellUserInteractions : MonoBehaviour
{
    public Material defaultMat;
    public Material overMat;
    public Material clickedMat;
    Renderer render;
    Cell cell;
    CellStates state = CellStates.Unselected;

    private void Start()
    {
        render = gameObject.GetComponent<Renderer>();
        cell = GetComponent<Cell>();
    }

    private void OnMouseOver()
    {
        if(state != CellStates.Selected)
            render.material = overMat;
    }

    private void OnMouseExit()
    {
        if(state != CellStates.Selected)
            render.material = defaultMat;
    }

    private void OnMouseUp()
    {
        render.material = clickedMat;
        state = CellStates.Selected;
    }
}
