using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
{
    public void NextDifficulty()
    {
        SceneManager.LoadScene("MediumScene");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}