using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChannelAnalyzers
{
    public interface IGraphBuilder
    {
        List<IGraphPoint> points { get; }
        GraphGridInfo graphGridInfo { get; }

        RectTransform GetContentRt();
        void InitGraph(GraphGridInfo info);
        void UpdateGraph(List<ASectionInfo> sectionInfos);
        float GetLastHandlePosX();
    }

    public interface IGraphPoint
    {
        public Vector2Int point { get; }

        void InitHandle(GraphHandleData initData);
        void Destroy();
        (int time, int height) GetHandleInfo();
        float GetPosX();
        RectTransform GetOwnerRt();
        int GetOrder();
    }

    public class GraphControllerInfo
    {
        public List<AChannelInfo> channelInfos;
        public List<int> savedDuties;

        public Action<List<AChannelInfo>> saveCallback;
        public Action<string> errorCallback;
    }

    public class GraphGridInfo
    {
        public int channelIndex;
        public float spacingX;
        public Vector2 renderAreaOffset;
        public Vector2 renderAreaSize;
        public List<int> savedDuties;
        public List<ASectionInfo> sectionInfos;

        public Action<ASectionInfo, bool> onRefreshSectionInfo;
        public Action<int, List<int>> startEditCallback;
    }

    public class GraphHandleData
    {
        public Action onDragStart;
        public Action onDragEnd;
        public Vector2Int initPos;
        public int channelIdex;
        public int time;
        public int order;

        public int upperThreaholdY;
        public int underThreaholdY;

        //각 duty들을 ui에 대응되게 재정의해놓은 값
        public List<int> dutiesByPanelRatio;

        public Func<int, (float start, float end)> onGetMovableRange;
        public Func<int, int> onGetTimeByPosX;
        public Action<ASectionInfo, bool> onRefreshSectionInfo;
    }
}