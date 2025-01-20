using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ChannelAnalyzers
{
    public class GraphBuilderImpl_OnlyView : MonoBehaviour, IGraphBuilder
    {
        protected int X_START_OFFSET_SIZE = 1;
        protected int TIME_SCALE_TEXT_OFFSET_Y = -10;

        [SerializeField] protected Vector2Int startPosOffset = new Vector2Int(50, 50);
        [SerializeField] protected RectTransform content;
        [SerializeField] protected RectTransform graphPanel;
        [SerializeField] protected RectTransform graphRenderArea;
        [SerializeField] protected RectTransform graphBgArea;
        [SerializeField] protected UITextMeshPro currentSelectedChannelInfoText;
        [SerializeField] protected UIButton dataEditButton;

        [Header("Prefabs")]
        [SerializeField] protected GameObject linePrefab;
        [SerializeField] protected GameObject pointPrefab;
        [SerializeField] protected GameObject levelMarkTextPrefab;
        [SerializeField] protected GameObject timeScaleTextPrefab;

        List<IGraphPoint> _points = new List<IGraphPoint>();
        public List<IGraphPoint> points => _points;
        protected List<GameObject> bgObjects = new List<GameObject>();

        public float GetGraphBGWidth() => graphBgArea?.rect.width ?? 0f;
        public float GetLastHandlePosX() => _points.Last().GetHandleInfo().time * _gridInfo.spacingX;
        public float GetHandlePosX(int order) => sectionInfos[order].time * _gridInfo.spacingX + X_START_OFFSET_SIZE;
        public RectTransform GetContentRt() => content;

        protected GraphGridInfo _gridInfo;
        public GraphGridInfo graphGridInfo => _gridInfo;

        protected List<GameObject> lines = new List<GameObject>();
        protected ScrollHolder scrollHolder = new ScrollHolder(ScrollHolder.Axis.X);

        protected List<int> dutiesByPanelRatios = new List<int>();
        protected Vector2 renderAreaOffset;
        protected Vector2 renderAreaSize;
        protected List<ASectionInfo> sectionInfos;

        protected virtual void Awake()
        {
            graphBgArea.sizeDelta = graphPanel.rect.size - startPosOffset;

            renderAreaSize = graphBgArea.rect.size;
            renderAreaOffset = graphBgArea.anchoredPosition;

            dataEditButton.onClick.AddListener(StartEditData);
        }

        protected void Update()
        {
            scrollHolder.UpdateHolder();
        }

        public void InitGraph(GraphGridInfo info)
        {
            if (currentSelectedChannelInfoText)
                currentSelectedChannelInfoText.text = $"Ch {info.channelIndex.ToString("D2")}";

            sectionInfos = info.sectionInfos;
            _gridInfo = info;

            graphBgArea.sizeDelta = graphPanel.rect.size - startPosOffset;

            var duties = info.savedDuties;
            var dutiesRatios = new List<int>();
            for (int i = 0; i < duties.Count; i++)  
                dutiesRatios.Add((int)(Mathf.InverseLerp(0, 255, duties[i]) * 100));

            dutiesByPanelRatios.Clear();
            foreach (var level in dutiesRatios)
            {
                dutiesByPanelRatios.Add(
                    (int)Mathf.Lerp(renderAreaOffset.y,
                    renderAreaSize.y - (startPosOffset.y * (1 - level * 0.01f)),
                    level * 0.01f));
            }

            scrollHolder.UpdateScrollHolderData(content, graphPanel, 0, this.sectionInfos.Last().time * _gridInfo.spacingX, 100);

            CreateGraph(sectionInfos);
        }

        public void UpdateGraph(List<ASectionInfo> sectionInfos)
        {
            if (null == _gridInfo)
                return;
            this.sectionInfos = sectionInfos;

            scrollHolder.UpdateScrollHolderData(content, graphPanel, 0, sectionInfos.Last().time * _gridInfo.spacingX, 100);
            CreateGraph(sectionInfos);
        }

        protected void CreateGraph(List<ASectionInfo> sectionInfos)
        {
            DestroyAllBgObjects();
            RemoveAllHandleObjects();

            CreatePoints(sectionInfos);
            DrawAxes();
            CreateLines();
            DrawGridLines();
        }

        protected void CreatePoints(List<ASectionInfo> infos)
        {
            float graphHeight = renderAreaSize.y;

            for (int i = 0; i < infos.Count; i++)
            {
                float xPosition = GetHandlePosX(i);
                float levelRetio = Mathf.InverseLerp(0, 255, infos[i].level) * graphHeight;
                float yPosition = ClosestFinder.FindClosestPoint((int)levelRetio, dutiesByPanelRatios);

                var newPoint = Instantiate(pointPrefab, graphRenderArea).GetComponent<RectTransform>();
                var point = newPoint.GetComponent<IGraphPoint>();
                
                GraphHandleData data = new GraphHandleData();
                data.channelIdex = _gridInfo.channelIndex;
                data.initPos = new Vector2Int((int)xPosition, (int)yPosition);
                data.time = infos[i].time;
                data.order = i;

                newPoint.name = $"{data.order}({data.time},{yPosition})";

                point.InitHandle(data);
                points.Add(point);
            }
        }

        protected void CreateLines()
        {
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

        // X축과 Y축을 그리는 코드
        protected void DrawAxes()
        {
            // X축 생성
            GameObject xAxis = new GameObject("XAxis", typeof(Image));
            xAxis.transform.SetParent(graphBgArea, false);

            var xAxisImg = xAxis.GetComponent<Image>();
            xAxisImg.color = Color.black;
            xAxisImg.raycastTarget = false;

            RectTransform xAxisRt = xAxis.GetComponent<RectTransform>();
            xAxisRt.anchorMin = xAxisRt.anchorMax = xAxisRt.pivot = new Vector2(0, 0);
            xAxisRt.sizeDelta = new Vector2(GetLastHandlePosX(), 5);

            // Handle의 pos x(Frame)에 num 설정
            for (int i = 0; i < _points.Count; i ++)
            {
                var point = _points[i];
                var go = Instantiate(timeScaleTextPrefab, xAxisRt);
                var tsText = go.GetComponent<Text>();
                if (tsText)
                {
                    tsText.fontSize = 10;
                    tsText.text = point.GetHandleInfo().time.ToString();
                    tsText.GetComponent<RectTransform>().anchoredPosition = new Vector2(point.GetPosX(), TIME_SCALE_TEXT_OFFSET_Y);
                }
            }

            // Y축 생성
            GameObject yAxis = new GameObject("YAxis", typeof(Image));
            yAxis.transform.SetParent(graphBgArea, false);

            var yAxisImg = yAxis.GetComponent<Image>();
            yAxisImg.color = Color.black;
            yAxisImg.raycastTarget = false;

            RectTransform yAxisRt = yAxis.GetComponent<RectTransform>();
            yAxisRt.anchorMin = yAxisRt.anchorMax = yAxisRt.pivot = new Vector2(0, 0);
            yAxisRt.sizeDelta = new Vector2(10, graphBgArea.sizeDelta.y);

            bgObjects.Add(xAxis);
            bgObjects.Add(yAxis);
        }

        // 회색 선을 그리는 코드
        protected void DrawGridLines()
        {
            for (int i = 0; i < dutiesByPanelRatios.Count; i++)
            {
                var duty = Provider.Instance.GetDuty();
                float yPosition = dutiesByPanelRatios[i] - 25;

                // 회색 선 생성
                GameObject gridLine = new GameObject("GridLine", typeof(Image));
                gridLine.transform.SetParent(graphBgArea, false);
                gridLine.GetComponent<Image>().color = Color.grey;
                gridLine.GetComponent<Image>().raycastTarget = false;
                RectTransform gridLineRt = gridLine.GetComponent<RectTransform>();
                gridLineRt.anchorMin = gridLineRt.anchorMax = gridLineRt.pivot = new Vector2(0, 0);
                gridLineRt.sizeDelta = new Vector2(GetLastHandlePosX(), 2);
                gridLineRt.anchoredPosition = new Vector2(0, yPosition);

                bgObjects.Add(gridLine);
            }
        }

        public void RemoveAllHandleObjects()
        {
            if (null != points)
            {
                foreach (var handle in points)
                    handle.Destroy();
                points.Clear();
            }

            if (null != lines)
            {
                foreach (var line in lines)
                    Destroy(line);
                lines.Clear();
            }
        }

        protected void DestroyAllBgObjects()
        {
            if (null != bgObjects)
            {
                foreach (var obj in bgObjects)
                    Destroy(obj);

                bgObjects.Clear();
            }
        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        protected void StartEditData()
        {
            _gridInfo.startEditCallback?.Invoke(_gridInfo.channelIndex , _gridInfo.savedDuties);
        }
    }
}