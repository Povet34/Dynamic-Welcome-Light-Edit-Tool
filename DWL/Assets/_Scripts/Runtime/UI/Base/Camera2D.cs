using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 2D ī�޶� ���� UI�� �׷��� ȭ���� ũ�⸦ ������
/// </summary>
[RequireComponent(typeof(Camera))]
public class Camera2D : MonoBehaviour, IObserver
{
    /// <summary>
    /// ���� ũ�� �ּҰ�, �ش� �ּҰ��� �����
    /// </summary>
    [HideInInspector]
    public int minWidth;
    /// <summary>
    /// ���� ũ�� �ּ� ��, �ش� �ּ� ���� �����
    /// </summary>
    [HideInInspector]
    public int minHeight;
    /// <summary>
    /// minWidth, minHeight�� �������� ������ ũ�⸦ ���η� ����, ���η� ���� ���ϴ� ���� 
    /// </summary>
    [HideInInspector]
    public bool matchWidth;

    /// <summary>
    /// ����̽��� �ػ󵵿� Camera2D�� ������ minWidth,minHeight�� ����Ͽ� ���� �������� ī�޶��� ũ�⸦ ����
    /// </summary>
    [HideInInspector]
    public int screenWidth;
    [HideInInspector]
    public int screenHeight;

    /// <summary>
    /// CanvasScaleByCamera2D �����ڿ��� ��Ƽ�� �ϱ� ����
    /// </summary>
    Subject _subject;
    public Subject subject
    {
        get
        {
            if (_subject == null)
            {
                _subject = new Subject();
            }

            return _subject;
        }
    }

    public int Priority { get; set; }
    public int ID { get; set; }

    private Camera _cam;

    public void OnEnable()
    {
        if (App.Instance == null)
        {
            return;
        }

        /// Camera�� Ȱ��ȭ �� ������ ScreenManager���� �ܸ����� �ػ󵵸� �����ͼ� ȭ�� ������ ��
        ScreenMngr screenManager = App.Instance.Screen;
        OnScreenSize(screenManager.ScreenWidth, screenManager.ScreenHeight);

        screenManager.subject.AddObserver(this);
    }

    public void OnDisable()
    {
        if (App.Instance == null)
        {
            return;
        }

        /// Camera�� ��Ȱ��ȭ �� ������ ScreenManager�� ��ϵ� �����ڸ� ��
        ScreenMngr screenManager = App.Instance.Screen;
        screenManager.subject.RemoveObserver(this);
    }

    public void OnResponse(object obj)
    {
        /// ScreenManager���� �ܸ����� �ػ󵵰� �����ٴ� ���� �˷��ָ� ī�޶� ����� ������
        ScreenMngr screenManager = obj as ScreenMngr;
        if (screenManager != null)
        {
            OnScreenSize(screenManager.ScreenWidth, screenManager.ScreenHeight);
        }
    }

    /// <summary>
    /// �ܸ����� �ػ󵵿� ���� ī�޶��� ũ�⸦ ������
    /// </summary>
    /// <param name="screenWidth">�ܸ����� ���� ũ��</param>
    /// <param name="screenHeight">�ܸ����� ���� ũ��</param>
    public void OnScreenSize(int screenWidth, int screenHeight)
    {
        if (_cam == null)
        {
            _cam = GetComponent<Camera>();
        }

        if (_cam.orthographic == false)
        {
            _cam.orthographic = true;
        }

        int orthographicSize = GetOrthographicSize(screenWidth, screenHeight);
        _cam.orthographicSize = orthographicSize;

        orthographicSize *= 2;

        this.screenWidth = (screenWidth * orthographicSize) / screenHeight;
        this.screenHeight = orthographicSize;

        subject.OnNotify();
    }

    /// <summary>
    /// 2D ī�޶��� ũ�⸦ ����Ͽ� ��ȯ
    /// </summary>
    /// <param name="screenWidth">�ܸ����� ���� ũ��</param>
    /// <param name="screenHeight">�ܸ����� ���� ũ��</param>
    /// <returns></returns>
    public int GetOrthographicSize(int screenWidth, int screenHeight)
    {
        int orthographicSize = 0;
        float addRate = 0.0f;

        /// ���� ũ�� ���� 
        if (matchWidth)
        {
            orthographicSize = Mathf.RoundToInt((screenHeight * minWidth) / screenWidth);
            if (orthographicSize < minHeight)
            {
                addRate = (float)minHeight / orthographicSize;
                orthographicSize = Mathf.RoundToInt(orthographicSize * addRate);
            }
        }
        else
        {
            orthographicSize = minHeight;
            int width = (screenWidth * minHeight) / screenHeight;
            if (width < minWidth)
            {
                addRate = (float)minWidth / width;
                orthographicSize = Mathf.RoundToInt(orthographicSize * addRate);
            }
        }

        return Mathf.RoundToInt(orthographicSize * 0.5f);
    }
}