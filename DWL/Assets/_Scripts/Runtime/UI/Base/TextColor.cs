using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColor : ObjectColor
{
    private UITextMeshPro text;

    public override void ChangeAppColor(Color newColor) 
    {
        if (null == text)
        {
            text = GetComponent<UITextMeshPro>();
        }

        if (null != text) 
        {
            text.color = newColor;
        }
    }
}