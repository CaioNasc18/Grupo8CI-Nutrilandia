using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItemFloresta : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string foodType;
    private Vector3 startPosition;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.position;
        Debug.Log("FoodItem iniciado: " + gameObject.name + " | Tipo: " + foodType);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Começou a arrastar: " + gameObject.name);
        transform.SetAsLastSibling();
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Largou: " + gameObject.name);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void ReturnToStart()
    {
        rectTransform.position = startPosition;
    }
}