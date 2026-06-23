using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class ReiMandaDificil : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI reiText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;

    [Header("Click Round")]
    public Button foodButton;
    public TextMeshProUGUI foodButtonText;

    [Header("Sequence Round")]
    public GameObject sequencePanel;
    public Button[] sequenceButtons;
    public TextMeshProUGUI[] sequenceButtonTexts;

    [Header("Drag Round")]
    public GameObject dragPanel;
    public RectTransform draggableFood;    // o alimento que se arrasta
    public TextMeshProUGUI draggableText;
    public RectTransform healthyZone;      // zona saudável
    public RectTransform unhealthyZone;    // zona não saudável
    public TextMeshProUGUI healthyZoneText;
    public TextMeshProUGUI unhealthyZoneText;

    [Header("Cenas")]
    public string victoryScene;
    public string failScene;

    [Header("Configurações")]
    public float timePerRound = 3f;
    public int totalRounds = 12;

    private List<string> healthyFoods = new List<string> { "Maçã", "Cenoura", "Brócolo", "Banana", "Alface", "Laranja" };
    private List<string> unhealthyFoods = new List<string> { "Bolo", "Gomas", "Chocolate", "Chips", "Rebuçados", "Donut" };

    private int lives = 3;
    private float timer;
    private bool waitingForClick = false;
    private bool isHealthy;
    private bool dragAnswered = false;

    void Start()
    {
        UpdateLivesText();
        healthyZoneText.text = "Saudável 🥦";
        unhealthyZoneText.text = "Guloseima 🍬";
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        for (int i = 0; i < totalRounds; i++)
        {
            float rand = Random.value;
            if (rand < 0.33f)
                yield return StartCoroutine(ClickRound());
            else if (rand < 0.66f)
                yield return StartCoroutine(SequenceRound());
            else
                yield return StartCoroutine(DragRound());

            if (lives <= 0) yield break;
            yield return new WaitForSeconds(0.4f);
        }

        SceneManager.LoadScene(victoryScene);
    }

    IEnumerator ClickRound()
    {
        isHealthy = Random.value > 0.5f;
        string food = isHealthy
            ? healthyFoods[Random.Range(0, healthyFoods.Count)]
            : unhealthyFoods[Random.Range(0, unhealthyFoods.Count)];

        reiText.text = "Rei manda clicar nos alimentos SAUDÁVEIS!";
        foodButtonText.text = food;
        foodButton.gameObject.SetActive(true);
        sequencePanel.SetActive(false);
        dragPanel.SetActive(false);

        timer = timePerRound;
        waitingForClick = true;

        while (waitingForClick && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        if (waitingForClick && isHealthy) LoseLife();
        waitingForClick = false;
        foodButton.gameObject.SetActive(false);
    }

    IEnumerator SequenceRound()
    {
        int correctIndex = Random.Range(0, 4);
        reiText.text = "Rei manda escolher o alimento SAUDÁVEL!";
        sequencePanel.SetActive(true);
        foodButton.gameObject.SetActive(false);
        dragPanel.SetActive(false);

        for (int i = 0; i < sequenceButtons.Length; i++)
        {
            sequenceButtonTexts[i].text = i == correctIndex
                ? healthyFoods[Random.Range(0, healthyFoods.Count)]
                : unhealthyFoods[Random.Range(0, unhealthyFoods.Count)];
        }

        timer = timePerRound;
        waitingForClick = true;

        while (waitingForClick && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        if (waitingForClick) LoseLife();
        waitingForClick = false;
        sequencePanel.SetActive(false);
    }

    IEnumerator DragRound()
    {
        isHealthy = Random.value > 0.5f;
        string food = isHealthy
            ? healthyFoods[Random.Range(0, healthyFoods.Count)]
            : unhealthyFoods[Random.Range(0, unhealthyFoods.Count)];

        reiText.text = "Rei manda arrastar para o lugar certo!";
        draggableText.text = food;
        dragPanel.SetActive(true);
        foodButton.gameObject.SetActive(false);
        sequencePanel.SetActive(false);
        dragAnswered = false;

        // Reset posição
        draggableFood.anchoredPosition = Vector2.zero;

        timer = timePerRound;

        while (!dragAnswered && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        if (!dragAnswered) LoseLife();
        dragPanel.SetActive(false);
    }

    // Chama isto quando o alimento é largado numa zona
    public void OnDroppedOnZone(bool droppedOnHealthy)
    {
        if (dragAnswered) return;
        dragAnswered = true;
        if (droppedOnHealthy != isHealthy) LoseLife();
    }

    public void OnFoodClicked()
    {
        if (!waitingForClick) return;
        if (!isHealthy) LoseLife();
        waitingForClick = false;
    }

    public void OnSequenceClicked(int index)
    {
        if (!waitingForClick) return;
        string clicked = sequenceButtonTexts[index].text;
        if (!healthyFoods.Contains(clicked)) LoseLife();
        waitingForClick = false;
    }

    void LoseLife()
    {
        lives--;
        UpdateLivesText();
        if (lives <= 0) SceneManager.LoadScene(failScene);
    }

    void UpdateLivesText()
    {
        livesText.text = "Vidas: " + lives;
    }
}