using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : BaseScene
{
    protected override void Init()
    {
        if (!App.Instance.DataTable.HasStringData(DataTableMngr.PLAYER_PREFAB_APP_COLOR_ID))
            App.Instance.DataTable.SetStringData(DataTableMngr.PLAYER_PREFAB_APP_COLOR_ID, "1");

        sceneType = Define.eScene.INIT;
        App.Instance.UI.Push(eUIType.Login);
    }

    public override void Clear()
    {
        App.Instance.UI.Clear();
    }
}