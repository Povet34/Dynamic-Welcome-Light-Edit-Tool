using Common.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChannelAnalyzers
{
    public class GraphBuilderImpl_UsingHandle : MonoBehaviour, 
        IGraphBuilder,
        IPointerDownHandler,
        IPointerUpHandler
    {
        float Y_MAX = 255;
        int X_OFFSET_SIZE = 50;

        [SerializeField] private Vector2Int startPosOffset = new Vector2Int(100, 100);

        [SerializeField] private Canvas canvas;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform graphPanel;
        [SerializeField] private RectTransform graphRenderArea;
        [SerializeField] private RectTransform graphBgArea;

        [Header("Prefabs")]
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private GameObject comparePointPrefab;
        [SerializeField] private GameObject levelMarkTextPrefab;
        [SerializeField] private GameObject timeScaleTextPrefab;
        [SerializeField] private GameObject yAxesLinePrefab;
        [SerializeField] private GameObject removeButtonPrefab;

        private List<GameObject> lines = new List<GameObject>();
        private ScrollHolder scrollHolder = new ScrollHolder(ScrollHolder.Axis.X);

        private List<GameObject> bgObjects = new List<GameObject>();

        private List<IGraphPoint> _points = new List<IGraphPoint>();
        public List<IGraphPoint> points => _points;

        private GraphGridInfo _gridInfo;
        public GraphGridInfo graphGridInfo => _gridInfo;

        private List<int> dutiesByPanelRatios = new List<int>();
        private Vector2 renderAreaOffset;
        private Vector2 renderAreaSize;
        private List<ASectionInfo> sectionInfos;
        private List<GameObject> yAxisList = new List<GameObject>();

        //Compare Value
        private List<ASectionInfo> compareSectionInofs;
        private List<IGraphPoint> comparePoints = new List<IGraphPoint>();
        private List<GameObject> compareLines = new List<GameObject>();

        public RectTransform GetContentRt() => content;
        public float GetLastHandlePosX() => sectionInfos[sectionInfos.Count - 1].time* _gridInfo.spacingX + X_OFFSET_SIZE;

        private float GetCompareGraphHandlePosX(int order) => compareSectionInofs[order].time * _gridInfo.spacingX + X_OFFSET_SIZE;

        public float GetHandlePosX(int order) => sectionInfos[order].time * _gridInfo.spacingX + X_OFFSET_SIZE;

        private void Awake()    
        {
            graphBgArea.sizeDelta = graphPanel.rect.size - startPosOffset;

            renderAreaSize = graphBgArea.rect.size;
            renderAreaOffset = graphBgArea.anchoredPosition;
        }

        private void Update()
        {
            scrollHolder.UpdateHolder();
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }

        public void InitGraph(GraphGridInfo info)
        {
            RemoveAllObjects();

            sectionInfos = info.sectionInfos;
            dutiesByPanelRatios.Clear();

            var duties = info.savedDuties;
            var dutiesRatios = new List<int>();
            for(int i = 0; i<  duties.Count; i++)
                dutiesRatios.Add((int)(Mathf.InverseLerp(0, 255, duties[i]) * 100));

            foreach (var level in dutiesRatios)
                dutiesByPanelRatios.Add((int)Mathf.Lerp(renderAreaOffset.y, renderAreaSize.y - (startPosOffset.y * (1 - level * 0.01f)), level * 0.01f));

            _gridInfo = info;

            scrollHolder.UpdateScrollHolderData(content, graphPanel, 0, sectionInfos.Last().time * _gridInfo.spacingX, 100);
            CreateGraph(info.sectionInfos);
        }

        public void UpdateGraph(List<ASectionInfo> sectionInfos)
        {
            if (null == _gridInfo)
                return;
            
            this.sectionInfos = sectionInfos;
            scrollHolder.UpdateScrollHolderData(content, graphPanel, 0, sectionInfos.Last().time * _gridInfo.spacingX, 100);

            CreateGraph(sectionInfos);
        }

        private void CreateGraph(List<ASectionInfo> sectionInfos)
        {
            DestroyAllBgObjects();
            RemoveAllObjects();

            CreatePoints(sectionInfos);
            DrawAxes();

            DrawHandleBetweenLine();
            DrawGridLinesAndLabels();
        }

        private void DestroyAllLines()
        {
            foreach (var line in lines)
                Destroy(line);

            lines.Clear();
        }

        private void CreatePoints(List<ASectionInfo> infos)
        {
            float graphHeight = graphBgArea.rect.height;

            for (int i = 0; i < infos.Count; i++)
            {
                float xPosition = GetHandlePosX(i);
                float yPosition = ClosestFinder.FindClosestPoint((int)(infos[i].level / Y_MAX * graphHeight), dutiesByPanelRatios);
                var newPoint = Instantiate(pointPrefab, graphRenderArea).GetComponent<RectTransform>();
                var point = newPoint.GetComponent<IGraphPoint>();

                GraphHandleData data = new GraphHandleData();   
                data.channelIdex = _gridInfo.channelIndex;
                data.underThreaholdY = (int)renderAreaOffset.y;
                data.upperThreaholdY = (int)renderAreaSize.y;
                data.initPos = new Vector2Int((int)xPosition, (int)yPosition);
                data.time = infos[i].time;
                data.order = i;
                data.onDragStart = DestroyAllLines;
                data.onDragEnd = RefreshChanageHandlePos;
                data.onGetMovableRange = GetMoveableRange;
                data.onGetTimeByPosX = GetTimeByPosX;
                data.dutiesByPanelRatio = dutiesByPanelRatios;
                data.onRefreshSectionInfo = _gridInfo.onRefreshSectionInfo;

                point.InitHandle(data);
                points.Add(point);
            }
        }

        private void RefreshChanageHandlePos() 
        {
            //정보를 바꾼다.
            for (int i = 0; i < sectionInfos.Count; i++)
            {
                var info = sectionInfos[i];
                //if(info.channelIndex)
            }

            //겹치는지 등, 중첩될 수 없는 것을 확인한다.

            //바뀐정보를 토대로, 그래프를 리프레시한다.

            DrawHandleBetweenLine();
        }


        private void DestroyAllBgObjects()
        {
            if(null != bgObjects)
            {
                foreach(var obj in bgObjects)
                    Destroy(obj);

                bgObjects.Clear();
            }
        }

        private void DrawHandleBetweenLine()
        {
            //Line을 다시 그린다.
            for (int i = 0; i < points.Count - 1; i++)
            {
                var newLine = Instantiate(linePrefab, graphRenderArea);
                var lineRt = newLine.GetComponent<RectTransform>();
                lineRt.anchoredPosition = points[i].point;

                var firstPoint = points[i].point;
                var secondPoint = points[i + 1].point;

                float dist = Vector2.Distance(firstPoint, secondPoint);
                lineRt.sizeDelta = new Vector2(dist, lineRt.sizeDelta.y);
                lineRt.pivot = new Vector2(0, 0.5f);

                float angle = Mathf.Atan2(secondPoint.y - firstPoint.y, secondPoint.x - firstPoint.x) * Mathf.Rad2Deg;
                lineRt.localEulerAngles = new Vector3(0, 0, angle);

                newLine.transform.SetSiblingIndex(0);
                lines.Add(newLine);
            }
        }

        //각 time의 미세 라인을 그림
        private void DrawYAxesLines(RectTransform xAxisRt)
        {
            for(int i = 0; i < yAxisList.Count; i++)
                Destroy(yAxisList[i].gameObject);

            yAxisList.Clear();

            int index = 1;
            for (float i = X_OFFSET_SIZE; i <= GetLastHandlePosX(); i += _gridInfo.spacingX)
            {
                var go = Instantiate(timeScaleTextPrefab, xAxisRt);
                var tsText = go.GetComponent<Text>();
                if (tsText)
                {
                    tsText.text = index.ToString();
                    tsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(i, -25);

                    YAxesLine.Info info = new YAxesLine.Info();
                    info.posX = i;
                    info.time = index;
                    info.size = new Vector2(1, 300);
                    info.rayCastTargetXAxisPadding = _gridInfo.spacingX / 2;

                    var yAxesGo = Instantiate(yAxesLinePrefab, graphBgArea);
                    yAxesGo.GetComponent<YAxesLine>().Init(info);

                    yAxisList.Add(yAxesGo);
                }
                index++;
            }
        }

        // X축과 Y축을 그리는 코드
        private void DrawAxes()
        {
            // X축 생성
            GameObject xAxis = new GameObject("XAxis", typeof(Image));
            xAxis.transform.SetParent(graphBgArea, false);
            xAxis.GetComponent<Image>().color = Color.black; 
            RectTransform xAxisRt = xAxis.GetComponent<RectTransform>();
            xAxisRt.anchorMin = xAxisRt.anchorMax = xAxisRt.pivot = new Vector2(0, 0);
            xAxisRt.sizeDelta = new Vector2(GetLastHandlePosX(), 5);

            // Y축 생성
            GameObject yAxis = new GameObject("YAxis", typeof(Image));
            yAxis.transform.SetParent(graphBgArea, false);
            yAxis.GetComponent<Image>().color = Color.black; 
            RectTransform yAxisRt = yAxis.GetComponent<RectTransform>();
            yAxisRt.anchorMin = yAxisRt.anchorMax = yAxisRt.pivot = new Vector2(0, 0);
            yAxisRt.sizeDelta = new Vector2(10, graphBgArea.sizeDelta.y);

            DrawYAxesLines(xAxisRt);

            bgObjects.Add(xAxis);
            bgObjects.Add(yAxis);
        }

        // 눈금 및 회색 선을 그리는 코드
        private void DrawGridLinesAndLabels()
        {
            for (int i = 0; i< dutiesByPanelRatios.Count; i++)
            {
                var duties = _gridInfo.savedDuties;
                float yPosition =dutiesByPanelRatios[i] - 25;

                // 회색 선 생성
                GameObject gridLine = new GameObject("GridLine", typeof(Image));
                gridLine.transform.SetParent(graphBgArea, false); 
                gridLine.GetComponent<Image>().color = Color.grey; 
                RectTransform gridLineRt = gridLine.GetComponent<RectTransform>();
                gridLineRt.anchorMin = gridLineRt.anchorMax = gridLineRt.pivot = new Vector2(0, 0); 
                gridLineRt.sizeDelta = new Vector2(GetLastHandlePosX(), 2);
                gridLineRt.anchoredPosition = new Vector2(0, yPosition);

                // 눈금 생성
                GameObject tickMark = new GameObject("TickMark", typeof(Image));
                tickMark.transform.SetParent(graphBgArea, false);
                tickMark.GetComponent<Image>().color = Color.black;
                RectTransform tickMarkRt = tickMark.GetComponent<RectTransform>();
                tickMarkRt.anchorMin = tickMarkRt.anchorMax = tickMarkRt.pivot = new Vector2(0, 0);
                tickMarkRt.sizeDelta = new Vector2(10, 5); 
                tickMarkRt.anchoredPosition = new Vector2(-5, yPosition);

                var levelGo = Instantiate(levelMarkTextPrefab, graphBgArea);
                RectTransform levelTextRt = levelGo.GetComponent<RectTransform>();
                levelTextRt.anchorMin = levelTextRt.anchorMax = levelTextRt.pivot = new Vector2(0, 0);
                levelTextRt.anchoredPosition = new Vector2(-35, yPosition);
                Text levelText = levelGo.GetComponent<Text>();
                levelText.text = duties[i].ToString();

                bgObjects.Add(gridLine);
                bgObjects.Add(tickMark);
                bgObjects.Add(levelGo);
            }
        }

        private void RemoveAllObjects()
        {
            if(null != _points)
            {
                foreach (var handle in _points)
                    handle.Destroy();
                _points.Clear();
            }

            if(null != lines)
            {
                foreach (var line in lines)
                    Destroy(line);
                lines.Clear();
            }
        }

        private (float start, float end) GetMoveableRange(int order)
        {
            float startTime = X_OFFSET_SIZE;
            float endTime = (int)GetLastHandlePosX();

            var target = sectionInfos.Find(x => x.order == order);
            if (null != target)
            {
                if (order != 0)
                    startTime = GetHandlePosX(order - 1);
                if (order != sectionInfos.Count - 1)
                    endTime = GetHandlePosX(order + 1);
            }

            return (startTime, endTime);
        }

        private int GetTimeByPosX(int posX)
        {
            posX = posX - X_OFFSET_SIZE;
            int time = Mathf.RoundToInt(posX / _gridInfo.spacingX);
            return time;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                var hitObj = eventData.pointerCurrentRaycast.gameObject;
                if (hitObj)
                {
                    var yA = hitObj.GetComponent<YAxesLine>();
                    if (yA)
                    {
                        ASectionInfo newInfo = new ASectionInfo();
                        newInfo.channelIndex = _gridInfo.channelIndex;
                        newInfo.time = yA.Time;
                        newInfo.order = -1;

                        //이건 ui 패널상의 level값이기 때문에, 해당 인덱스를 알아내서, duty값으로 치환해줘야한다.
                        var uiHeightLevel = ClosestFinder.FindClosestPoint((int)hitObj.GetComponent<RectTransform>().InverseTransformPoint(eventData.position).y, dutiesByPanelRatios);
                        int levelIndex = dutiesByPanelRatios.IndexOf(uiHeightLevel);

                        newInfo.level = Provider.Instance.GetDuty().GetlevelByIndex(levelIndex);

                        _gridInfo.onRefreshSectionInfo?.Invoke(newInfo, false);
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                var hitObj = eventData.pointerCurrentRaycast.gameObject;
                if (hitObj)
                {
                    var gp = hitObj.GetComponent<IGraphPoint>();
                    if (null != gp)
                    {
                        var bt = gp.GetOwnerRt().GetComponentInChildren<UIButton>();
                        if (bt && bt.name == "RemoveButton") //어우 나중에 뺴든 해라.
                        {
                            bt.gameObject.SetActive(!bt.gameObject.activeSelf);
                        }
                        else if (removeButtonPrefab)
                        {
                            var go = Instantiate(removeButtonPrefab, gp.GetOwnerRt());
                            go.GetComponent<UIButton>().SetCallback(null,
                                (buttonData) =>
                                {
                                    ASectionInfo info = new ASectionInfo();
                                    info.order = gp.GetOrder();

                                    _gridInfo.onRefreshSectionInfo.Invoke(info, true);
                                    gp.Destroy();
                                });
                        }
                    }
                }
            }
        }

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

                var newPoint = Instantiate(comparePointPrefab, graphRenderArea).GetComponent<RectTransform>();
                var point = newPoint.GetComponent<IGraphPoint>();

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