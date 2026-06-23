using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScriptFloresta : MonoBehaviour
{
    [SerializeField]
    private bool isHealthy = false;

    private GameRunnerFloresta gameRunner;

    void Start()
    {
        gameRunner = FindFirstObjectByType<GameRunnerFloresta>();

        if (gameRunner == null)
            Debug.LogError("GameRunnerFloresta não encontrado na cena!", this);
    }

    public void OnHit()
    {
        if (gameRunner == null) return;

        if (isHealthy)
            gameRunner.IncrementWrongAnswer();
        else
            gameRunner.IncrementRightAnswer();
    }

    public void OnMiss()
    {
        if (gameRunner == null) return;

        gameRunner.IncrementWrongAnswer();
    }
}