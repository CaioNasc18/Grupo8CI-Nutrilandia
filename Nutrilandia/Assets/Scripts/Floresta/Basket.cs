using UnityEngine;

public class Basket : MonoBehaviour
{
    public string acceptedType; // tem de coincidir com foodType do alimento

    void OnTriggerEnter2D(Collider2D other)
    {
        FoodItem food = other.GetComponent<FoodItem>();
        if (food == null) return;

        if (food.foodType == acceptedType)
        {
            // Acertou!
            food.transform.position = transform.position;
            food.GetComponent<Collider2D>().enabled = false; // já não pode ser arrastado outra vez
            Debug.Log("Correto: " + food.foodType);
            // Aqui podes chamar som, pontuação, animação, etc.
        }
        else
        {
            // Errou
            food.ReturnToStart();
            Debug.Log("Errado!");
        }
    }
}