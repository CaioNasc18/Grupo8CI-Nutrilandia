using UnityEngine;
using UnityEngine.EventSystems;

public class Basket : MonoBehaviour, IDropHandler
{
    public string acceptedType;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop chamado no cesto: " + gameObject.name);

        GameObject dropped = eventData.pointerDrag;
        if (dropped == null)
        {
            Debug.Log("dropped é null!");
            return;
        }

        FoodItemFloresta food = dropped.GetComponent<FoodItemFloresta>();
        if (food == null)
        {
            Debug.Log("FoodItemFloresta não encontrado!");
            return;
        }

        Debug.Log("Tipo do alimento: " + food.foodType + " | Tipo aceite: " + acceptedType);

        if (food.foodType.Trim().ToLower() == acceptedType.Trim().ToLower())
        {
            food.GetComponent<RectTransform>().position = transform.position;
            food.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Debug.Log("Correto: " + food.foodType);
            CestosManager.Instance.FoodPlacedCorrectly();
        }
        else
        {
            food.ReturnToStart();
            Debug.Log("Errado! Esperado: " + acceptedType + " | Recebido: " + food.foodType);
        }
    }
}