using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItemFloresta : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string foodType; // ex: "Fruta", "Vegetal", "Laticinio", "Doce"

    private Vector3 startPosition;
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // a verificação de "acertou/errou" acontece no Basket (via OnDrop)
    }

    public void ReturnToStart()
    {
        rectTransform.position = startPosition;
    }
}