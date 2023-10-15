using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private List<GameObject> tileList;
    [SerializeField] private TextAsset levelDataJSON;
    [SerializeField] private LevelData levelDataList;
    private List<string> colorOptions = new List<string>();
    private string[] curentColorOptions = new string[] { "TextureBlue", "TexturePink", "TexturePurple", "TextureRed", "TextureWhite", "TextureYellow" };
    private string filePath;
    void Start()
    {
        LoadLevelData();
        SpawnTilesPrefab();
    }

    private void LoadLevelData()
    {
        filePath = AssetDatabase.GetAssetPath(levelDataJSON);
        levelDataList = JsonUtility.FromJson<LevelData>(levelDataJSON.text);
    }
    public string GetRandomLevelTexture(List<string> textureList)
    {
        if (textureList == null || textureList.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, levelDataList.Level);
        return textureList[randomIndex];
    }
    public string GetRandomColorTexture()
    {
        if (colorOptions.Count == 0)
        {
            colorOptions = curentColorOptions.ToList();
            GetRandomColorTexture();
        }
        string randomColor = colorOptions[Random.Range(0, colorOptions.Count)];
        switch (randomColor)
        {
            case "TextureBlue":
                colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(levelDataList.TextureBlue);
            case "TexturePink":
                colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(levelDataList.TexturePink);
            case "TexturePurple":
                colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(levelDataList.TexturePurple);
            case "TextureRed":
                colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(levelDataList.TextureRed);
            case "TextureWhite":
                colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(levelDataList.TextureWhite);
            case "TextureYellow":
                colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(levelDataList.TextureYellow);
            default:
                return null;
        }
    }
    private void SpawnTilesPrefab()
    {
        float x = -3f;
        float z = 6f;
        string nameTexture = GetRandomColorTexture();
        for (int i = 1; i <= levelDataList.Level * 9; i++)
        {
            GameObject newTile = Instantiate(tilePrefab, transform);
            newTile.name = nameTexture;
            tileList.Add(newTile);
            newTile.transform.localPosition = new Vector3(x, 2f, z);
            newTile.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("tiles/" + nameTexture);
            if (i % 3 == 0)
            {
                nameTexture = GetRandomColorTexture();
            }
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

[System.Serializable]
public class LevelData
{
    public int Level;
    public List<string> TextureBlue;
    public List<string> TexturePink;
    public List<string> TexturePurple;
    public List<string> TextureRed;
    public List<string> TextureWhite;
    public List<string> TextureYellow;
}