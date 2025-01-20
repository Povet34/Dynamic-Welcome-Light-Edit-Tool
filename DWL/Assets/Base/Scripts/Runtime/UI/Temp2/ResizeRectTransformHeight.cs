using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Neofect.UI
{
    public class ResizeRectTransformHeight : MonoBehaviour
    {
        public int margin = 0;
        public int maxHeight = 0;
        public GridLayoutGroup gridLayoutGroup;

        public RectTransform myTransform;

        public void Refresh(int count)
        {
            var height = gridLayoutGroup.cellSize.y * count + gridLayoutGroup.spacing.y * (count - 1) + margin;
            if (height > maxHeight)
                height = maxHeight;
            myTransform.sizeDelta = new Vector2(myTransform.sizeDelta.x, height);
        }
    }
}
