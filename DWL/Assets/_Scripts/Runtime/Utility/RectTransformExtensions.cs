using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtensions
{
    public enum RegionType
    {
        TopLeft = 0,
        BottomLeft = 1,
        TopRight = 2,
        BottomRight = 3,
        Center = 100,
    }

    public static bool IsFullyContainedIn(this RectTransform rectTransform, RectTransform container)
    {
        Vector3[] rectCorners = new Vector3[4];
        Vector3[] containerCorners = new Vector3[4];

        rectTransform.GetWorldCorners(rectCorners);
        container.GetWorldCorners(containerCorners);

        foreach (var corner in rectCorners)
        {
            if (!IsPointInside(corner, containerCorners))
                return false;
        }

        return true;
    }

    private static bool IsPointInside(Vector3 point, Vector3[] containerCorners)
    {
        bool insideHorizontal = point.x >= containerCorners[0].x && point.x <= containerCorners[2].x;
        bool insideVertical = point.y >= containerCorners[0].y && point.y <= containerCorners[1].y;

        return insideHorizontal && insideVertical;
    }

    public static Rect GetCenterPanel(RectTransform rt)
    {
        return new Rect(rt.localPosition, rt.rect.size);
    }

    public static Rect[] GetQuadrants(RectTransform rt)
    {
        Rect[] quadrants = new Rect[4];
        Vector2 size = new Vector2(rt.rect.width * 0.5f, rt.rect.height * 0.5f);
        Vector2 center = rt.localPosition;

        // 좌측상단
        quadrants[0] = new Rect(center.x - size.x, center.y, size.x, size.y);
        // 우측상단
        quadrants[1] = new Rect(center.x, center.y, size.x, size.y);
        // 좌측하단
        quadrants[2] = new Rect(center.x - size.x, center.y - size.y, size.x, size.y);
        // 우측하단
        quadrants[3] = new Rect(center.x, center.y - size.y, size.x, size.y);

        return quadrants;
    }

    public static Rect GetPanelArea(RectTransform panel, RegionType type)
    {
        Vector2 size = panel.rect.size;
        Vector2 halfSize = size * 0.5f;
        Vector2 center = (Vector2)panel.localPosition;

        switch (type)
        {
            case RegionType.Center:
                return new Rect(center - halfSize, size);
            case RegionType.TopLeft:
                return new Rect(center + new Vector2(-halfSize.x, 0), halfSize);
            case RegionType.TopRight:
                return new Rect(center, halfSize);
            case RegionType.BottomLeft:
                return new Rect(center - new Vector2(halfSize.x, halfSize.y), halfSize);
            case RegionType.BottomRight:
                return new Rect(center - new Vector2(0, halfSize.y), halfSize);
            default:
                return new Rect(center - halfSize, size);
        }
    }

    public static Rect GetSubPanelArea(RectTransform parentPanel, RegionType type)
    {
        Vector2 fullSize = parentPanel.rect.size;
        Vector2 quarterSize = fullSize * 0.5f;

        Vector2 offset = quarterSize * 0.5f;

        switch (type)
        {
            case RegionType.Center:
                return new Rect(Vector2.zero, fullSize);
            case RegionType.TopLeft:
                return new Rect(new Vector2(-offset.x, offset.y), quarterSize);
            case RegionType.TopRight:
                return new Rect(new Vector2(offset.x, offset.y), quarterSize);
            case RegionType.BottomLeft:
                return new Rect(new Vector2(-offset.x, -offset.y), quarterSize);
            case RegionType.BottomRight:
                return new Rect(new Vector2(offset.x, -offset.y), quarterSize);
            default:
                return new Rect(Vector2.zero, fullSize);
        }
    }

    public static Vector2 GetSubPanelCenter(RectTransform parentPanel, RegionType type)
    {
        Vector2 halfSize = parentPanel.rect.size * 0.5f;
        Vector2 quarterSize = halfSize * 0.5f;

        switch (type)
        {
            case RegionType.TopLeft:
                return new Vector2(-quarterSize.x, quarterSize.y) + (Vector2)parentPanel.localPosition - ((Vector2)parentPanel.localPosition * 0.5f);
            case RegionType.TopRight:
                return new Vector2(quarterSize.x, quarterSize.y) + (Vector2)parentPanel.localPosition + ((Vector2)parentPanel.localPosition * 0.5f);
            case RegionType.BottomLeft:
                return new Vector2(-quarterSize.x, -quarterSize.y) + (Vector2)parentPanel.localPosition - ((Vector2)parentPanel.localPosition * 0.5f);
            case RegionType.BottomRight:
                return new Vector2(quarterSize.x, -quarterSize.y) + (Vector2)parentPanel.localPosition + ((Vector2)parentPanel.localPosition * 0.5f);
            default:
                return (Vector2)parentPanel.localPosition; // 기본적으로 부모 패널의 중앙을 반환
        }
    }

    public static void SetSubPanel(this RectTransform subPanel, Rect area)
    {
        subPanel.anchorMin = new Vector2(0.5f, 0.5f);
        subPanel.anchorMax = new Vector2(0.5f, 0.5f);

        subPanel.anchoredPosition = new Vector2(area.x, area.y);
        subPanel.sizeDelta = new Vector2(area.width, area.height);
    }
}