using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ReiMandaMedio : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI reiText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI timerText;

    [Header("Sequence")]
    public Button[] sequenceButtons;
    public Image[] sequenceButtonImages;

    [Header("Alimentos")]
    public FoodItem[] foods;

    [Header("Cenas")]
    public string victoryScene;
    public string failScene;

    [Header("Configurações")]
    public float roundTime = 8f;
    public float flashDuration = 0.6f;
    public int totalRounds = 5;
    public int sequenceLength = 5;

    private int lives = 3;
    private float timer;
    private List<int> sequence = new List<int>();
    private int sequenceStep = 0;
    private bool playerTurn = false;

    void Start()
    {
        UpdateLivesText();
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        for (int i = 0; i < totalRounds; i++)
        {
            yield return StartCoroutine(SequenceRound());
            if (lives <= 0) yield break;
            yield return new WaitForSeconds(0.5f);
        }

        SceneManager.LoadScene(victoryScene);
    }

    IEnumerator SequenceRound()
    {
        reiText.text = "Observa a sequência!";
        timerText.text = "";

        // Atribui sprites aleatórios aos 6 botões
        List<FoodItem> shuffled = new List<FoodItem>(foods);
        Shuffle(shuffled);
        for (int i = 0; i < sequenceButtons.Length; i++)
        {
            sequenceButtonImages[i].sprite = shuffled[i % shuffled.Count].sprite;
            sequenceButtons[i].interactable = false;
        }

        yield return new WaitForSeconds(0.5f);

        // Gera sequência aleatória de 5 dos 6 botões
        sequence.Clear();
        List<int> indices = new List<int> { 0, 1, 2, 3, 4, 5 };
        Shuffle(indices);
        for (int i = 0; i < sequenceLength; i++)
            sequence.Add(indices[i]);

        // Mostra a sequência a piscar
        foreach (int idx in sequence)
        {
            yield return StartCoroutine(FlashButton(idx));
            yield return new WaitForSeconds(0.2f);
        }

        // Vez do jogador
        reiText.text = "Repete a sequência!";
        sequenceStep = 0;
        playerTurn = true;

        foreach (var btn in sequenceButtons)
            btn.interactable = true;

        timer = roundTime;
        while (playerTurn && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        if (playerTurn) LoseLife();
        playerTurn = false;

        foreach (var btn in sequenceButtons)
            btn.interactable = false;
    }

    IEnumerator FlashButton(int idx)
    {
        Color original = sequenceButtonImages[idx].color;
        sequenceButtonImages[idx].color = Color.yellow;
        yield return new WaitForSeconds(flashDuration);
        sequenceButtonImages[idx].color = original;
    }

    public void OnSequenceButtonClicked(int idx)
    {
        if (!playerTurn) return;

        if (idx == sequence[sequenceStep])
        {
            sequenceStep++;
            if (sequenceStep >= sequence.Count)
                playerTurn = false;
        }
        else
        {
            LoseLife();
            playerTurn = false;
        }
    }

    void LoseLife()
    {
        lives--;
        UpdateLivesText();
        if (lives <= 0) SceneManager.LoadScene(failScene);
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