using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; 

namespace Common.UI
{
    public class DutySetterPoupupUI : UIBase
    {
        #region Enum Object : -------------------------------------------------
        
        private enum Texts
        {
            CreateDutyAmountInputFieldText,
			CreateDutiesText,
			SaveDutiesText,
        }

        private enum Buttons
        {
            CancelButton,
			SaveDutiesButton,
        }

        private enum InputFields
        {
            CreateDutyAmountInputField,
        }

        private enum GameObjects
        {
            GoDutyInputPanel,
        }

        #endregion

        #region Accessor : ----------------------------------------------------
		private UITextMeshPro createDutiesText => GetText(Convert.ToInt32(Texts.CreateDutiesText));
		private UITextMeshPro saveDutiesText => GetText(Convert.ToInt32(Texts.SaveDutiesText));

		private UIButton cancelButton => GetButton(Convert.ToInt32(Buttons.CancelButton));
		private UIButton saveDutiesButton => GetButton(Convert.ToInt32(Buttons.SaveDutiesButton));

        private GameObject goDutyInputPanel => GetGameObject(Convert.ToInt32(GameObjects.GoDutyInputPanel));

        #endregion

        private List<DutyInputCell> dutyInputCells;

        protected override void Awake()
        {
            base.Awake();
            Bind<UITextMeshPro>(typeof(Texts));
			Bind<UIButton>(typeof(Buttons));
			Bind<UIInputField>(typeof(InputFields));
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
            cancelButton.BindButtonEvent(string.Empty, OnCancelButton);
			saveDutiesButton.BindButtonEvent(string.Empty, OnSaveDutiesButton);
        }
        #endregion

        public void OnCancelButton(string buttonData)
        {
            Hide();
        }

		public void OnSaveDutiesButton(string buttonData)
        {
            SaveDutiesButton();
        }

        #endregion

        public void InitDutySetter()
        {
            if(null == dutyInputCells || dutyInputCells.Count == 0) 
            {
                var cells = goDutyInputPanel.GetComponentsInChildren<DutyInputCell>();
                dutyInputCells = cells.ToList();
            }

            var duty = Provider.Instance.GetDuty();

            for(int i = 0; i < dutyInputCells.Count; i++) 
                dutyInputCells[i].Init(duty.GetlevelByIndex(i));
        }

        private void SaveDutiesButton()
        {
            List<int> levels = new List<int>();
            for(int i = 0; i < dutyInputCells.Count; i++) 
            {
                var level = dutyInputCells[i].GetLevel();
                if (level == -1)
                {
                    Provider.Instance.ShowErrorPopup("Exist not valid value");
                    return;
                }
                
                levels.Add(level);
            }

            Provider.Instance.GetDuty().SetLevels(levels);

            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();

            InitDutySetter();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}