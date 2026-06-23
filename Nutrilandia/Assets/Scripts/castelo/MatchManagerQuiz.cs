using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchManagerQuiz : MonoBehaviour
{
    public static MatchManagerQuiz Instance;
    public GameObject linePrefab;
    public Transform canvasTransform;
    public string nextScene;
    public string failScene;
    public int totalPairs = 4; // número de pares a ligar

    private cardConnectQuiz selectedLeft;
    Dictionary<cardConnectQuiz, cardConnectQuiz> matches =
        new Dictionary<cardConnectQuiz, cardConnectQuiz>();

    void Awake()
    {
        Instance = this;
    }

    public void Select(cardConnectQuiz point)
    {
        if (point.isLeft)
        {
            if (point.isConnected) return;
            selectedLeft = point;
        }
        else
        {
            if (selectedLeft == null) return;
            if (point.isConnected) return;

            CreateConnection(selectedLeft, point);
            selectedLeft.isConnected = true;
            point.isConnected = true;
            matches.Add(selectedLeft, point);
            selectedLeft = null;

            CheckAnswers();
        }
    }

    void CreateConnection(cardConnectQuiz left, cardConnectQuiz right)
    {
        GameObject line = Instantiate(linePrefab);
        line.transform.SetParent(canvasTransform, false);
        Connection connection = line.GetComponent<Connection>();
        connection.start = left.GetComponent<RectTransform>();
        connection.end = right.GetComponent<RectTransform>();
    }

    void CheckAnswers()
    {
        if (matches.Count < totalPairs) return; // espera que todos estejam ligados

        foreach (var pair in matches)
        {
            if (pair.Key.id != pair.Value.id)
            {
                SceneManager.LoadScene(failScene);
                return;
            }
        }
        SceneManager.LoadScene(nextScene);
    }
}