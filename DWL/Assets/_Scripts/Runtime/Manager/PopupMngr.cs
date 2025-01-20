using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public virtual void ShowPopup(PopupSetting settings, Action callbackClose) { }
    public virtual void ClosePopup() { }
}

public class PopupSetting
{
    public enum eButtonType
    {
        None = -1,
        OneButton,
        TwoButton,
    }

    public string Title { get; set; }
    public string Desc { get; set; }
    public eButtonType ButtonType { get; set; }
    public string[] ButtonTexts { get; set; }
    public Action CallbackPositive { get; set; }
    public Action CallbackNegative { get; set; }

    /// <summary>
    /// General Popup 
    /// </summary>
    public PopupSetting(string title, string desc, eButtonType buttonType, string[] buttonsTexts, 
        Action callbackPositive, Action callbackNegative) 
    {
        this.Title = title;
        this.Desc = desc;
        this.ButtonType = buttonType;
        this.ButtonTexts = buttonsTexts;
        this.CallbackPositive = callbackPositive;
        this.CallbackNegative = callbackNegative;
    }

    /// <summary>
    /// Toast Popup
    /// </summary>
    public PopupSetting(string desc)
    {
        this.Desc = desc;
        this.ButtonType = eButtonType.None;
        this.ButtonTexts = new string[0];
        this.CallbackPositive = null;
        this.CallbackNegative = null;
    }
}

public class PopupMngr : MonoBehaviour, IMngr
{
    [Serializable]
    public class PopupPoolItem
    {
        public Popup popupPrefab;
        public int aountToPool;
    }

    /// UI의 최상위 Canvas로, 해당 Canvas하위에 UI가 생성되고 삭제됨
    public Canvas rootCanvas;
    public List<PopupPoolItem> itemToPool;
    
    private List<Popup> pooledPopupLst;
    private Stack<Popup> activePopupStack;

    [SerializeField] private List<Popup> popupPrefabLst;

    private Stack<Popup> popupStack = new Stack<Popup>();

    public void Init()
    {
        pooledPopupLst = new List<Popup>();
        activePopupStack = new Stack<Popup>();

        foreach (var item in itemToPool) 
        {
            for (int index = 0; index < item.aountToPool; index++) 
            {
                Popup popup = Instantiate(item.popupPrefab, rootCanvas.transform);
                popup.gameObject.SetActive(false);
                pooledPopupLst.Add(popup);
            }
        }
    }

    public void UpdateFrame() 
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            HidePopup();
        }
    }

    public void UpdateSec() { }
    public void Clear() { }

    public void ShowPopup<T>(PopupSetting settings) where T : Popup
    {
        T popup = GetPopup<T>();
        if (null != popup)
        {
            activePopupStack.Push(popup);
            popup.ShowPopup(settings, HidePopup);
        }
    }

    private void HidePopup()
    {
        if (activePopupStack.Count > 0) 
        {
            Popup popup = activePopupStack.Pop();
            popup.ClosePopup();
        }
    }

    private T GetPopup<T>() where T : Popup
    {
        foreach (var popup in pooledPopupLst) 
        {
            if (!popup.gameObject.activeInHierarchy && popup is T)
            {
                return popup as T;
            }
        }

        return null;
    }
}