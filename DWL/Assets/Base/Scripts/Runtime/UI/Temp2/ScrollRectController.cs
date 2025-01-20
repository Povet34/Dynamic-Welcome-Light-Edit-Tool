using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public class ScrollRectController : MonoBehaviour
    {
        public int cellHeight;
        public int cellMaxCount;

        public int topMarginHeight;
        public int bottomMarginHeight;

        public ScrollRect scrollRect;
        public RectTransform content;

        private RectTransform scrollRectTransform;

        private void Awake()
        {
            scrollRectTransform = GetComponent<RectTransform>();
        }

        public void SetCellMaxCount(int cellMaxCount)
        {
            this.cellMaxCount = cellMaxCount;
        }

        public void SetTopMarginHeight(int topMarginHeight)
        {
            this.topMarginHeight = topMarginHeight;
        }

        public void SetBottomMarginHeight(int bottomMarginHeight)
        {
            this.bottomMarginHeight = bottomMarginHeight;
        }

        public void SetNormalizedPosition(int targetIndex)
        {
            var differentRange = content.sizeDelta.y - scrollRectTransform.sizeDelta.y;
            if (differentRange <= 0)
                return;

            float topNormalizedHeight = topMarginHeight / differentRange;
            float cellNormalizedHeight = cellHeight / differentRange;
            float bottomNormalizedHeight = bottomMarginHeight / differentRange;

            float topNormalizedPosition = 1 - (topNormalizedHeight + targetIndex * cellNormalizedHeight);
            if (scrollRect.verticalNormalizedPosition < topNormalizedPosition)
                scrollRect.verticalNormalizedPosition = topNormalizedPosition;

            float bottomNormalizedPosition = bottomNormalizedHeight + (cellMaxCount - 1 - targetIndex) * cellNormalizedHeight;
            if (scrollRect.verticalNormalizedPosition > bottomNormalizedPosition)
                scrollRect.verticalNormalizedPosition = bottomNormalizedPosition;
        }
    }
