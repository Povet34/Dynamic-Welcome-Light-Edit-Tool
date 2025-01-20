using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Neofect.BodyChecker.UI
{
    public class SpriteDropdownSupporter : MonoBehaviour, IDropdownSupporter
    {
        public Image target;

        public Sprite on;
        public Sprite off;

        public void SetToggle(bool isOn)
        {
            if (isOn)
                target.sprite = on;
            else
                target.sprite = off;
        }
    }
}