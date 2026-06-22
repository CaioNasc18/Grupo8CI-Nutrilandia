using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    // Ao mudar para public, a "caixinha" aparece no Inspector do Unity
    public TMP_Text timerText; 
    
    private float currentTimer;
    private bool isCounting;

    void Start()
    {
        // Já não precisamos do GetComponent<TMP_Text>() porque vamos arrastar manualmente
        currentTimer = 0;
        isCounting = true;

        // Aviso amigável caso te esqueças de arrastar o texto no Unity
        if (timerText == null)
        {
            Debug.LogError("Cuidado! Esqueceste-te de arrastar o objeto de Texto para o campo Timer Text no Inspector!");
        }
    }

    void Update()
{
    if (!isCounting || timerText == null)
    {
        return;
    }

    currentTimer += Time.deltaTime;

   
    int seconds = Mathf.FloorToInt(currentTimer);
    
    
    timerText.text = $"{seconds:000}";
}


    public int GetTimerAndStop()
    {
        isCounting = false;
        return (int)currentTimer;
    }
}