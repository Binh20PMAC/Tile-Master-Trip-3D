using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectTiles : MonoBehaviour
{
    private Vector3 originalScale;
    private GameObject currentHoveredObject = null;

    private void Update()
    {
        HoverScale();
    }
    private void HoverScale()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Tile" && hit.collider.gameObject != currentHoveredObject)
            {
                originalScale = hit.collider.gameObject.transform.localScale;
                currentHoveredObject = hit.collider.gameObject;
                currentHoveredObject.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            }
            else if (hit.collider.gameObject != currentHoveredObject && currentHoveredObject != null)
            {
                currentHoveredObject.transform.localScale = originalScale;
                currentHoveredObject = null;
            }
        }
    }
}
