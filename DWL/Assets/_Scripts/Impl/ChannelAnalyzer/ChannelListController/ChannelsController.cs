using ChannelAnalyzers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelsController : MonoBehaviour
{
    const int CELL_SIZE_Y = 50;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform content;

    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;

    private AChannelsControllerInfo info;
    private List<OnlyChannelCell> onlyChannelCells = new List<OnlyChannelCell>();
    private ScrollHolder scrollHolder = new ScrollHolder(ScrollHolder.Axis.Y);

    public void Init(AChannelsControllerInfo info)
    {
        if (null != info)
        {
            this.info = info;

            DestroyAllChannelCells();
            CreateChannelCells(info.channelInfos);
        }
    }

    public void DestroyAllChannelCells()
    {
        if (null != onlyChannelCells)
        {
            foreach (var cell in onlyChannelCells)
                Destroy(cell.gameObject);

            onlyChannelCells.Clear();
        }
    }

    private void CreateChannelCells(List<AChannelInfo> infos)
    {
        if (null == infos && infos.Count == 0)
            return;

        for (int i = 0; i < infos.Count; i++)
        {
            CreateChannelCell(infos[i]);
        }
        UpdateContentHolder();
    }


    void CreateChannelCell(AChannelInfo info)
    {
        if (cellPrefab)
        {
            var go = Instantiate(cellPrefab, content);
            var channelCell = go.GetComponent<OnlyChannelCell>();
            channelCell.Init(info, this.info.showGraphCallback);

            onlyChannelCells.Add(channelCell);
        }
    }

    void UpdateContentHolder()
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x, onlyChannelCells.Count * CELL_SIZE_Y);
        scrollHolder.UpdateScrollHolderData(content, panel, 0, content.rect.height);
    }

    private void Update()
    {
        scrollHolder.UpdateHolder();
    }
}
