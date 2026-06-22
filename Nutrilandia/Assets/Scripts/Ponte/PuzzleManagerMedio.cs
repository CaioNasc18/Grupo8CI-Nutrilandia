using UnityEngine.SceneManagement;
public static class PuzzleManagerMedio
{
    private static int M_rightAnswers = 0;
    private static int M_wrongAnswers = 0;
    public static void IncrementRightAnswer()
    {
        M_rightAnswers++;
        if (M_rightAnswers == 16)
        {
            SceneManager.LoadScene("PuzzleResultado");
        }
    }
    public static void IncrementWrongAnswer()
    {
        M_wrongAnswers++;
    }
    public static int GetRightAnswer()
    {
        return M_rightAnswers;
    }
    public static int GetWrongAnswer()
    {
        return M_wrongAnswers;
    }
    public static void Reset()
    {
        M_rightAnswers = 0;
        M_wrongAnswers = 0;
    }
}