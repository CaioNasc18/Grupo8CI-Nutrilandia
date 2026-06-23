using UnityEngine;

public class FoodItem : MonoBehaviour
{
    public string foodType; // ex: "Fruta", "Vegetal", "Lixo"

    private Vector3 startPosition;
    private Vector3 offset;
    private bool isDragging;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint = cam.ScreenToWorldPoint(mousePoint);
        mousePoint.z = 0;
        return mousePoint;
    }

    public void ReturnToStart()
    {
        transform.position = startPosition;
    }
}