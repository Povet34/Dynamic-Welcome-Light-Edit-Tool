using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine; 

namespace Common.UI
{
    public class CopyrightPopupUI : UIBase
    {
        #region Enum Object : -------------------------------------------------
        
        private enum Texts
        {
            CopyrightText1,
			CopyrightText2,
			CopyrightText3,
			CopyrightText4,
			CopyrightTitleText,
            ActivatedOnDateText,
        }

        private enum Buttons
        {
            ConfirmButton,
        }

        private enum GameObjects
        {
            GoBg,
			GoCopyrightPanel,
        }

        #endregion

        #region Accessor : ----------------------------------------------------
		private UITextMeshPro copyrightText1 => GetText(Convert.ToInt32(Texts.CopyrightText1));
		private UITextMeshPro copyrightText2 => GetText(Convert.ToInt32(Texts.CopyrightText2));
		private UITextMeshPro copyrightText3 => GetText(Convert.ToInt32(Texts.CopyrightText3));
		private UITextMeshPro copyrightText4 => GetText(Convert.ToInt32(Texts.CopyrightText4));
		private UITextMeshPro copyrightTitleText => GetText(Convert.ToInt32(Texts.CopyrightTitleText));
		private UITextMeshPro activatedOnDateText => GetText(Convert.ToInt32(Texts.ActivatedOnDateText));

		private UIButton confirmButton => GetButton(Convert.ToInt32(Buttons.ConfirmButton));

		private GameObject goBg => GetGameObject(Convert.ToInt32(GameObjects.GoBg));
		private GameObject goCopyrightPanel => GetGameObject(Convert.ToInt32(GameObjects.GoCopyrightPanel));


        #endregion

        const string ACTIVATE_DATE = "ActivateDate";

        protected override void Awake()
        {
            base.Awake();
            Bind<UITextMeshPro>(typeof(Texts));
			Bind<UIButton>(typeof(Buttons));
			Bind<GameObject>(typeof(GameObjects));
            RegisterEventHandler();
        }

        public override void OnInit() {}

        public override void OnActive() 
        {
            base.OnActive();
        }
        
        public override void OnInactive() {}
        public override void OnUpdateFrame() {}
        public override void OnUpdateSec() {}
        public override void OnClear() {}
        public override bool IsEscape() { return base.IsEscape(); }

        #region UI Event : ----------------------------------------------------

        #region Register Event Handler : ----------------------------
        private void RegisterEventHandler()
        {
            confirmButton.BindButtonEvent(string.Empty, OnConfirmButton);
        }
        #endregion

        public void OnConfirmButton(string buttonData)
        {
            gameObject.SetActive(false);
        }

        #endregion


        public void Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            ActivatedDate();
        }

        public void LoadActivatedDate()
        {
            if(!PlayerPrefs.HasKey(ACTIVATE_DATE))
                PlayerPrefs.SetString(ACTIVATE_DATE, DateTime.Today.ToString("yyyy-MM-dd"));
        }

        void ActivatedDate()
        {
            activatedOnDateText.text = $"Activated on {PlayerPrefs.GetString(ACTIVATE_DATE)}";
        }
    }
}