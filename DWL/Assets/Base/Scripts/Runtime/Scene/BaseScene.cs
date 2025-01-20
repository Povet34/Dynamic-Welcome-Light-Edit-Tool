using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.eScene sceneType { get; protected set; } = Define.eScene.NONE;

    private void Awake()
    {
        Init();
    }

    protected abstract void Init();

    public abstract void Clear();
}