using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;  // <- adiciona este using

public class DialogInputHandlerPonte : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private DialogManagerPonte dialogManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        dialogManager.OnScreenClick();
    }

    private void Update()
    {
        // Substituído para o novo Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame)
            {
                dialogManager.OnScreenClick();
            }
        }
    }
}