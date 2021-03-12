using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public Camera cam;
    public LayerMask terrain;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // Left Click
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
            {
                // click on cell
                hit.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}
