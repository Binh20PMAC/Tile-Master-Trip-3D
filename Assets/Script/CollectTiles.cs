using System.Collections;
using UnityEngine;

public class CollectTiles : MonoBehaviour
{
    [SerializeField] private GameObject currentHoveredTile = null;
    [SerializeField] private Transform[] boxTransforms;
    [SerializeField] private GameObject[] box;
    [SerializeField] private bool checkWin = false;
    [SerializeField] private SpawnTiles spawnTiles;
    [SerializeField] private UIManager uiManager;
    private Vector3 originalScale;
    private bool canClick = true;
    private int star = 0;
    private void Start()
    {
        uiManager = GameObject.Find("UICode").GetComponent<UIManager>();
        box = new GameObject[boxTransforms.Length + 2];
    }
    private void Update()
    {
        HoverScale();
        MoveToBoxAndCheckCollect();
    }

    public void ClearBox()
    {
        currentHoveredTile = null;
        box = new GameObject[boxTransforms.Length + 2];
    }
    private void HoverScale()
    {
        if (uiManager.GetPause()) return;
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
        if (Input.GetMouseButtonUp(0) && currentHoveredTile != null && canClick)
        {
            canClick = false;
            Invoke("EnableClick", 0.2f);
            for (int i = 0; i < box.Length; i++)
            {
                if (box[i] != null && box[i].name != currentHoveredTile.name) continue;
                else if (box[i] != null && box[i + 1] != null && box[i].name == currentHoveredTile.name && box[i + 1].name != currentHoveredTile.name)
                {
                    BackTileFromBox(i + 1, box.Length);
                    box[i + 1] = currentHoveredTile;
                    currentHoveredTile.transform.position = boxTransforms[i + 1].position;
                    currentHoveredTile.transform.rotation = Quaternion.identity;
                    CheckLose();
                    StartCoroutine(DontTouch());
                    break;
                }
                else if (box[i + 2] != null && box[i + 1].name == currentHoveredTile.name)
                {
                    BackTileFromBox(i + 2, box.Length);
                    box[i + 2] = currentHoveredTile;
                    currentHoveredTile.transform.position = boxTransforms[i + 2].position;
                    currentHoveredTile.transform.rotation = Quaternion.identity;
                    StartCoroutine(CollectTileFromBox(i, i + 2));
                    if (box[i + 3] != null) StartCoroutine(ForwardTileFromBox(i + 3, box.Length));
                    StartCoroutine(DontTouch());
                    break;
                }
                else if (box[i] == null)
                {
                    box[i] = currentHoveredTile;
                    currentHoveredTile.transform.position = boxTransforms[i].position;
                    currentHoveredTile.transform.rotation = Quaternion.identity;
                    if (i >= 2)
                    {
                        if (box[i - 2].name == currentHoveredTile.name)
                        {
                            StartCoroutine(CollectTileFromBox(i - 2, i));
                        }
                        else CheckLose();
                    }
                    StartCoroutine(DontTouch());
                    break;
                }
            }

        }
    }

    private void EnableClick()
    {
        canClick = true;
    }
    private IEnumerator CollectTileFromBox(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            yield return new WaitForSeconds(0.06f);
            box[i].SetActive(false);
            box[i] = null;
        }
        uiManager.SetStar(++star);
        CheckWin();
    }
    private IEnumerator ForwardTileFromBox(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            yield return new WaitForSeconds(0.06f);
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

    private IEnumerator ShrinkTile(GameObject tile)
    {
        if (tile != null)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime * 0.01f;
                tile.transform.localScale = Vector3.Lerp(tile.transform.localScale, tile.transform.localScale - new Vector3(0.5f, 0.5f, 0.5f), t);
                if (tile.transform.localScale.x < 2f) yield break;
                yield return null;
            }

        }
    }
    private IEnumerator DontTouch()
    {
        foreach (var tile in box)
        {
            if (tile != null && tile.tag != "Untagged")
            {
                tile.tag = "Untagged";
                yield return StartCoroutine(ShrinkTile(tile));
            }
        }
    }
    private void CheckWin()
    {
        foreach (var tile in spawnTiles.GetTileList())
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
            uiManager.SetActiveWin(true, star);
            spawnTiles.GetLevelDataList().Star += star;
            ++spawnTiles.GetLevelDataList().Level;
            string updatedJson = JsonUtility.ToJson(spawnTiles.GetLevelDataList());
            spawnTiles.SetLevelDataJson(updatedJson);
        }
    }

    private void CheckLose()
    {
        int index = 0;
        foreach (var tile in box) if (tile != null) index++;
        if (index >= 7)
        {
            uiManager.SetActiveLose(true);
            currentHoveredTile = null;
        }
    }
}
