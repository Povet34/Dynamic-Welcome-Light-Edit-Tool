using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI
{
    public class SegmentPixelChannel : UIBase, IPixelChannel
    {
        #region Enum Object : -------------------------------------------------

        private enum Texts
        {
            PixelNotifierText,
        }

        private enum Images
        {
            DragHandleImageFirstImage,
            PixelNotifierImage,
            DragHandleImageSecondImage,
            TwoDotLineImage,
        }

        #endregion

        #region Accessor : ----------------------------------------------------
        private UITextMeshPro pixelNotifierText => GetText(Convert.ToInt32(Texts.PixelNotifierText));

        private UIImage dragHandleImageFirstImage => GetImage(Convert.ToInt32(Images.DragHandleImageFirstImage));
        private UIImage pixelNotifierImage => GetImage(Convert.ToInt32(Images.PixelNotifierImage));
        private UIImage dragHandleImageSecondImage => GetImage(Convert.ToInt32(Images.DragHandleImageSecondImage));
        private UIImage twoDotLineImage => GetImage(Convert.ToInt32(Images.TwoDotLineImage));

        #endregion

        private Texture2D _sourceTexture;
        public Texture2D SourceTexture => _sourceTexture;

        private List<PixelData> pixelDatas = new List<PixelData>();

        private int index;
        public int Index { get => index; set => index = value; }

        private Vector2 segmentStartPos
        {
            get
            {
                if (!_firstHandleRt)
                    _firstHandleRt = dragHandleImageFirstImage.GetComponent<RectTransform>();

                return _firstHandleRt.anchoredPosition;
            }
            set
            {
                if (!_firstHandleRt)
                    _firstHandleRt = dragHandleImageFirstImage.GetComponent<RectTransform>();

                _firstHandleRt.anchoredPosition = value;
            }
        }
        private Vector2 segmentEndPos
        {
            get
            {
                if (!_secondHandleRt)
                    _secondHandleRt = dragHandleImageSecondImage.GetComponent<RectTransform>();

                return _secondHandleRt.anchoredPosition;
            }
            set
            {
                if (!_secondHandleRt)
                    _secondHandleRt = dragHandleImageSecondImage.GetComponent<RectTransform>();

                _secondHandleRt.anchoredPosition = value;
            }
        }
        public ePixelChannelType PixelChannelType => ePixelChannelType.Segment;

        private RectTransform _firstHandleRt;
        private RectTransform _secondHandleRt;
        private RectTransform _segmentRt;

        protected override void Awake()
        {
            base.Awake();
            Bind<UITextMeshPro>(typeof(Texts));
            Bind<UIImage>(typeof(Images));

            RegisterEventHandler();
        }

        public override void OnInit() { }

        public override void OnActive()
        {
            base.OnActive();
        }

        public override void OnInactive() { }
        public override void OnUpdateFrame() { }
        public override void OnUpdateSec() { }
        public override void OnClear() { }
        public override bool IsEscape() { return base.IsEscape(); }

        #region UI Event : ----------------------------------------------------

        #region Register Event Handler : ----------------------------
        private void RegisterEventHandler()
        {

        }
        #endregion


        #endregion

        public void Init(int index, Color color, Vector2 initPos, int scale, bool isNotifierShow, Action<IPixelChannel> selectedCallback, Action<int, Vector2Int> changePositionByHandle)
        {
            dragHandleImageFirstImage.color = color;
            pixelNotifierImage.color = color * 0.3f;

            Index = index;

            _segmentRt = twoDotLineImage.GetComponent<RectTransform>();
            ChangeSize(scale);

            var handles = GetComponentsInChildren<DragHandle_IPixcelChannel>();
            if (null != handles)
            {
                foreach (var handle in handles)
                    handle?.Setup(this, selectedCallback, changePositionByHandle);
            }

            dragHandleImageFirstImage.rectTransform.anchoredPosition = initPos;
            dragHandleImageSecondImage.rectTransform.anchoredPosition = initPos + new Vector2(20, 0);

            ShowNotifier(isNotifierShow);
        }

        public void SetTextrue(Texture2D texture, int recordTime, List<Vector2> positions)
        {
            _sourceTexture = texture;
            var firstPos = positions[0];
            var secondPos = positions[1];

            var linePoints = BresenhamLineAlgorithm.GetLinePoints((int)firstPos.x, (int)firstPos.y, (int)secondPos.x, (int)secondPos.y, 3);

            pixelDatas.Clear();
            for (int i = 0; i < linePoints.Count; i++)
            {
                var point = linePoints[i];
                Color pixelColor = GetPixelColor(SourceTexture, point.x, point.y);
                PixelData data = new PixelData();
                data.UpdateData(index, pixelColor, point, recordTime);
                pixelDatas.Add(data);
            }
        }

        void Update()
        {
            if (segmentStartPos == Vector2.zero && segmentEndPos == Vector2.zero)
                return;

            DrawLine(segmentStartPos, segmentEndPos);
        }

        public List<PixelData> GetPixelDatas()
        {
            return new List<PixelData>(pixelDatas);
        }

        public List<GameObject> GetDragHandles()
        {
            return new List<GameObject>()
            {
                dragHandleImageFirstImage.gameObject,
                dragHandleImageSecondImage.gameObject,
            };
        }

        Color GetPixelColor(Texture2D texture, float x, float y)
        {
            return texture.GetPixel((int)x, (int)y);
        }

        void DrawLine(Vector2 start, Vector2 end)
        {
            if (!_segmentRt)
                return;

            Vector2 direction = end - start;
            _segmentRt.sizeDelta = new Vector2(direction.magnitude, _segmentRt.sizeDelta.y);
            _segmentRt.pivot = new Vector2(0, 0.5f);
            _segmentRt.anchoredPosition = start;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _segmentRt.rotation = Quaternion.Euler(0, 0, angle);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void ShowNotifier(bool isShow)
        {
            if (pixelNotifierImage)
                pixelNotifierImage.gameObject.SetActive(isShow);
        }

        public void SetHandlePos(List<Vector2> points)
        {
            if (null == points || points.Count < 2) return;

            segmentStartPos = points[0];
            segmentEndPos = points[1];
        }

        public void UpdateSelection(bool isSelected)
        {
            var spritePath = isSelected ? Definitions.CHANNEL_SELECTED_SPRITE_NAME : Definitions.CHANNEL_BASIC_SPRITE_NAME;

            if (string.IsNullOrEmpty(spritePath))
                return;

            var sprite = Resources.Load<Sprite>(spritePath);

            if (sprite)
            {
                if (dragHandleImageFirstImage)
                    dragHandleImageFirstImage.sprite = sprite;

                if (dragHandleImageSecondImage)
                    dragHandleImageSecondImage.sprite = sprite;
            }
        }


        /// <summary>
        /// 지금은 line 자체를 사용하지 않기때문에, Index를 보여주지는 않겠지만, 포함은 하기때문에 index를 넣어주긴 해야한다.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="positions"></param>
        public void UpdateChannelInfo(int index, Vector2Int position)
        {
            this.index = index;

            if (pixelNotifierText && pixelNotifierImage.IsActive())
                pixelNotifierText.text = index.ToString("D2");

            return;
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent, false);
        }

        public void ChangeSize(int multi)
        {
            dragHandleImageFirstImage.rectTransform.sizeDelta = Vector2.one * Definitions.GetChannelSize_ByRawImg(multi);
            dragHandleImageSecondImage.rectTransform.sizeDelta = Vector2.one * Definitions.GetChannelSize_ByRawImg(multi);
            twoDotLineImage.rectTransform.sizeDelta = new Vector2(twoDotLineImage.rectTransform.sizeDelta.x, Definitions.GetSegmentSize_ByRawImg(multi));
        }

        public Vector2Int GetFirstHandlePos()
        {
            if (dragHandleImageFirstImage)
            {
                Vector2Int posInt = new Vector2Int(
                    (int)dragHandleImageFirstImage.rectTransform.anchoredPosition.x,
                    (int)dragHandleImageFirstImage.rectTransform.anchoredPosition.y);

                return posInt;
            }

            return Vector2Int.zero;
        }
    }
}