using UnityEngine;

namespace ChannelAnalyzers
{
    public class GraphPointImpl_OnlyView : MonoBehaviour, IGraphPoint
    {
        private RectTransform rectTransform;

        private int time;
        private Vector2Int _point;
        public Vector2Int point => _point;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public (int time, int height) GetHandleInfo()
        {
            return (time, point.y);
        }

        public void InitHandle(GraphHandleData initData)
        {
            if (null == initData)
                return;

            _point = initData.initPos;
            time = initData.time;

            rectTransform.anchoredPosition = new Vector2(_point.x, _point.y);
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
            NDebug.Log("Not use");
            return -1;
        }
    }
}