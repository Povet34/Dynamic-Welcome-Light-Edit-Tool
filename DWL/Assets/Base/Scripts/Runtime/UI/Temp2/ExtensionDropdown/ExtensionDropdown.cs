using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Neofect.BodyChecker.UI
{
    public class ExtensionDropdown : Dropdown
    {
        private IDropdownSupporter[] supporters;

        protected IDropdownSupporter[] Supporters
        {
            get
            {
                if (supporters == null)
                    supporters = GetComponents<IDropdownSupporter>();
                return supporters;
            }
        }

        private List<Toggle> toggleList = new List<Toggle>();

        private List<int> disableIndexList = new List<int>();

        public void SetDisableIndexeList(List<int> disableIndexList)
        {
            this.disableIndexList = disableIndexList;
        }

        // 하위항목을 고른 후, Dropdown 버튼이 selected가 되지 않도록, 비워놓는다.
        public override void OnSelect(BaseEventData eventData)
        {
            if (Supporters != null)
            {
                foreach (var s in Supporters)
                    s.SetToggle(false);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (Supporters != null)
            {
                foreach (var s in Supporters)
                    s.SetToggle(true);
            }
        }

        protected override GameObject CreateDropdownList(GameObject template)
        {
            toggleList.Clear();
            return base.CreateDropdownList(template);
        }

        protected override DropdownItem CreateItem(DropdownItem itemTemplate)
        {
            var item = base.CreateItem(itemTemplate);
            toggleList.Add(item.toggle);
            return item;
        }

        protected override GameObject CreateBlocker(Canvas rootCanvas)
        {
            foreach(var index in disableIndexList)
            {
                if(index < toggleList.Count)
                {
                    toggleList[index].interactable = false;
                }
            }
            return base.CreateBlocker(rootCanvas);
        }
    }
}