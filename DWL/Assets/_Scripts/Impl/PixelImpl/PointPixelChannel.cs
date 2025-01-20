using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI
{
    public class PointPixelChannel : UIBase, IPixelChannel
    {
        #region Enum Object : -------------------------------------------------

        private enum Texts
        {
            PixelNotifierText,
        }

        private enum Images
        {
            DragHandleImage,
            PixelNotifierImage,
        }

        #endregion

        #region Accessor : ----------------------------------------------------
        private UITextMeshPro pixelNotifierText => GetText(Convert.ToInt32(Texts.PixelNotifierText));

        private UIImage dragHandleImage => GetImage(Convert.ToInt32(Images.DragHandleImage));
        private UIImage pixelNotifierImage => GetImage(Convert.ToInt32(Images.PixelNotifierImage));


        #endregion

        private Texture2D _sourceTexture;
        public Texture2D SourceTexture => _sourceTexture;

        private PixelData pixelData = new PixelData();

        private int index;
        public int Index { get => index; set => index = value; }

        public ePixelChannelType PixelChannelType => ePixelChannelType.Point;

        private Vector2 initSize;

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
            this.index = index;
            dragHandleImage.color = color;
            pixelNotifierImage.color = color * 0.3f;

            GetComponentInChildren<DragHandle_IPixcelChannel>()?.Setup(this, selectedCallback, changePositionByHandle);

            initSize = dragHandleImage.rectTransform.rect.size;
            ChangeSize(scale);

            dragHandleImage.rectTransform.anchoredPosition = initPos;
            ShowNotifier(isNotifierShow);
        }

        public void SetTextrue(Texture2D texture, int recordTime, List<Vector2> positions)
        {
            _sourceTexture = texture;

            for (int i = 0; i < positions.Count; i++)
            {
                var pos = new Vector2Int((int)positions[i].x, (int)positions[i].y);
                Color pixelColor = GetPixelColor(SourceTexture, pos.x, pos.y);
                pixelData.UpdateData(index, pixelColor, pos, recordTime);
            }

            if (pixelNotifierText)
                pixelNotifierText.text = pixelData.GetIndex();
        }

        Color GetPixelColor(Texture2D texture, float x, float y)
        {
            return texture.GetPixel((int)x, (int)y);
        }

        public List<PixelData> GetPixelDatas()
        {
            return new List<PixelData>() { pixelData };
        }

        public void ShowTargetNotifier(bool isShow)
        {
            pixelNotifierImage?.gameObject.SetActive(isShow);
        }

        public List<GameObject> GetDragHandles()
        {
            if (dragHandleImage)
                return new List<GameObject>() { dragHandleImage.gameObject };

            return null;
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
            if (null != points && points.Count > 0)
            {
                dragHandleImage.GetComponent<RectTransform>().anchoredPosition = points[0];
            }
        }

        public void UpdateSelection(bool isSelected)
        {
            var spritePath = isSelected ? Definitions.CHANNEL_SELECTED_SPRITE_NAME : Definitions.CHANNEL_BASIC_SPRITE_NAME;

            if (string.IsNullOrEmpty(spritePath))
                return;

            var sprite = Resources.Load<Sprite>(spritePath);

            if (sprite && dragHandleImage)
            {
                dragHandleImage.sprite = sprite;
            }
        }

        public void UpdateChannelInfo(int index, Vector2Int position)
        {
            this.index = index;

            Color pixelColor = null != SourceTexture ? GetPixelColor(SourceTexture, position.x, position.y) : Color.black;
            pixelData.UpdateData(index, pixelColor, position, 0);

            if (pixelNotifierText && pixelNotifierImage.IsActive())
                pixelNotifierText.text = pixelData.GetIndex();
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent, false);
        }

        public void ChangeSize(int multi)
        {
            dragHandleImage.rectTransform.sizeDelta = Vector2.one * Definitions.GetChannelSize_ByRawImg(multi);
        }

        public Vector2Int GetFirstHandlePos()
        {
            if (dragHandleImage)
            {
                Vector2Int posInt = new Vector2Int(
                    (int)dragHandleImage.rectTransform.anchoredPosition.x, 
                    (int)dragHandleImage.rectTransform.anchoredPosition.y);

                return posInt;
            }

            return Vector2Int.zero;
        }
    }
}