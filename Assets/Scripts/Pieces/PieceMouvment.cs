using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PieceMouvment : MonoBehaviour
{
    public Cell originCell;

    Transform originCellTransform;
    
    public Cell[] path;
    
    Cell currentCell;

    public float lookatDuration = 1.0f;
    public float moveDuration = 1.0f;
    public float rotateDuration = 1.0f;
    public float height = 1.5f;

    private void Start()
    {
        originCellTransform = originCell.transform;
        currentCell = originCell;
        //PlaceAt(originCell);
        FolowPath(path);
    }


    // Move cell by cell
    public void FolowPath(Cell[] path)
    {
        StartCoroutine(MovePath(path));
    }

    private IEnumerator MovePath(Cell[] path)
    {
        foreach (Cell cell in path)
        {
            Transform cellTransform = cell.transform;
            Tweener looker = transform.DOLookAt(cellTransform.position, lookatDuration, AxisConstraint.Y);
            yield return looker.WaitForCompletion();

            Vector3 heightOffset = Vector3.zero;
            heightOffset.y = height;
            Tweener mover = transform.DOMove(cellTransform.position + heightOffset, moveDuration);
            yield return mover.WaitForCompletion();

            Tweener rotater = transform.DORotate(cellTransform.forward, moveDuration);
            yield return rotater.WaitForCompletion();
        }
    }

    // Move to one cell
    public void PlaceAt(Cell cell)
    {
        currentCell = cell;
        Transform cellTransform = cell.transform;
        Vector3 heightOffset = Vector3.zero;
        heightOffset.y = height;

        StartCoroutine(MovePiece(cellTransform));
    }

    private IEnumerator MovePiece(Transform cellTransform)
    {

        Tweener looker = transform.DOLookAt(cellTransform.position, lookatDuration, AxisConstraint.Y);
        yield return looker.WaitForCompletion();

        Vector3 heightOffset = Vector3.zero;
        heightOffset.y = height;
        Tweener mover = transform.DOMove(cellTransform.position + heightOffset, moveDuration);
        yield return mover.WaitForCompletion();

        Tweener rotater = transform.DORotate(cellTransform.forward, moveDuration);
        yield return rotater.WaitForCompletion();

    }
}
