using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChannelAnalyzers
{
    public class GraphPointImpl_UsingHandle : MonoBehaviour, IGraphPoint, 
        IBeginDragHandler, 
        IDragHandler, 
        IEndDragHandler
    {
        private Canvas canvas;
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;

        private int ownerIndex; //나를 소유중인 Channel의 인덱스

        private int upperThreaholdY;
        private int underThreaholdY;

        private Vector2Int _point;
        private int time;
        public Vector2Int point => _point;

        private int order;

        private Action onDragStart;
        private Action onDragEnd;
        private Func<int, (float start, float end)> onGetMovableRange;
        private Func<int, int> onGetTimeByPosX;
        private Action<ASectionInfo, bool> onHandleChanged;

        private List<int> dutiesByPanelRatios = new List<int>();

        private (float start, float end) movableRange;

        public void InitHandle(GraphHandleData initData)
        {
            if (null == initData)
                return;

            canvas = GetComponentInParent<Canvas>();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            UpdateHandlePosition(initData.initPos);

            time = initData.time;
            order = initData.order;

            ownerIndex = initData.channelIdex;

            upperThreaholdY = initData.upperThreaholdY;
            underThreaholdY = initData.underThreaholdY;

            onDragStart = initData.onDragStart;
            onDragEnd = initData.onDragEnd;

            dutiesByPanelRatios = initData.dutiesByPanelRatio;

            onGetMovableRange = initData.onGetMovableRange;
            onGetTimeByPosX = initData.onGetTimeByPosX;
            onHandleChanged = initData.onRefreshSectionInfo;

        }

        private void UpdateHandlePosition(Vector2 newPos)
        {
            _point = new Vector2Int(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y));
            rectTransform.anchoredPosition = new Vector2(_point.x, _point.y);
        }

        public (int time, int height) GetHandleInfo()
        {
            return (time, _point.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onDragStart.Invoke();

            canvasGroup.alpha = .6f;
            canvasGroup.blocksRaycasts = false;

            movableRange = onGetMovableRange(order);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 newPosition = rectTransform.anchoredPosition;
            newPosition.y = ClosestFinder.FindClosestPoint((int)newPosition.y, dutiesByPanelRatios);
            UpdateHandlePosition(newPosition);

            onDragEnd.Invoke();

            var info = new ASectionInfo();
            info.channelIndex = ownerIndex;
            info.order = order;
            info.time = onGetTimeByPosX?.Invoke(point.x) ?? -1;// Todo errorLog 
            info.level = Provider.Instance.GetDuty().GetPanelRatioLevel(_point.y, dutiesByPanelRatios);

            onHandleChanged?.Invoke(info, false);

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 newPosition = rectTransform.anchoredPosition;

            //x에 대한처리 order -1, order +1 사이의 time값이 Range다.
            newPosition.x += (eventData.delta.x / canvas.scaleFactor);
            newPosition.x = Mathf.Clamp(newPosition.x, movableRange.start, movableRange.end);

            // y에 대한 처리.. 최종적으로는 Duty에 붙는다.
            newPosition.y += (eventData.delta.y / canvas.scaleFactor);
            newPosition.y = Mathf.Clamp(newPosition.y, underThreaholdY, upperThreaholdY);

            UpdateHandlePosition(newPosition);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public float GetPosX()
        {
            return rectTransform.anchoredPosition.x;
        }

        public RectTransform GetOwnerRt()
        {
            return rectTransform;
        }

        public int GetOrder()
        {
            return order;
        }
    }
}
