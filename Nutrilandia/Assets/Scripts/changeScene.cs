using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public void ChangeSceneAction(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}