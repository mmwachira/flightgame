using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void StartGame()
    {
        ManagerUI.Instance.ShowGameplayHUD();
        ManagerLevel.Instance.StartGame();
        ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicGameplay);
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    public void Start()
    {
        ManagerUI.Instance.ShowStartGameScreen();
        ManagerLevel.Instance.Setup();
        ManagerSounds.Instance.PlayMusic(ManagerSounds.Instance.MusicMenu);
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
    
    public void ReplayGame()
    {
        SceneManager.LoadScene("mainscene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}