using NPOI.SS.Formula.Functions;
using System.Windows.Forms;
using UnityEngine;

public class RectResizeHelper
{
    private RectTransform viewport;
    private RectTransform childPanel;

    public RectResizeHelper(RectTransform viewport, RectTransform childPanel)
    {
        this.viewport = viewport;
        this.childPanel = childPanel;
    }

    public Vector2 ViewportToChildPanelPosition(Vector2 viewportPosition)
    {
        // Convert the viewport point to local coordinates of the viewport
        Vector2 localViewportPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, viewportPosition, null, out localViewportPosition);

        // Calculate the scale difference between the viewport and the child panel
        float scaleX = viewport.rect.width / childPanel.rect.width;
        float scaleY = viewport.rect.height / childPanel.rect.height;

        // Convert local viewport position to child panel position taking scale and offset into account
        Vector2 childPanelPosition = new Vector2(
            localViewportPosition.x / scaleX + childPanel.anchoredPosition.x,
            localViewportPosition.y / scaleY + childPanel.anchoredPosition.y
        );

        return childPanelPosition;
    }

    public Vector3 GetViewportCenterInPanel(Vector3 pos)
    {
        Vector3 panelLocalCenter = childPanel.InverseTransformPoint(pos + new Vector3(viewport.rect.width / 2 + 155 + 155, viewport.rect.height / 2 + 13 + 95, 0));
        return panelLocalCenter;
    }
}
