using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class RequiredNotifier : MonoBehaviour
{
    [SerializeField] GameObject goNotifiy;
    [SerializeField] UIImage bgImage;
    [SerializeField] UITextMeshPro notiText;

    private RectTransform canvasTr;
    private RectTransform bgImageTr => bgImage.rectTransform;
    private RectTransform rt;

    private Vector2 minSize = new Vector2(100, 30);
    private Vector2 maxSize = new Vector2(200, 200);

    private int minFontSize = 12;
    private int maxFontSize = 15;

    private GameObject curPickObject;

    private void Awake()
    {
        Init();
    }

    void Update()
    {
        FindHasRequiredNotifyTarget();
        
        CheckShow();

        ClampToCanvas();
    }

    void Init()
    {
        canvasTr = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        rt = GetComponent<RectTransform>();
    }

    void ClampToCanvas()
    {
        transform.position = Input.mousePosition;

        Vector3[] canvasCorners = new Vector3[4];
        canvasTr.GetWorldCorners(canvasCorners);

        Vector3[] elementCorners = new Vector3[4];
        rt.GetWorldCorners(elementCorners);

        Vector3 position = rt.position;

        // Left side
        if (elementCorners[0].x < canvasCorners[0].x)
        {
            position.x += canvasCorners[0].x - elementCorners[0].x;
        }
        // Right side
        if (elementCorners[2].x > canvasCorners[2].x)
        {
            position.x -= elementCorners[2].x - canvasCorners[2].x;
        }
        // Bottom side
        if (elementCorners[3].y < canvasCorners[0].y) // Bottom corner
        {
            position.y += canvasCorners[0].y - elementCorners[3].y;
        }
        // Top side
        if (elementCorners[1].y > canvasCorners[1].y) // Top corner
        {
            position.y -= elementCorners[1].y - canvasCorners[1].y;
        }

        rt.position = position;

    }

    void FindHasRequiredNotifyTarget()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject == curPickObject)
                    return;
            }

            foreach (var hit in results)
            {
                var notify = Provider.Instance.GetText(hit.gameObject.name);
                if (!string.IsNullOrEmpty(notify))
                {
                    curPickObject = hit.gameObject;

                    GetNotifyText(notify);
                    ResizingTextBounds();

                    return;
                }
            }
        }

        curPickObject = null;
        GetNotifyText(null);
        ResizingTextBounds();
    }

    void GetNotifyText(string notify) 
    {
        notiText.text = notify;
    }

    void ResizingTextBounds()
    {
        notiText.fontSize = Mathf.Max(notiText.fontSize, minFontSize);

        Vector2 preferredValues = notiText.GetPreferredValues(notiText.text, maxSize.x, maxSize.y);

        float newWidth = Mathf.Clamp(preferredValues.x, minSize.x, maxSize.x);
        float newHeight = Mathf.Clamp(preferredValues.y, minSize.y, maxSize.y);

        rt.sizeDelta = new Vector2(newWidth, newHeight);
    }

    void CheckShow()
    {
        goNotifiy.SetActive(curPickObject != null);
    }
}
