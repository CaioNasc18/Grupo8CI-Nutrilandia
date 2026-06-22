using UnityEngine.SceneManagement;
public static class PuzzleManager
{
    private static int F_rightAnswers = 0;
    private static int F_wrongAnswers = 0;
    public static void IncrementRightAnswer()
    {
        F_rightAnswers++;
        if (F_rightAnswers == 9)
        {
            SceneManager.LoadScene("PuzzleResultado");
        }
    }
    public static void IncrementWrongAnswer()
    {
        F_wrongAnswers++;
    }
    public static int GetRightAnswer()
    {
        return F_rightAnswers;
    }
    public static int GetWrongAnswer()
    {
        return F_wrongAnswers;
    }
    public static void Reset()
    {
        F_rightAnswers = 0;
        F_wrongAnswers = 0;
    }
}