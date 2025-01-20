using System;
using System.Windows.Forms;
using UnityEngine;

namespace Common.UI
{
    public class CreateChannelContextMenu : UIBase
    {
        #region Enum Object : -------------------------------------------------

        private enum Buttons
        {
            CreatePointChannelButton,
            CreateSegmentChannelButton,
        }

        #endregion

        #region Accessor : ----------------------------------------------------

        private UIButton createPointChannelButton => GetButton(Convert.ToInt32(Buttons.CreatePointChannelButton));
        private UIButton createSegmentChannelButton => GetButton(Convert.ToInt32(Buttons.CreateSegmentChannelButton));


        #endregion

        Action<ePixelChannelType, Vector2> createCallback;
        RectTransform rt;
        RectResizeHelper helper;

        protected override void Awake()
        {
            base.Awake();
            Bind<UIButton>(typeof(Buttons));

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
            createPointChannelButton.BindButtonEvent(string.Empty, OnCreatePointChannelButton);
            createSegmentChannelButton.BindButtonEvent(string.Empty, OnCreateSegmentChannelButton);
        }
        #endregion

        public void OnCreatePointChannelButton(string buttonData)
        {
            createCallback?.Invoke(ePixelChannelType.Point, helper.GetViewportCenterInPanel(rt.anchoredPosition));
            Show(false);
        }

        public void OnCreateSegmentChannelButton(string buttonData)
        {
            createCallback?.Invoke(ePixelChannelType.Segment, helper.GetViewportCenterInPanel(rt.anchoredPosition));
            Show(false);
        }

        #endregion

        public void Init(Action<ePixelChannelType, Vector2> createCallback, RectResizeHelper helper)
        {
            this.createCallback = createCallback;
            this.helper = helper;
            rt = GetComponent<RectTransform>();
        }

        public void ReplaceAndShow(Vector2 pos)
        {
            if (rt)
            {
                rt.anchoredPosition = pos;
                Show(true);
            }
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }
    }
}