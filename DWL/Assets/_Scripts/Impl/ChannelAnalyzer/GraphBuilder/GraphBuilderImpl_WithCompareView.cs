using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChannelAnalyzers
{

    public class GraphBuilderImpl_WithCompareView : GraphBuilderImpl_OnlyView
    {
        [Header("CompareView")]
        [SerializeField] private RectTransform compareGraphRenderArea;

        List<IGraphPoint> comparePoints = new List<IGraphPoint>();
        private List<GameObject> compareLines = new List<GameObject>();

        private float GetCompareGraphHandlePosX(int order) => compareSectionInofs[order].time * _gridInfo.spacingX + X_START_OFFSET_SIZE;

        private List<ASectionInfo> compareSectionInofs;

        public void BuildComparewGraph(List<ASectionInfo> sectionInfos)
        {
            compareSectionInofs = sectionInfos;

            RemoveAllCompareGraphObjects();

            CreateComparePoints(sectionInfos);
            CreateCompareLines();
        }

        private void CreateComparePoints(List<ASectionInfo> infos)
        {
            float graphHeight = renderAreaSize.y;

            for (int i = 0; i < infos.Count; i++)
            {
                float xPosition = GetCompareGraphHandlePosX(i);
                float levelRetio = Mathf.InverseLerp(0, 255, infos[i].level) * graphHeight;
                float yPosition = ClosestFinder.FindClosestPoint((int)levelRetio, dutiesByPanelRatios);

                var newPoint = Instantiate(pointPrefab, graphRenderArea).GetComponent<RectTransform>();
                var point = newPoint.GetComponent<IGraphPoint>();

                var img = newPoint.GetComponent<UIImage>();
                if(img)
                    img.color = Definitions.COMPARE_GRAPH_LINE_COLOR;

                GraphHandleData data = new GraphHandleData();
                data.channelIdex = _gridInfo.channelIndex;
                data.initPos = new Vector2Int((int)xPosition, (int)yPosition);
                data.time = infos[i].time;
                data.order = i;

                newPoint.name = $"{data.order}({data.time},{yPosition})";

                point.InitHandle(data);
                comparePoints.Add(point);
            }
        }

        private void CreateCompareLines()
        {
            for (int i = 0; i < comparePoints.Count - 1; i++)
            {
                var newLine = Instantiate(linePrefab, graphRenderArea);
                var lineRt = newLine.GetComponent<RectTransform>();
                newLine.GetComponent<UIImage>().color = Definitions.COMPARE_GRAPH_LINE_COLOR;

                lineRt.anchoredPosition = comparePoints[i].point;

                var firstPoint = comparePoints[i].point;
                var secondPoint = comparePoints[i + 1].point;

                float dist = Vector2.Distance(firstPoint, secondPoint);
                lineRt.sizeDelta = new Vector2(dist, lineRt.sizeDelta.y);
                lineRt.pivot = new Vector2(0, 0.5f);

                float angle = Mathf.Atan2(secondPoint.y - firstPoint.y, secondPoint.x - firstPoint.x) * Mathf.Rad2Deg;
                lineRt.localEulerAngles = new Vector3(0, 0, angle);

                newLine.transform.SetSiblingIndex(0);
                compareLines.Add(newLine);
            }
        }

        private void RemoveAllCompareGraphObjects()
        {
            if (null != comparePoints)
            {
                foreach (var handle in comparePoints)
                    handle.Destroy();
                comparePoints.Clear();
            }

            if (null != compareLines)
            {
                foreach (var line in compareLines)
                    Destroy(line);
                compareLines.Clear();
            }
        }
    }
}