using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public Button correctButton;
    public Button[] allButtons;

    public string nextSceneName;
    public string wrongSceneName;
    public void CorrectAnswer()
    {
        StartCoroutine(CorrectRoutine());
    }

    public void WrongAnswer()
    {
        StartCoroutine(WrongRoutine());
    }

    IEnumerator CorrectRoutine()
    {
        correctButton.image.color = Color.green;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator WrongRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(wrongSceneName);
    }
}