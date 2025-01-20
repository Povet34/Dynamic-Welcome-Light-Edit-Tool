using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollHolder
{
    public enum Axis
    {
        X, Y
    }

    private Axis axis;
    private RectTransform content;
    private RectTransform panel;
    private (float min, float max) scrollBounds;
    private float fixedValue;
    private float panelOffset;

    private Vector2 minLocation;
    private Vector2 maxLocation;
    private Vector2 initLocation;

    public ScrollHolder(Axis axis)
    {
        this.axis = axis;
    }

    public void UpdateHolder()
    {
        if (null == content || null == panel)
            return;

        bool isLarger = axis == Axis.X ? content.rect.width >= panel.rect.width - panelOffset : content.rect.height >= panel.rect.height - panelOffset;

        if (isLarger)
        {
            if(axis == Axis.X)
            {
                if (content.anchoredPosition.x < scrollBounds.max)
                    content.anchoredPosition = maxLocation;

                if (content.anchoredPosition.x > scrollBounds.min)
                    content.anchoredPosition = minLocation;
            }
            else
            {
                if (content.anchoredPosition.y > -scrollBounds.max)
                    content.anchoredPosition = maxLocation;

                if (content.anchoredPosition.y < scrollBounds.min)
                    content.anchoredPosition = minLocation;
            }
        }
        else
        {
            content.anchoredPosition = initLocation;
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            if(axis.Equals(Axis.Y))
                Debug.Log($"scrollBounds : ({scrollBounds.min}, {scrollBounds.max}), ");
        }
    }

    public void UpdateScrollHolderData(RectTransform content, RectTransform panel, float min, float max, float panelOffset = 0f)
    {
        fixedValue = axis == Axis.X ? content.anchoredPosition.y : content.anchoredPosition.x;

        this.panel = panel;
        this.content = content;

        var maxAxis = axis == Axis.X ? -(max - panel.rect.width + 100) : -(max - panel.rect.height + 150);

        scrollBounds.max = maxAxis;
        scrollBounds.min = min;

        minLocation = ConvertAxis(new Vector2(scrollBounds.min, fixedValue));
        maxLocation = ConvertAxis(new Vector2(scrollBounds.max, fixedValue));
        initLocation = ConvertAxis(new Vector2(0, fixedValue));

        this.panelOffset = panelOffset;
    }

    private Vector2 ConvertAxis(Vector2 value)
    {
        return axis switch
        {
            Axis.X => new Vector2(value.x, value.y),
            Axis.Y => new Vector2(value.y, -value.x),
            _ => Vector2.zero
        };
    }
}
