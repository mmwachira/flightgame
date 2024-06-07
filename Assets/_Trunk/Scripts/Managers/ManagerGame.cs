using UnityEngine;

public class ManagerGame : MonoBehaviour
{
    public static ManagerGame Instance { get; private set; }

    private bool _gameOver;
    private bool _gamePaused;

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
        ManagerEducationalContent.Instance.LoadEducationalQuestions();
        ManagerLevel.Instance.Setup();
    }

    void Update()
    {
        if (_gameOver)
        {
            //Time.timeScale = 0;
            ManagerLevel.Instance.EndGame();
            ManagerUI.Instance.ShowGameOverScreen();
            ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicMenu);
        }

        if (_gamePaused)
        {
            Time.timeScale = 0;
            ManagerUI.Instance.ShowMenu();

        }
    }

    private void ResetGame()
    {
        _gameOver = false;
        _gamePaused = false;
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

    public void PauseGame()
    {
        _gamePaused = true;
    }

    public void ResumeGame()
    {
        _gamePaused = false;
        Time.timeScale = 1;
        ManagerUI.Instance.ShowGameplayHUD();
        

    }

    public void GameOver()
    {
        _gameOver = true;
        ManagerUI.Instance.UpdateFinalScore((int)ManagerLevel.Instance.TotalRunDistance);
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