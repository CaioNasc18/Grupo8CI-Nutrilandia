using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRunner : MonoBehaviour
{
    [Header("Tempo das toupeiras")]
    [Range(0.5f, 3f)]
    [SerializeField]
    private float moleTimer = 1f;

    [Header("Tempo do jogo")]
    [Range(10, 120)]
    [SerializeField]
    private int matchTimer = 30;

    [Header("Lista de alimentos/toupeiras")]
    [SerializeField]
    private List<GameObject> listMoles;

    private System.Random randomNumberGenerator;
    private float currentTimer;

    [Header("UI")]
    [SerializeField]
    private TMP_Text timerText;

    [SerializeField]
    private TMP_Text scoreText;

    private int score = 0;

    void Start()
    {
        randomNumberGenerator = new System.Random();

        currentTimer = 0;

        // Esconde todas no início
        listMoles.ForEach(item => item.SetActive(false));

        UpdateScoreText();

        StartCoroutine(ChangeMole());
    }

    IEnumerator ChangeMole()
    {
        while (currentTimer < matchTimer)
        {
            // Esconde todas
            listMoles.ForEach(item => item.SetActive(false));

            // Escolhe uma aleatória
            int randomNumber = randomNumberGenerator.Next(0, listMoles.Count);

            // Mostra
            listMoles[randomNumber].SetActive(true);

            // Espera
            yield return new WaitForSeconds(moleTimer);
        }
    }

    void Update()
    {
        currentTimer += Time.deltaTime;

        // Atualiza tempo
        float numberOfSeconds = matchTimer - currentTimer;

        numberOfSeconds = Mathf.Clamp(numberOfSeconds, 0, matchTimer);

        timerText.text = $"{numberOfSeconds:00}";

        // Fim do jogo
        if (currentTimer >= matchTimer || score < 0)
        {
            SceneManager.LoadScene("FinalScene");
        }
    }

    // Alimento pouco saudável
    public void IncrementRightAnswer()
    {
        score++;

        UpdateScoreText();
    }

    // Alimento saudável
    public void IncrementWrongAnswer()
    {
        score--;

        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Pontos: " + score;
    }

    public int GetScore()
    {
        return score;
    }
}