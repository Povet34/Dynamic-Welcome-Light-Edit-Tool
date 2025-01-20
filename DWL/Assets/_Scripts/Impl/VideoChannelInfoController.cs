using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VideoChannelInfoController : MonoBehaviour
{
    const int CELL_SIZE_Y = 80;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform content;

    private ScrollHolder scrollHolder = new ScrollHolder(ScrollHolder.Axis.Y);
    private List<VideoPixelChannelPositionCell> channelPositionCells = new List<VideoPixelChannelPositionCell>();

    [Header("Prefabs")]
    [SerializeField] private GameObject videoPixelChannelPositionCellPrefab;

    List<IPixelChannel> pixelChannels;
    Action<int, Vector2Int> changePosByControllerCallback;
    Action<int, Vector2Int> changePositionByHandleCallback;

    public void Init(ref List<IPixelChannel> pixelChannels, 
        Action<int, Vector2Int> changePosByControllerCallback,
        Action<int, Vector2Int> changePositionByHandleCallback)
    {
        this.pixelChannels = pixelChannels;
        this.changePosByControllerCallback = changePosByControllerCallback;
        this.changePositionByHandleCallback = changePositionByHandleCallback;

        CreateAllChannelPositionCells();
    }

    void UpdateContentHolder()
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, channelPositionCells.Count * CELL_SIZE_Y);
        scrollHolder.UpdateScrollHolderData(content, panel, 0, content.rect.height);
    }

    private void Update()
    {
        scrollHolder.UpdateHolder();
        UpdateVisibleItems();
    }

    private void UpdateVisibleItems()
    {
        float viewportHeight = panel.rect.height;
        float contentHeight = content.rect.height;

        // Get the normalized scroll position (0 to 1)
        float scrollPosition = 1 - scrollRect.verticalNormalizedPosition;

        // Calculate the top and bottom boundaries of the visible area based on scroll position
        float topVisibleY = (contentHeight - viewportHeight) * scrollPosition;
        float bottomVisibleY = topVisibleY + viewportHeight + CELL_SIZE_Y;

        // Loop through your items and check if they are within the visible area
        for (int i = 0; i < channelPositionCells.Count; i++)
        {
            var section = channelPositionCells[i];

            RectTransform itemRect = section.GetComponent<RectTransform>();

            float itemTopY = -itemRect.anchoredPosition.y;
            float itemBottomY = itemTopY + CELL_SIZE_Y;

            bool isVisible = (itemTopY < bottomVisibleY && itemBottomY > topVisibleY);
            section.ShowChildren(isVisible);
        }
    }

    public void ClearList()
    {
        DestroyAllCells();
    }

    public void RefreshChannlInfos()
    {
        if (IsValid())
        {
            var pointChannelCount = pixelChannels.FindAll(x => x.PixelChannelType == ePixelChannelType.Point).Count;

            int sub = channelPositionCells.Count - pointChannelCount;
            _UpdateCellCount();
            _UpdateDatas();

            void _UpdateCellCount()
            {
                if (sub > 0) //줄어들었음
                {
                    for(int i = 0; i < sub; i++)
                    {
                        var remove = channelPositionCells.Last();
                        channelPositionCells.Remove(remove);
                        Destroy(remove.gameObject);
                    }
                }
                else
                {
                    for (int i = 0; i < Mathf.Abs(sub); i++)
                    {
                        CreateChannel(-1, Vector2Int.zero);
                    }
                }
            }

            void _UpdateDatas()
            {
                for(int i = 0; i < channelPositionCells.Count; i++)
                {
                    var cell = channelPositionCells[i];
                    if(null != cell)
                    {
                        var pixel = pixelChannels[i];

                        cell.UpdateIndex(pixel.Index);
                        cell.UpdateByPixelChannelPos(pixel.GetFirstHandlePos());
                        
                        changePositionByHandleCallback?.Invoke(pixel.Index, pixel.GetFirstHandlePos());
                    }
                }

                UpdateContentHolder();
            }
        }
    }

    public void CreateAllChannelPositionCells()
    {
        DestroyAllCells();

        if (null != pixelChannels)
        {
            for (int i = 0; i < pixelChannels.Count; i++)
            {
                var ch = pixelChannels[i];
                if (ch.PixelChannelType == ePixelChannelType.Segment)
                    continue;

                CreateChannel(ch.Index, ch.GetFirstHandlePos());
            }
        }

        UpdateContentHolder();
    }

    public void CreateChannelCell(int idx, Vector2Int pos)
    {
        CreateChannel(idx, pos);
        channelPositionCells.OrderBy(cell => cell.ChannlIndex).ToList();

        UpdateContentHolder();
    }

    public void DestroyChannelCell(int idx)
    {
        if(null != channelPositionCells)
        {
            VideoPixelChannelPositionCell target = null;
            foreach(var cell in channelPositionCells)
            {
                if (idx == cell.ChannlIndex)
                {
                    target = cell;
                    break;
                }
            }

            channelPositionCells.Remove(target);
            Destroy(target);
        }    
    }

    private void CreateChannel(int idx, Vector2Int pos)
    {
        var go = Instantiate(videoPixelChannelPositionCellPrefab, content);
        var cell = go.GetComponent<VideoPixelChannelPositionCell>();
        if (cell)
        {
            cell.Init(idx, changePosByControllerCallback);
            cell.UpdateByPixelChannelPos(pos);

            channelPositionCells.Add(cell);
            changePositionByHandleCallback?.Invoke(idx, pos);
        }
    }

    private void DestroyAllCells()
    {
        if (null != channelPositionCells)
        {
            foreach (var cell in channelPositionCells)
                Destroy(cell.gameObject);

            channelPositionCells.Clear();
        }
    }

    public void ChangePositionByHandle(int chIdx, Vector2Int pos)
    {
        if (IsValid())
        {
            foreach(var cell in channelPositionCells)
            {
                if(cell.ChannlIndex == chIdx)
                {
                    cell.UpdateByPixelChannelPos(pos);
                    break;
                }
            }
        }
    }

    private bool IsValid()
    {
        return null != pixelChannels && null != channelPositionCells;
    }
}
