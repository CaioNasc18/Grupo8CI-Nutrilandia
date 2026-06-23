using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ReiMandaDificil : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public RectTransform basket;          // o cesto
    public RectTransform spawnArea;       // área onde os alimentos caem

    [Header("Alimentos")]
    public FoodItem[] foods;
    public GameObject foodPrefab;         // prefab do alimento que cai

    [Header("Cenas")]
    public string victoryScene;
    public string failScene;

    [Header("Configurações")]
    public float gameDuration = 45f;
    public float spawnInterval = 1.2f;
    public float fallSpeed = 300f;
    public float speedIncreaseRate = 20f; // aumenta por segundo

    private int lives = 3;
    private float timer;
    private bool gameActive = false;
    private List<GameObject> fallingFoods = new List<GameObject>();

    void Start()
    {
        UpdateLivesText();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        timer = gameDuration;
        gameActive = true;
        StartCoroutine(SpawnFoods());

        while (timer > 0 && gameActive)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            fallSpeed += speedIncreaseRate * Time.deltaTime;
            yield return null;
        }

        if (lives > 0)
            SceneManager.LoadScene(victoryScene);
    }

    IEnumerator SpawnFoods()
    {
        while (gameActive && timer > 0)
        {
            SpawnFood();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnFood()
    {
        FoodItem food = foods[Random.Range(0, foods.Length)];

        GameObject obj = Instantiate(foodPrefab, spawnArea);
        RectTransform rt = obj.GetComponent<RectTransform>();

        // Posição aleatória no topo
        float x = Random.Range(-spawnArea.rect.width / 2f + 50f, spawnArea.rect.width / 2f - 50f);
        rt.anchoredPosition = new Vector2(x, spawnArea.rect.height / 2f);

        // Mete o sprite
        obj.GetComponent<Image>().sprite = food.sprite;

        // Guarda se é saudável
        FallingFood ff = obj.AddComponent<FallingFood>();
        ff.isHealthy = food.isHealthy;
        ff.manager = this;

        fallingFoods.Add(obj);
    }

    void Update()
    {
        if (!gameActive) return;

        // Move o cesto com o rato
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            spawnArea, Input.mousePosition, null, out mousePos);
        basket.anchoredPosition = new Vector2(
            Mathf.Clamp(mousePos.x, -spawnArea.rect.width / 2f + 80f, spawnArea.rect.width / 2f - 80f),
            basket.anchoredPosition.y);

        // Move os alimentos para baixo
        foreach (GameObject food in fallingFoods)
        {
            if (food == null) continue;
            RectTransform rt = food.GetComponent<RectTransform>();
            rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

            // Verifica colisão com o cesto
            if (RectOverlap(rt, basket))
            {
                FallingFood ff = food.GetComponent<FallingFood>();
                if (ff.isHealthy)
                {
                    // Apanhou saudável — bom!
                    Destroy(food);
                }
                else
                {
                    // Apanhou guloseima — perde vida!
                    LoseLife();
                    Destroy(food);
                }
            }

            // Caiu no chão
            if (rt.anchoredPosition.y < -spawnArea.rect.height / 2f)
            {
                FallingFood ff = food.GetComponent<FallingFood>();
                if (ff.isHealthy)
                {
                    // Deixou cair saudável — perde vida!
                    LoseLife();
                }
                Destroy(food);
            }
        }

        fallingFoods.RemoveAll(f => f == null);
    }

    bool RectOverlap(RectTransform a, RectTransform b)
    {
        Rect ra = GetWorldRect(a);
        Rect rb = GetWorldRect(b);
        return ra.Overlaps(rb);
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y,
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y);
    }

    void LoseLife()
    {
        lives--;
        UpdateLivesText();
        if (lives <= 0)
        {
            gameActive = false;
            SceneManager.LoadScene(failScene);
        }
    }

    void UpdateLivesText()
    {
        livesText.text = lives == 3 ? "Vidas: 3" :
                         lives == 2 ? "Vidas: 2" :
                         lives == 1 ? "Vidas: 1" : "Vidas: 0";
    }
}

public class FallingFood : MonoBehaviour
{
    public bool isHealthy;
    public ReiMandaDificil manager;
}