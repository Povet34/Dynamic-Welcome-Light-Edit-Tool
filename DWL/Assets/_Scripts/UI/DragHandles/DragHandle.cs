using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform rt;
    private CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup)
            canvasGroup.blocksRaycasts = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup)
            canvasGroup.blocksRaycasts = true;

        if (rt)
        {
            rt.anchoredPosition = GetRoundedPos();
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
    }

    protected Vector2Int GetRoundedPos()
    {
        var pos = rt.anchoredPosition;
        Vector2Int roundedPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        return roundedPos;
    }
}