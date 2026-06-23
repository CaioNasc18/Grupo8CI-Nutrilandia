using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FoodItem
{
    public Sprite sprite;
    public bool isHealthy;
}

public class ReiMandaFacil : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI reiText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;
    public Button foodButtonPrefab;
    public RectTransform spawnArea;

    [Header("Alimentos")]
    public FoodItem[] foods; // defines no Inspector!

    [Header("Cenas")]
    public string victoryScene;
    public string failScene;

    [Header("Configurações")]
    public float roundTime = 10f;
    public int totalRounds = 5;

    private int lives = 3;
    private int healthyRemaining;
    private float timer;
    private bool roundActive = false;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    void Start()
    {
        UpdateLivesText();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        for (int i = 0; i < totalRounds; i++)
        {
            yield return StartCoroutine(StartRound());
            if (lives <= 0) yield break;
            yield return new WaitForSeconds(0.5f);
        }

        SceneManager.LoadScene(victoryScene);
    }

    IEnumerator StartRound()
    {
        reiText.text = "Rei manda clicar nos alimentos SAUDÁVEIS!";
        SpawnButtons();

        timer = roundTime;
        roundActive = true;

        while (roundActive && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        if (roundActive && healthyRemaining > 0)
            LoseLife();

        roundActive = false;
        ClearButtons();
    }

    void SpawnButtons()
    {
        ClearButtons();

        // Escolhe 4 saudáveis e 4 não saudáveis da lista
        List<FoodItem> healthy = new List<FoodItem>();
        List<FoodItem> unhealthy = new List<FoodItem>();

        foreach (var food in foods)
        {
            if (food.isHealthy) healthy.Add(food);
            else unhealthy.Add(food);
        }

        Shuffle(healthy);
        Shuffle(unhealthy);

        List<FoodItem> selected = new List<FoodItem>();
        for (int i = 0; i < Mathf.Min(4, healthy.Count); i++) selected.Add(healthy[i]);
        for (int i = 0; i < Mathf.Min(4, unhealthy.Count); i++) selected.Add(unhealthy[i]);
        Shuffle(selected);

        healthyRemaining = Mathf.Min(4, healthy.Count);

        foreach (var food in selected)
        {
            Button btn = Instantiate(foodButtonPrefab, spawnArea);
            RectTransform rt = btn.GetComponent<RectTransform>();

            float x = Random.Range(-spawnArea.rect.width / 2f + 80f, spawnArea.rect.width / 2f - 80f);
            float y = Random.Range(-spawnArea.rect.height / 2f + 40f, spawnArea.rect.height / 2f - 40f);
            rt.anchoredPosition = new Vector2(x, y);

            // Mete o sprite no botão
            btn.GetComponentInChildren<Image>().sprite = food.sprite;

            bool isHealthy = food.isHealthy;
            btn.onClick.AddListener(() => OnButtonClicked(btn.gameObject, isHealthy));

            spawnedButtons.Add(btn.gameObject);
        }
    }

    void OnButtonClicked(GameObject btn, bool isHealthy)
    {
        if (!roundActive) return;

        if (isHealthy)
        {
            healthyRemaining--;
            Destroy(btn);
            if (healthyRemaining <= 0)
                roundActive = false;
        }
        else
        {
            LoseLife();
            Destroy(btn);
        }
    }

    void ClearButtons()
    {
        foreach (GameObject btn in spawnedButtons)
            if (btn != null) Destroy(btn);
        spawnedButtons.Clear();
    }

    void LoseLife()
    {
        lives--;
        UpdateLivesText();
        if (lives <= 0)
        {
            ClearButtons();
            SceneManager.LoadScene(failScene);
        }
    }

    void UpdateLivesText()
    {
        livesText.text = lives == 3 ? "Vidas: 3" :
                         lives == 2 ? "Vidas: 2" :
                         lives == 1 ? "Vidas: 1" : "Vidas: 0";
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}