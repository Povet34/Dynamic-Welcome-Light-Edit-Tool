using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 드래그로 채널을 선택할 수 있게 하는 클래스
/// </summary>
public class AreaSelectionImpl_PixelChannel : MonoBehaviour, IAreaSelection
{
    private RectTransform selectionBox;
    private Vector2 startPos;
    private bool isSelecting;

    private List<IPixelChannel> pixelChannels;

    private Action<IPixelChannel> addCurChannelList;
    private Action clearCurChannelList;

    private void Update()
    {
        if (isSelecting)
            Select();
    }

    public void Select()
    {
        Vector2 currentMousePos = Input.mousePosition;
        Vector2 size = currentMousePos - startPos;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        selectionBox.anchoredPosition = startPos + size / 2f;

        clearCurChannelList?.Invoke();

        foreach (var channel in pixelChannels)
        {
            if (null == channel)
                continue;

            var handle = channel.GetDragHandles().FirstOrDefault();

            if (IsWithinSelection(handle))
                addCurChannelList?.Invoke(channel);
        }
    }

    public void PointerDown()
    {
        startPos = Input.mousePosition;
        selectionBox.gameObject.SetActive(true);
        isSelecting = true;
    }

    public void Cancel()
    {
        selectionBox.gameObject.SetActive(false);
        isSelecting = false;
    }

    bool IsWithinSelection(GameObject obj)
    {
        Bounds bounds = new Bounds(selectionBox.position, selectionBox.sizeDelta);
        return bounds.Contains(obj.transform.position);
    }

    public static AreaSelectionImpl_PixelChannel Create(Transform parent, List<IPixelChannel> channels, Action<IPixelChannel> addCurChannelList, Action clearCurChannelList)
    {
        var go = new GameObject("AreaSelection");
        var selection = go.AddComponent<AreaSelectionImpl_PixelChannel>();
        selection.transform.SetParent(parent);

        var imgGo = new GameObject("selectAreaBox");
        imgGo.transform.SetParent(selection.transform);

        var img = imgGo.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.3f);

        selection.selectionBox = imgGo.GetComponent<RectTransform>();
        selection.selectionBox.gameObject.SetActive(false);

        selection.pixelChannels = channels;

        selection.addCurChannelList = addCurChannelList;
        selection.clearCurChannelList = clearCurChannelList;

        return selection;
    }
}
