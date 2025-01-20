using System;

namespace Common.UI
{
    public class DutyInputCell : UIBase
    {
        #region Enum Object : -------------------------------------------------

        private enum Texts
        {
            DutyIndexText,
            DutyInputFieldText,
        }

        private enum InputFields
        {
            DutyInputField,
        }

        #endregion

        #region Accessor : ----------------------------------------------------

        private UIInputField dutyInputField => GetInputField(Convert.ToInt32(InputFields.DutyInputField));

        #endregion

        protected override void Awake()
        {
            base.Awake();
            Bind<UITextMeshPro>(typeof(Texts));
            Bind<UIInputField>(typeof(InputFields));

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
            dutyInputField.BindInputFieldEvent(OnDutyInputFieldEndEdit, OnDutyInputFieldSubmit, OnDutyInputFieldValueChanged);
        }
        #endregion

        public void OnDutyInputFieldEndEdit(string text)
        {

        }

        public void OnDutyInputFieldSubmit(string text)
        {

        }

        public void OnDutyInputFieldValueChanged(string text)
        {

        }

        #endregion

        public void Init(int level)
        {
            dutyInputField.text = level.ToString();
        }

        public int GetLevel()
        {
            if (int.TryParse(dutyInputField.text, out int level))
            {
                return level;
            }

            return -1;
        }
    }
}