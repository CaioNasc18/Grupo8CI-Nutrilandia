using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRunnerFloresta : MonoBehaviour
{
    [Header("Tempo das toupeiras")]
    [Range(0.5f, 5f)]
    [SerializeField]
    private float moleTimer = 2f;

    [Header("Fade das toupeiras")]
    [SerializeField]
    private float fadeDuration = 0.5f;

    [Header("Tempo do jogo")]
    [Range(10, 120)]
    [SerializeField]
    private int matchTimer = 30;

    [Header("Lista de alimentos/toupeiras")]
    [SerializeField]
    private List<GameObject> listMoles;

    private System.Random randomNumberGenerator;
    private float currentTimer;
    private bool gameEnded = false;

    [Header("UI")]
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private TMP_Text scoreText;

    private int score = 0;

    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("timerText não atribuído no Inspector!", this);
            return;
        }
        if (listMoles == null || listMoles.Count == 0)
        {
            Debug.LogError("listMoles vazia no Inspector!", this);
            return;
        }

        randomNumberGenerator = new System.Random();
        currentTimer = 0;
        gameEnded = false;
        GameManagerFloresta.Reset();
        listMoles.ForEach(item => item.SetActive(false));
        UpdateScoreText();
        StartCoroutine(ChangeMole());
    }

    IEnumerator ChangeMole()
    {
        Debug.Log("ChangeMole iniciado");
        int lastRandom = -1;

        while (currentTimer < matchTimer)
        {
            listMoles.ForEach(item => item.SetActive(false));
            yield return new WaitForSeconds(fadeDuration);

            // Garante que não escolhe o mesmo objeto duas vezes seguidas
            int randomNumber;
            do
            {
                randomNumber = randomNumberGenerator.Next(0, listMoles.Count);
            } while (randomNumber == lastRandom && listMoles.Count > 1);

            lastRandom = randomNumber;
            Debug.Log($"A mostrar objeto {randomNumber}");
            listMoles[randomNumber].SetActive(true);

            yield return new WaitForSeconds(moleTimer);
        }

        listMoles.ForEach(item => item.SetActive(false));
    }

    void Update()
    {
        if (gameEnded) return;

        currentTimer += Time.deltaTime;

        float numberOfSeconds = matchTimer - currentTimer;
        numberOfSeconds = Mathf.Clamp(numberOfSeconds, 0, matchTimer);
        timerText.text = $"{numberOfSeconds:00}";

        if (currentTimer >= matchTimer)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        listMoles.ForEach(item => item.SetActive(false));
        StopAllCoroutines();
        SceneManager.LoadScene("ToupeiraResultado");
    }

    public void IncrementRightAnswer()
    {
        if (gameEnded) return;
        GameManagerFloresta.IncrementRightAnswer();
        score++;
        UpdateScoreText();
    }

    public void IncrementWrongAnswer()
    {
        if (gameEnded) return;
        GameManagerFloresta.IncrementWrongAnswer();
        score--;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Pontos: " + score;
    }

    public int GetScore()
    {
        return score;
    }
}