using UnityEngine;

public class ImageTrigger : MonoBehaviour
{
    public bool isObstacle; // ✓ nas imagens que não se pode tocar

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isObstacle)
            Debug.Log("Perdeste!");
        else
            Debug.Log("Checkpoint!");
    }
}