using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image targetImage;
    private Vector2 _startPosition;
    private RectTransform _rectTransform;
    private Canvas _myCanvas;
    private CanvasGroup _canvasGroup;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPosition = _rectTransform.transform.position;
        _myCanvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _myCanvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
    }
    public void ResetImage()
    {
        gameObject.GetComponent<RectTransform>().position = _startPosition;
    }
}