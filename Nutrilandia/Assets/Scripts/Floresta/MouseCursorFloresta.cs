using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;
        transform.position = pos;
    }
}