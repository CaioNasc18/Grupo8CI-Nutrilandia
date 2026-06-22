using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ScoreScriptFloresta : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textField;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        textField.text = GameManagerFloresta.GetScore().ToString();
    }
}