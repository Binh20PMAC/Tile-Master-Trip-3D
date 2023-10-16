using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] SpawnTiles spawnTiles;
    [SerializeField] private TMP_Text txtStar;
    [SerializeField] private TMP_Text txtClock;
    [SerializeField] private TMP_Text txtLevel;
    private float countdownTime = 120f;

    private void Start()
    {
        txtLevel.text = $"Lv.{spawnTiles.LoadLevelDataList().Level}";
        txtStar.text = $"{spawnTiles.LoadLevelDataList().Star}";
    }

    private void Update()
    {
        UpdateTimerUI();
    }

    public void SetStar(int start)
    {
        txtStar.text = $"{start}";
    }

    private void UpdateTimerUI()
    {
        countdownTime -= Time.deltaTime;
        if (countdownTime < 0)
        {
            countdownTime = 0;
            Debug.Log("You Lose");
        }
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        txtClock.text = string.Format("{0:00}:{1:00}", minutes, seconds); ;
    }
}
