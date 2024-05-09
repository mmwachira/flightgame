using UnityEngine;

public class ManagerGame : MonoBehaviour
{
    public static ManagerGame Instance { get; private set; }

    private bool _gameOver;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        ResetGame();
        ManagerLevel.Instance.Setup();
    }

    void Update()
    {
        if (_gameOver)
        {
            Time.timeScale = 0;
            ManagerUI.Instance.ShowGameOverScreen();
            ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicMenu);
        }
    }

    private void ResetGame()
    {
        _gameOver = false;
        ManagerUI.Instance.ShowStartGameScreen();
        ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicMenu); 
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        ManagerUI.Instance.ShowGameplayHUD();
        ManagerLevel.Instance.StartGame();
        ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicGameplay);
    }

    public void GameOver()
    {
        _gameOver = true;
    }
    
    public void ReplayGame()
    {
        ResetGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}