using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataTableMngr;

public class ObjectColor : MonoBehaviour
{
    public eAppColor appColor;

    public virtual void ChangeAppColor(Color newColor) { }
}