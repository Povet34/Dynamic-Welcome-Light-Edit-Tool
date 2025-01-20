using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        sceneType = Define.eScene.LOBBY;
        App.Instance.UI.Push(eUIType.NavigationBar);
    }

    public override void Clear()
    {
        
    }
}