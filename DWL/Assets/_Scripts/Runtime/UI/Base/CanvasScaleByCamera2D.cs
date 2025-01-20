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
        /// Canvas가 활성화 될 때 Canvas의 사이즈 설정 및 2D카메라에 관찰자로 등록
        OnScreenSize();
        Camera.subject.AddObserver(this);
    }

    public void OnDisable()
    {
        /// Canvas가 비활성화 될 때 2D카메라에 관찰자에서 제거
        Camera.subject.RemoveObserver(this);
    }

    public void OnResponse(object obj)
    {
        /// 2D 카메라 사이즈 변경 시 Canvas의 사이즈 변경 
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