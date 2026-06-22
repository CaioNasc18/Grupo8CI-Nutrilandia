using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Coloca este script em cada um dos 3 montinhos de comida (Images).
/// Quando clicado, avisa o PlateGameManager para iniciar o drag.
/// </summary>
public class FoodPileClickHandler : MonoBehaviour, IPointerDownHandler  // ← era IPointerClickHandler
{
    [SerializeField] private PlateGameManager gameManager;
    [SerializeField] private PlateGameManager.FoodType foodType;
    [SerializeField] private Sprite portionSprite;

    public void OnPointerDown(PointerEventData eventData)
{
    Debug.Log("CLICOU: " + foodType);
    gameManager.OnFoodPileClicked(foodType, portionSprite);
}
}

