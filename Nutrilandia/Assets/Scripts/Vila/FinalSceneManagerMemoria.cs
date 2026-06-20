using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class FinalSceneManager : MonoBehaviour
{
 [SerializeField]
 private TMP_Text answersText;
 public void Start()
 {
 answersText.text = GameManager.GetSeconds().ToString();
 }
 public void TestAgain()
 {
GameManager.ResetGame();    
 SceneManager.LoadScene("MainScene");
 }
} 