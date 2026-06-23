using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinalScriptManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text rightAnswersText;
    [SerializeField]
    private TMP_Text wrongAnswersText;
    public void Start()
    {
        rightAnswersText.text = PuzzleManager.GetRightAnswer().ToString();
        rightAnswersText.text = PuzzleManagerMedio.GetRightAnswer().ToString();
        rightAnswersText.text = PuzzleManagerDificil.GetRightAnswer().ToString();
        wrongAnswersText.text = PuzzleManager.GetWrongAnswer().ToString();
        wrongAnswersText.text = PuzzleManagerMedio.GetWrongAnswer().ToString();
        wrongAnswersText.text = PuzzleManagerDificil.GetWrongAnswer().ToString();
    }
}