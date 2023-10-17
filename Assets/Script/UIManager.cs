using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SpawnTiles spawnTiles;
    [SerializeField] private GameObject gameScenePrefab;
    [SerializeField] private TMP_Text txtStar;
    [SerializeField] private TMP_Text txtClock;
    [SerializeField] private TMP_Text txtLevel;
    [SerializeField] private TMP_Text txtHomeLevel;
    [SerializeField] private TMP_Text txtHomeCoin;
    [SerializeField] private TMP_Text txtHomeStar;
    [SerializeField] private TMP_Text txtLevelLose;
    [SerializeField] private GameObject backgroundPause;
    [SerializeField] private GameObject backgroundLose;
    [SerializeField] private GameObject backgroundSeting;
    [SerializeField] private GameObject backgroundGamePlay;
    [SerializeField] private GameObject backgroundGameHome;
    [SerializeField] private GameObject gameScene;
    [SerializeField] private Sprite[] backgroundSound;
    [SerializeField] private Sprite[] backgroundMusic;
    [SerializeField] private Image backgroundSoundButton;
    [SerializeField] private Image backgroundMusicButton;
    [SerializeField] private Image backgroundSetingSoundButton;
    [SerializeField] private Image backgroundSetingMusicButton;
    private float countdownTime = 120f;
    private bool isPaused = false;
    private void Awake()
    {
        SpawnGameScene(false);
        spawnTiles.SetLevelDataList();
    }
    private void Start()
    {
        txtHomeStar.text = $"{spawnTiles.GetLevelDataList().Star}";
        txtHomeCoin.text = $"{spawnTiles.GetLevelDataList().Coin}";
        txtHomeLevel.text = $"{spawnTiles.GetLevelDataList().Level}";
        txtLevel.text = $"Lv.{spawnTiles.GetLevelDataList().Level}";
        txtStar.text = "0";
        backgroundSoundButton.sprite = backgroundSound[spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        backgroundMusicButton.sprite = backgroundMusic[spawnTiles.GetLevelDataList().Music ? 1 : 0];
        backgroundSetingSoundButton.sprite = backgroundSound[spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        backgroundSetingMusicButton.sprite = backgroundMusic[spawnTiles.GetLevelDataList().Music ? 1 : 0];
    }
    private void Update()
    {
        UpdateTimerUI();
    }
    private void UpdateTimerUI()
    {
        if (backgroundLose.activeInHierarchy) return;
        countdownTime -= Time.deltaTime;
        if (countdownTime < 0)
        {
            countdownTime = 0;
            if (!backgroundLose.activeInHierarchy)
            {
                SetActiveLose(true);
                countdownTime = 60f;
            }
        }
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        txtClock.text = string.Format("{0:00}:{1:00}", minutes, seconds); ;
    }
    public void SetStar(int star)
    {
        txtStar.text = $"{star}";
    }
    private void SpawnGameScene(bool active)
    {
        if (gameScene != null) Destroy(gameScene);
        GameObject newGameScene = Instantiate(gameScenePrefab);
        newGameScene.name = gameScenePrefab.name;
        spawnTiles = newGameScene.GetComponentInChildren<SpawnTiles>();
        gameScene = newGameScene;
        newGameScene.SetActive(active);
    }
    private void DestroyGameScene()
    {
        Destroy(gameScene);
    }
    public void SetActiveLose(bool active)
    {
        backgroundLose.SetActive (active);
        isPaused = active;
        txtLevelLose.text = $"LEVEL {spawnTiles.GetLevelDataList().Level}";
    }
    public void TryAgain()
    {
        countdownTime = 120f;
        isPaused = false;
        SetActiveLose(false);
        Time.timeScale = 1f;
        DestroyGameScene();
        SpawnGameScene(true);
    }
    public bool GetPause()
    {
        return isPaused;
    }
    public void Pause()
    {
        isPaused = true;
        backgroundPause.SetActive(true);
        Time.timeScale = 0;
    }
    public void Continute()
    {
        isPaused = false;
        backgroundPause.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OpenSeting()
    {
        backgroundSeting.SetActive(true);
    }
    public void CloseSeting()
    {
        backgroundSeting.SetActive(false);
    }
    public void PlayGame()
    {     
        Time.timeScale = 1f;
        backgroundGameHome.SetActive(false);
        backgroundGamePlay.SetActive(true);
        SpawnGameScene(true);
    }
    public void GoHome()
    {
        countdownTime = 120f;
        isPaused = false;
        backgroundGameHome.SetActive(true);
        backgroundGamePlay.SetActive(false);
        backgroundPause.SetActive(false);
        backgroundLose.SetActive(false);
        Start();
        DestroyGameScene();
    }
    public void Sound()
    {
        spawnTiles.GetLevelDataList().Sound = !spawnTiles.GetLevelDataList().Sound;
        backgroundSoundButton.sprite = backgroundSound[spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        backgroundSetingSoundButton.sprite = backgroundSound[spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        string updatedJson = JsonUtility.ToJson(spawnTiles.GetLevelDataList());
        spawnTiles.SetLevelDataJson(updatedJson);
    }

    public void Music()
    {
        spawnTiles.GetLevelDataList().Music = !spawnTiles.GetLevelDataList().Music;
        backgroundMusicButton.sprite = backgroundMusic[spawnTiles.GetLevelDataList().Music ? 1 : 0];
        backgroundSetingMusicButton.sprite = backgroundMusic[spawnTiles.GetLevelDataList().Music ? 1 : 0];
        string updatedJson = JsonUtility.ToJson(spawnTiles.GetLevelDataList());
        spawnTiles.SetLevelDataJson(updatedJson);
    }
}
