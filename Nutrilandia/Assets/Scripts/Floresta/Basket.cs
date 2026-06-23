using UnityEngine;
using UnityEngine.EventSystems;

public class Basket : MonoBehaviour, IDropHandler
{
    public string acceptedType;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        FoodItemFloresta food = dropped.GetComponent<FoodItemFloresta>();
        if (food == null) return;

        if (food.foodType == acceptedType)
        {
            food.GetComponent<RectTransform>().position = transform.position;
            dropped.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Debug.Log("Correto: " + food.foodType);

            CestosManager.Instance.FoodPlacedCorrectly(); // ← nome atualizado
        }
        else
        {
            food.ReturnToStart();
            Debug.Log("Errado!");
        }
    }
}