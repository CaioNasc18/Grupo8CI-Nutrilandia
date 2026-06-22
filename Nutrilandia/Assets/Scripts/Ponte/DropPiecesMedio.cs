using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DropPiecesMedio : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        var collisionElement = eventData.pointerDrag.GetComponent<PuzzlePiece>();
        if (collisionElement == null) return;

        if (collisionElement.targetImage.name == this.gameObject.name)
        {
            var currentColor = this.GetComponent<Image>().color;
            currentColor.a = 1;
            GetComponent<Image>().color = currentColor;
            Destroy(collisionElement.gameObject, 0);
            PuzzleManagerMedio.IncrementRightAnswer();
        }
        else
        {
            collisionElement.ResetImage();
            PuzzleManagerMedio.IncrementWrongAnswer();
        }
    }
}