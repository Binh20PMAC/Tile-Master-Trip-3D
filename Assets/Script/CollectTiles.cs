using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CollectTiles : MonoBehaviour
{
    [SerializeField] private GameObject currentHoveredTile = null;
    [SerializeField] private Transform[] boxTransforms;
    [SerializeField] private GameObject[] box;
    [SerializeField] private bool checkWin = false;
    [SerializeField] SpawnTiles spawnTiles;
    private Vector3 originalScale;

    private void Start()
    {
        box = new GameObject[boxTransforms.Length + 2];
    }
    private void Update()
    {
        HoverScale();
        MoveToBoxAndCheckCollect();
    }
    private void HoverScale()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Tile" && hit.collider.gameObject != currentHoveredTile)
            {
                if (currentHoveredTile != null)
                {
                    if (currentHoveredTile.transform.localScale != originalScale)
                    {
                        currentHoveredTile.transform.localScale = originalScale;
                    }
                }
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
    private void MoveToBoxAndCheckCollect()
    {
        if (Input.GetMouseButtonDown(0) && currentHoveredTile != null)
        {
            for (int i = 0; i < box.Length; i++)
            {
                if (box[i] != null && box[i].name != currentHoveredTile.name) continue;
                else if (box[i] != null && box[i + 1] != null && box[i].name == currentHoveredTile.name && box[i + 1].name != currentHoveredTile.name)
                {
                    BackTileFromBox(i + 1, box.Length);
                    box[i + 1] = currentHoveredTile;
                    currentHoveredTile.transform.position = boxTransforms[i + 1].position;
                    CheckLose();
                    DontTouch();
                    break;
                }
                else if (box[i + 2] != null && box[i + 1].name == currentHoveredTile.name)
                {
                    BackTileFromBox(i + 2, box.Length);
                    box[i + 2] = currentHoveredTile;
                    currentHoveredTile.transform.position = boxTransforms[i + 2].position;
                    CollectTileFromBox(i, i + 2);
                    if (box[i + 3] != null) ForwardTileFromBox(i + 3, box.Length);
                    DontTouch();
                    break;
                }
                else if (box[i] == null)
                {
                    box[i] = currentHoveredTile;
                    currentHoveredTile.transform.position = boxTransforms[i].position;
                    if (i >= 2)
                    {
                        if (box[i - 2].name == currentHoveredTile.name)
                        {
                            CollectTileFromBox(i - 2, i);
                        }
                        else CheckLose();
                    }
                    DontTouch();
                    break;
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
        CheckWin();
    }
    private void ForwardTileFromBox(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (box[i] == null) continue;
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

    private void DontTouch()
    {
        foreach (var tile in box)
        {
            if (tile != null)
            {
                tile.tag = "Untagged";
            }
            else break;
        }
    }

    private void CheckWin()
    {
        foreach (var tile in spawnTiles.tileList)
        {
            if (tile.activeInHierarchy)
            {
                checkWin = false;
                break;
            }
            else checkWin = true;
        }
        if (checkWin)
        {
            ++spawnTiles.levelDataList.Level;
            string updatedJson = JsonUtility.ToJson(spawnTiles.levelDataList);
            File.WriteAllText(Application.dataPath + "/Resources/LevelData.json", updatedJson);
            AssetDatabase.Refresh();
        }
    }

    private void CheckLose()
    {
        int index = 0;
        foreach (var tile in box) if (tile != null) index++;
        if (index >= 8) Debug.Log("You Lose");
    }
}
