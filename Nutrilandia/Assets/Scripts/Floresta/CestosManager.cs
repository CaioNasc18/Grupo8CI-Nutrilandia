using UnityEngine;
using UnityEngine.SceneManagement;

public class CestosManager : MonoBehaviour
{
    public static CestosManager Instance;

    public int totalFoods;
    private int placedCorrectly = 0;

    public string nextSceneName; // nome exato da Scene a carregar ao ganhar

    void Start()
    {
        FoodItemFloresta[] foods = FindObjectsByType<FoodItemFloresta>(FindObjectsSortMode.None);
        Debug.Log("Alimentos encontrados: " + foods.Length);
        foreach (FoodItemFloresta f in foods)
        {
            Debug.Log("- " + f.gameObject.name + " | Tipo: " + f.foodType);
        }
        totalFoods = foods.Length;
        Debug.Log("Total Foods definido para: " + totalFoods);
    }

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

