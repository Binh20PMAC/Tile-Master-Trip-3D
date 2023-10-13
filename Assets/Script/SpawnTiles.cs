using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int level;
    [SerializeField] private List<GameObject> tileList;
    void Start()
    {
        SpawnTilesPrefab();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnTilesPrefab()
    {
        float x = -3f;
        float z = 6f;
        for (int i = 0; i < level * 9; i++)
        {
            GameObject newTile = Instantiate(tilePrefab, transform);
            newTile.name = "Tile " + i;
            tileList.Add(newTile);
            newTile.transform.localPosition = new Vector3(x, 2f, z);

            if (x > 2f)
            {
                z -= 3f;
                x = -3f;
            }
            else
            {
                x += 3f;
            }
        }
    }

}
