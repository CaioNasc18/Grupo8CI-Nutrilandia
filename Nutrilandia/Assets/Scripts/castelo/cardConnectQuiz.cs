using UnityEngine;

public class cardConnectQuiz : MonoBehaviour
{
    public bool isLeft;

    [HideInInspector]
    public bool isConnected = false;

    public string id; // Example: "Laticinios", "Frutas", etc.

    public void Click()
    {
        MatchManagerQuiz.Instance.Select(this);
    }

}
