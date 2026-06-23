using UnityEngine.SceneManagement;
public static class PuzzleManagerDificil
{
    private static int D_rightAnswers = 0;
    private static int D_wrongAnswers = 0;
    public static void IncrementRightAnswer()
    {
        D_rightAnswers++;
        if (D_rightAnswers == 16)
        {
            SceneManager.LoadScene("PuzzleResultado");
        }
    }
    public static void IncrementWrongAnswer()
    {
        D_wrongAnswers++;
    }
    public static int GetRightAnswer()
    {
        return D_rightAnswers;
    }
    public static int GetWrongAnswer()
    {
        return D_wrongAnswers;
    }
    public static void Reset()
    {
        D_rightAnswers = 0;
        D_wrongAnswers = 0;
    }
}