using UnityEngine.SceneManagement;
using UnityEngine;

public class GameOverButtons : MonoBehaviour
{
    public void ReplayGame()
    {
        PlayerController.gameOver = false;
        SceneManager.LoadScene("mainscene");
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}