using UnityEngine;
using UnityEngine.SceneManagement;

public class CestosManager : MonoBehaviour
{
    public static CestosManager Instance;

    public int totalFoods;
    private int placedCorrectly = 0;

    public string nextSceneName; // nome exato da Scene a carregar ao ganhar

    void Awake()
    {
        Instance = this;
    }

    public void FoodPlacedCorrectly()
    {
        placedCorrectly++;
        Debug.Log("Colocados corretamente: " + placedCorrectly + " / " + totalFoods);

        if (placedCorrectly >= totalFoods)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        Debug.Log("Parabéns, completaste o jogo! A carregar: " + "FimJardimEncantado");
        SceneManager.LoadScene("FimJardimEncantado");
    }
}