using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectTiles : MonoBehaviour
{
    [SerializeField] private GameObject currentHoveredTile = null;
    [SerializeField] private Transform[] boxTransforms;
    [SerializeField] private GameObject[] box;
    private Vector3 originalScale;

    private void Start()
    {
        box = new GameObject[boxTransforms.Length + 2];
    }
    private void Update()
    {
        HoverScale();
        MoveToBox();
    }
    private void HoverScale()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Tile" && hit.collider.gameObject != currentHoveredTile)
            {
                originalScale = hit.collider.gameObject.transform.localScale;
                currentHoveredTile = hit.collider.gameObject;
                currentHoveredTile.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            }
            else if (hit.collider.gameObject != currentHoveredTile && currentHoveredTile != null)
            {
                currentHoveredTile.transform.localScale = originalScale;
                currentHoveredTile = null;
            }
        }
    }
    private void MoveToBox()
    {
        if (Input.GetMouseButtonDown(0) && currentHoveredTile != null)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if (i >= 7)
                {
                    Debug.Log("You lose");
                }
                else
                {
                    if (box[i] != null && box[i + 1] != null && box[i].name == currentHoveredTile.name && box[i + 1].name != currentHoveredTile.name)
                    {
                        BackTileFromBox(i + 1, box.Length);
                        box[i + 1] = currentHoveredTile;
                        currentHoveredTile.transform.position = boxTransforms[i + 1].position;
                        break;
                    }
                    else if (box[i + 2] != null && box[i + 1].name == currentHoveredTile.name)
                    {
                        if (box[i + 3] != null)
                        {
                            BackTileFromBox(i + 2, box.Length);
                            box[i + 2] = currentHoveredTile;
                            currentHoveredTile.transform.position = boxTransforms[i + 2].position;
                        }
                        CollectTileFromBox(i, i + 2);
                        if (box[i + 3] != null) ForwardTileFromBox(i + 3, box.Length);
                        break;
                    }
                    else if (box[i] == null)
                    {
                        box[i] = currentHoveredTile;
                        currentHoveredTile.transform.position = boxTransforms[i].position;
                        if (i >= 2)
                        {
                            if (box[i - 1].name == currentHoveredTile.name)
                            {
                                CollectTileFromBox(i - 2, i);
                            }
                        }
                        break;
                    }
                }
            }

        }
    }

    private void CollectTileFromBox(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            box[i].SetActive(false);
            box[i] = null;
        }
    }
    private void ForwardTileFromBox(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (box[i] != null) break;
            box[i - 3] = box[i];
            box[i - 3].transform.position = boxTransforms[i - 3].position;
            box[i] = null;
        }
    }
    private void BackTileFromBox(int start, int end)
    {
        for (int i = end - 1; i >= start; i--)
        {
            if (box[i] == null) continue;
            box[i + 1] = box[i];
            box[i + 1].transform.position = boxTransforms[i + 1].position;
        }
    }
}
