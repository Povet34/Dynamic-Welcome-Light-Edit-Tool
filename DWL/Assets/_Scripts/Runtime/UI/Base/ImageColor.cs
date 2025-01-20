using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageColor : ObjectColor
{
    private UIImage image;

    public override void ChangeAppColor(Color newColor) 
    {
        if (null == image)
        {
            image = GetComponent<UIImage>();
        }

        if (null != image) 
        {
            image.color = newColor;
        }
    }

}