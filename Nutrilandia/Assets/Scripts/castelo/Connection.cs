using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour
{
    public RectTransform start;
    public RectTransform end;
    private RectTransform rt;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (start == null || end == null) return;

        Vector2 startPos = start.position;
        Vector2 endPos = end.position;
        Vector2 dir = endPos - startPos;

        rt.position = (startPos + endPos) / 2f;
        rt.sizeDelta = new Vector2(dir.magnitude, 5f);
        rt.rotation = Quaternion.Euler(0, 0,
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
}