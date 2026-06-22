using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlateGameManager : MonoBehaviour
{
    public enum FoodType { Vegetais, Arroz, Frango }

    [Header("Cursor Colher")]
    [SerializeField] private Texture2D spoonCursor;
    [SerializeField] private Vector2 spoonHotspot = new Vector2(16, 48);

    [Header("Montinhos de Comida (clicáveis)")]
    [SerializeField] private FoodPile[] foodPiles;

    [Header("Prato")]
    [SerializeField] private RectTransform plateDropZone;
    [SerializeField] private RectTransform plateStackParent;
    [SerializeField] private float stackOffsetY = 18f;

    [Header("Colher (UI que segue o rato)")]
    [SerializeField] private RectTransform spoonUI;
    [SerializeField] private Image spoonFoodImage;

    [Header("Capitão Cenoura - Diálogo de Erro/Vitória")]
    [SerializeField] private DialogManager dialogManager;

    [Header("Porções corretas")]
    [SerializeField] private int correctVegetais = 5;
    [SerializeField] private int correctArroz    = 4;
    [SerializeField] private int correctFrango   = 3;

    [Header("Próxima Cena")]
    [SerializeField] private string nextSceneName = "FimCapturarObjetos";

    // --- estado interno ---
    private Dictionary<FoodType, int> portionsOnPlate = new Dictionary<FoodType, int>()
    {
        { FoodType.Vegetais, 0 },
        { FoodType.Arroz,    0 },
        { FoodType.Frango,   0 }
    };

    private Dictionary<FoodType, List<GameObject>> stackedImages = new Dictionary<FoodType, List<GameObject>>()
    {
        { FoodType.Vegetais, new List<GameObject>() },
        { FoodType.Arroz,    new List<GameObject>() },
        { FoodType.Frango,   new List<GameObject>() }
    };

    private Dictionary<FoodType, int> maxPortions = new Dictionary<FoodType, int>()
    {
        { FoodType.Vegetais, 5 },
        { FoodType.Arroz,    4 },
        { FoodType.Frango,   3 }
    };

    private FoodType? heldFood = null;
    private bool isDragging    = false;
    private bool gameComplete  = false;

    // -------------------------------------------------------

    private void Start()
    {
        if (spoonCursor != null)
            Cursor.SetCursor(spoonCursor, spoonHotspot, CursorMode.Auto);

        spoonUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDragging)
        {
            spoonUI.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasReleasedThisFrame)
                TryDropOnPlate();
        }
    }

    // -------------------------------------------------------
    // Clique no montinho
    // -------------------------------------------------------

    public void OnFoodPileClicked(FoodType type, Sprite portionSprite)
    {
        if (isDragging || gameComplete) return;

        if (portionsOnPlate[type] >= maxPortions[type])
        {
            StartErrorDialog(GetMaxReachedMessage(type));
            return;
        }

        heldFood   = type;
        isDragging = true;

        spoonFoodImage.sprite = portionSprite;
        spoonUI.gameObject.SetActive(true);
        spoonUI.position = Mouse.current.position.ReadValue();
    }

    // -------------------------------------------------------
    // Drop no prato
    // -------------------------------------------------------

    private void TryDropOnPlate()
    {
        isDragging = false;
        spoonUI.gameObject.SetActive(false);

        if (heldFood == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (RectTransformUtility.RectangleContainsScreenPoint(plateDropZone, mousePos, null))
        {
            AddPortionToPlate(heldFood.Value);
            CheckIfComplete();
        }

        heldFood = null;
    }

    private void AddPortionToPlate(FoodType type)
    {
        // Não adiciona se já está no máximo
        if (portionsOnPlate[type] >= maxPortions[type]) return;

        portionsOnPlate[type]++;

        FoodPile pile = GetPileByType(type);
        if (pile == null) return;

        GameObject portionObj = new GameObject("Portion_" + type + "_" + portionsOnPlate[type]);
        portionObj.transform.SetParent(plateStackParent, false);

        Image img = portionObj.AddComponent<Image>();
        img.sprite = pile.portionSprite;
        img.SetNativeSize();

        portionObj.transform.localScale = Vector3.one * 0.35f;

        RectTransform rt = portionObj.GetComponent<RectTransform>();
        float xOffset = GetXOffsetForType(type);
        float yOffset = stackedImages[type].Count * stackOffsetY;
        rt.anchoredPosition = new Vector2(xOffset, yOffset);

        stackedImages[type].Add(portionObj);
        StartCoroutine(PopScale(rt));
    }

    private void CheckIfComplete()
    {
        // Não verifica se um diálogo já está ativo
        if (dialogManager.IsDialogActive) return;

        if (portionsOnPlate[FoodType.Vegetais] == correctVegetais &&
            portionsOnPlate[FoodType.Arroz]    == correctArroz    &&
            portionsOnPlate[FoodType.Frango]   >= 2 &&
            portionsOnPlate[FoodType.Frango]   <= correctFrango)
        {
            ValidateAnswer();
        }
    }

    // -------------------------------------------------------
    // Validação
    // -------------------------------------------------------

    private void ValidateAnswer()
    {
        string errorMsg = GetErrorMessage();

        if (errorMsg != null)
            StartErrorDialog(errorMsg);
        else
            StartCoroutine(WinSequence());
    }

    private string GetErrorMessage()
    {
        if (portionsOnPlate[FoodType.Vegetais] != correctVegetais)
            return "Hmm! Os vegetais precisam de " + correctVegetais + " colheres! Tenta de novo!";

        if (portionsOnPlate[FoodType.Arroz] != correctArroz)
            return "O arroz precisa de " + correctArroz + " colheres! Vamos corrigir!";

        if (portionsOnPlate[FoodType.Frango] < 2 || portionsOnPlate[FoodType.Frango] > correctFrango)
            return "O frango ou peixe precisa de 2 a 3 colheres! Quase lá!";

        return null;
    }

    private string GetMaxReachedMessage(FoodType type)
    {
        return "Já tens " + maxPortions[type] + " colheres de " + GetFoodName(type) + " no prato! Isso chega!";
    }

    // -------------------------------------------------------
    // Diálogo de erro / vitória
    // -------------------------------------------------------

    private void StartErrorDialog(string message)
    {
        StartCoroutine(ErrorSequence(message));
    }

    private IEnumerator ErrorSequence(string message)
    {
        if (dialogManager.IsDialogActive) yield break; 
        dialogManager.SetSingleLine("Capitão Cenoura", message);
        dialogManager.StartDialog();
        yield return new WaitUntil(() => !dialogManager.IsDialogActive);
        ClearPlate();
    }

    private IEnumerator WinSequence()
    {
        gameComplete = true;
        dialogManager.SetSingleLine("Capitão Cenoura", "Muito bem! Fizeste um prato perfeito! Estou muito orgulhoso de ti!");
        dialogManager.StartDialog();
        yield return new WaitUntil(() => !dialogManager.IsDialogActive);
        SceneManager.LoadScene(nextSceneName);
    }

    // -------------------------------------------------------
    // Limpar prato
    // -------------------------------------------------------

    private void ClearPlate()
    {
        foreach (var type in stackedImages.Keys)
        {
            foreach (var obj in stackedImages[type])
                Destroy(obj);
            stackedImages[type].Clear();
            portionsOnPlate[type] = 0;
        }
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private FoodPile GetPileByType(FoodType type)
    {
        foreach (var pile in foodPiles)
            if (pile.type == type) return pile;
        return null;
    }

    private float GetXOffsetForType(FoodType type)
    {
        switch (type)
        {
            case FoodType.Vegetais: return -60f;
            case FoodType.Arroz:    return   0f;
            case FoodType.Frango:   return  60f;
            default:                return   0f;
        }
    }

    private string GetFoodName(FoodType type)
    {
        switch (type)
        {
            case FoodType.Vegetais: return "vegetais";
            case FoodType.Arroz:    return "arroz";
            case FoodType.Frango:   return "frango";
            default:                return "comida";
        }
    }

    private IEnumerator PopScale(RectTransform rt)
    {
        float elapsed = 0f;
        float duration = 0.25f;
        Vector3 target = rt.localScale;
        rt.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float eased = 1f - Mathf.Pow(2f, -10f * t) * Mathf.Cos(t * Mathf.PI * 2.2f);
            rt.localScale = Vector3.LerpUnclamped(Vector3.zero, target, eased);
            yield return null;
        }
        rt.localScale = target;
    }
}

// -------------------------------------------------------
// Estruturas de dados
// -------------------------------------------------------
[System.Serializable]
public class FoodPile
{
    public PlateGameManager.FoodType type;
    public Sprite portionSprite;
}