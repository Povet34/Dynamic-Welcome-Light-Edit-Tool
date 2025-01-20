using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasScaleByCamera2D : MonoBehaviour, IObserver
{
    public int Priority { get; set; }
    public int ID { get; set; }

    public Camera2D Camera;

    private Canvas canvas;
    private CanvasScaler canvasScaler;

    public void OnEnable()
    {
        /// Canvas�� Ȱ��ȭ �� �� Canvas�� ������ ���� �� 2Dī�޶� �����ڷ� ���
        OnScreenSize();
        Camera.subject.AddObserver(this);
    }

    public void OnDisable()
    {
        /// Canvas�� ��Ȱ��ȭ �� �� 2Dī�޶� �����ڿ��� ����
        Camera.subject.RemoveObserver(this);
    }

    public void OnResponse(object obj)
    {
        /// 2D ī�޶� ������ ���� �� Canvas�� ������ ���� 
        OnScreenSize();
    }

    private void OnScreenSize()
    {
        if (null == canvas)
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.GetComponent<Camera>();
        }

        if (null == canvasScaler)
        {
            canvasScaler = GetComponent<CanvasScaler>();
        }

        if (canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }

        canvasScaler.referenceResolution = new Vector2(Camera.screenWidth, Camera.screenHeight);
        canvasScaler.matchWidthOrHeight = Camera.matchWidth ? 0 : 1;
    }
}