using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SpawnTiles _spawnTiles;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private GameObject _gameScenePrefab;
    [SerializeField] private TMP_Text _txtStar;
    [SerializeField] private TMP_Text _txtStarWin;
    [SerializeField] private TMP_Text _txtClock;
    [SerializeField] private TMP_Text _txtLevel;
    [SerializeField] private TMP_Text _txtHomeLevel;
    [SerializeField] private TMP_Text _txtHomeCoin;
    [SerializeField] private TMP_Text _txtHomeStar;
    [SerializeField] private TMP_Text _txtLevelLose;
    [SerializeField] private GameObject _backgroundPause;
    [SerializeField] private GameObject _backgroundLose;
    [SerializeField] private GameObject _backgroundWin;
    [SerializeField] private GameObject _backgroundSeting;
    [SerializeField] private GameObject _backgroundGamePlay;
    [SerializeField] private GameObject _backgroundGameHome;
    [SerializeField] private GameObject _gameScene;
    [SerializeField] private Sprite[] _backgroundSound;
    [SerializeField] private Sprite[] _backgroundMusic;
    [SerializeField] private Image _backgroundSoundButton;
    [SerializeField] private Image _backgroundMusicButton;
    [SerializeField] private Image _backgroundSetingSoundButton;
    [SerializeField] private Image _backgroundSetingMusicButton;
    [SerializeField] private float _countdownTime = 600f;
    private bool _isPaused = false;
    private void Awake()
    {
        SpawnGameScene(false);
        _spawnTiles.SetLevelDataList();
    }
    private void Start()
    {
        _txtHomeStar.text = $"{_spawnTiles.GetLevelDataList().Star}";
        _txtHomeCoin.text = $"{_spawnTiles.GetLevelDataList().Coin}";
        _txtHomeLevel.text = $"{_spawnTiles.GetLevelDataList().Level}";
        _txtLevel.text = $"Lv.{_spawnTiles.GetLevelDataList().Level}";
        _txtStar.text = "0";
        _backgroundSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        _backgroundMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        _backgroundSetingSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        _backgroundSetingMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
    }
    private void Update()
    {
        UpdateTimerUI();
    }
    private void UpdateTimerUI()
    {
        if (_backgroundLose.activeInHierarchy) return;
        _countdownTime -= Time.deltaTime;
        if (_countdownTime < 0)
        {
            _countdownTime = 0;
            SetActiveLose(true);
        }
        int minutes = Mathf.FloorToInt(_countdownTime / 60);
        int seconds = Mathf.FloorToInt(_countdownTime % 60);
        _txtClock.text = string.Format("{0:00}:{1:00}", minutes, seconds); ;
    }
    public void SetStar(int star)
    {
        _txtStar.text = $"{star}";
    }

    public void SetActiveWin(bool active, int star)
    {
        _backgroundWin.SetActive(active);
        _txtStarWin.text = $"+{star}";
        Time.timeScale = 0;
    }
    private void SpawnGameScene(bool active)
    {
        if (_gameScene != null) Destroy(_gameScene);
        GameObject newGameScene = Instantiate(_gameScenePrefab);
        newGameScene.name = _gameScenePrefab.name;
        _spawnTiles = newGameScene.GetComponentInChildren<SpawnTiles>();
        _gameScene = newGameScene;
        newGameScene.SetActive(active);
    }
    public void SetActiveLose(bool active)
    {
        _backgroundLose.SetActive(active);
        _isPaused = active;
        _txtLevelLose.text = $"LEVEL {_spawnTiles.GetLevelDataList().Level}";
        _countdownTime = 0;
        _audioManager.PlaySFX("Lose");
    }

    public bool GetPause()
    {
        return _isPaused;
    }
    public void Pause()
    {
        _isPaused = true;
        _backgroundPause.SetActive(true);
        _backgroundWin.SetActive(false);
        _backgroundLose.SetActive(false);
        Time.timeScale = 0;
        _audioManager.PlaySFX("Button");
    }
    public void Continute()
    {
        _isPaused = false;
        _backgroundPause.SetActive(false);
        Time.timeScale = 1f;
        if ($"LV.{_spawnTiles.GetLevelDataList().Level}" != _txtLevel.text) PlayGame();
        _audioManager.PlaySFX("Button");
    }
    public void OpenSeting()
    {
        _backgroundSeting.SetActive(true);
        _audioManager.PlaySFX("Button");
    }
    public void CloseSeting()
    {
        _backgroundSeting.SetActive(false);
        _audioManager.PlaySFX("Button");
    }
    public void PlayGame()
    {
        _txtStar.text = "0";
        _txtLevel.text = $"LV.{_spawnTiles.GetLevelDataList().Level}";
        _countdownTime = 540f + (_spawnTiles.GetLevelDataList().Level * 60);
        _isPaused = false;
        Time.timeScale = 1f;
        _backgroundGamePlay.SetActive(true);
        _backgroundGameHome.SetActive(false);
        _backgroundWin.SetActive(false);
        _backgroundLose.SetActive(false);
        _backgroundSeting.SetActive(false);
        SpawnGameScene(true);
        _audioManager.PlaySFX("Button");
    }
    public void GoHome()
    {
        _countdownTime = 120f;
        _isPaused = false;
        _backgroundGameHome.SetActive(true);
        _backgroundGamePlay.SetActive(false);
        _backgroundPause.SetActive(false);
        _backgroundLose.SetActive(false);
        Start();
        Destroy(_gameScene);
        _audioManager.PlaySFX("Button");
    }
    public void Sound()
    {
        _spawnTiles.GetLevelDataList().Sound = !_spawnTiles.GetLevelDataList().Sound;
        _backgroundSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        _backgroundSetingSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
        _spawnTiles.SetLevelDataJson(updatedJson);
        _audioManager.PlaySFX("Button");
    }

    public void Music()
    {
        _spawnTiles.GetLevelDataList().Music = !_spawnTiles.GetLevelDataList().Music;
        _backgroundMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        _backgroundSetingMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
        _spawnTiles.SetLevelDataJson(updatedJson);
        _audioManager.PlaySFX("Button");
    }
}
