using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using Neofect.Utility;
using UnityEngine.AddressableAssets;
using UnityEditor;

public abstract class UIBase : MonoBehaviour, IBase
{
    protected Dictionary<Type, UnityEngine.Object[]> objectDic = new Dictionary<Type, UnityEngine.Object[]>();
    protected ObjectColor[] objectColorArr;

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        objectDic.Add(typeof(T), objects);

        for (int index = 0; index < names.Length; index++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[index] = Utility.FindChild(gameObject, names[index], true);
            }
            else
            {
                objects[index] = Utility.FindChild<T>(gameObject, names[index], true);
            }

            if (null == objects[index])
                Debug.Log($"Failed to bind({names[index]})");
        }
    }

    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (objectDic.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[index] as T;
    }

    protected GameObject GetGameObject(int index) { return Get<GameObject>(index); }
    protected UITextMeshPro GetText(int index) { return Get<UITextMeshPro>(index); }
    protected UIRawImage GetRawImage(int index) { return Get<UIRawImage>(index); }
    protected UIImage GetImage(int index) { return Get<UIImage>(index); }
    protected UIButton GetButton(int index) { return Get<UIButton>(index); }
    protected UIToggle GetToggle(int index) { return Get<UIToggle>(index); }
    protected UIInputField GetInputField(int index) { return Get<UIInputField>(index); }
    protected UISlider GetSlider(int index) { return Get<UISlider>(index); }

    protected Switching GetSwitching(int index) { return Get<Switching>(index); }

    /// <summary>
    /// UI 데이터를 가지고 있는 ScriptableObject
    /// </summary>
    private UIAddressableData uiAddressableData;
    /// <summary>
    /// UI의 최상위에 있는 Canvas
    /// </summary>
    private Canvas canvas;

    public eUIType UIType { get { return uiAddressableData.uiType; } }
    public AssetReference AssetReference { get { return uiAddressableData.reference; } }
    public bool IsIgnoreEscape { get { return uiAddressableData.isIgnoreEscape; } }
    public bool IsFullScreen { get { return uiAddressableData.isFullScreen; } }
    public bool IsCaching { get { return uiAddressableData.isCaching; } }
    public int FixedSortingOrder { get { return uiAddressableData.fixedSortingOrder; } }

    protected virtual void Awake()
    {
        if (null != canvas)
        {
            canvas.enabled = false;
        }

        objectColorArr = this.GetComponentsInChildren<ObjectColor>(true);
    }

    protected void ShutdownApp()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void InitUI(UIAddressableData uiAddressableData, int stackCount)
    {
        this.uiAddressableData = uiAddressableData;
        canvas = GetComponent<Canvas>();
        canvas.overrideSorting = true;

        if (FixedSortingOrder != 0)
            canvas.sortingOrder = FixedSortingOrder;
        else
            canvas.sortingOrder = stackCount;

        OnInit();
    }

    public void ActiveUI()
    {
        canvas.enabled = true;
        OnActive();
    }

    public void InactiveUI()
    {
        canvas.enabled = false;
        OnInactive();
    }

    public void UpdateFrameUI()
    {
        if (canvas.enabled == false)
        {
            return;
        }

        OnUpdateFrame();
    }

    /// <summary>
    /// 매 초마다 호출되는 함수
    /// </summary>
    public void UpdateSecUI()
    {
        if (canvas.enabled == false)
        {
            return;
        }

        OnUpdateSec();
    }

    /// <summary>
    /// UI가 사라질 때 호출 되는 함수 
    /// </summary>
    public void ClearUI()
    {
        OnClear();
        uiAddressableData = null;
        GameObject.Destroy(gameObject, 0f);
    }

    public virtual void OnInit() { }

    public virtual void OnActive() 
    {
        var strData = App.Instance.DataTable.GetStringData(DataTableMngr.PLAYER_PREFAB_APP_COLOR_ID);
        if (!string.IsNullOrEmpty(strData) && int.TryParse(strData, out int _appColorId))
        {
            if (_appColorId != 0)
            {
                var appColor = App.Instance.DataTable.GetAppColor(_appColorId);
                if (null != appColor)
                {
                    if (null != objectColorArr && objectColorArr.Length > 0)
                    {
                        foreach (var obj in objectColorArr)
                        {
                            Color newColor = appColor.GetColor(obj.appColor);
                            obj.ChangeAppColor(newColor);
                        }
                    }
                }
            }
        }
    }

    public virtual void OnInactive() { }
    public virtual void OnUpdateFrame() { }
    public virtual void OnUpdateSec() { }
    public virtual void OnClear() { }
    public virtual bool IsEscape() { return true; }
}