using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIMngr : MonoBehaviour, IMngr
{
    /// 게임에서 사용하는 모든 UI가 정의된 ScriptableObject
    public UIAddressable uIAddressable;
    /// UI의 최상위 Canvas로, 해당 Canvas하위에 UI가 생성되고 삭제됨
    public Canvas rootCanvas;

    /// 현재 생성되고 있는 UI 정보 
    UIAddressableData currentData = null;
    /// UI를 순서에 따라 순차적으로 생성하기 위해 Queue로 관리 
    Queue<int> requestUIQueue = new Queue<int>();
    /// 현재 화면에 보여지는 UI를 Stack으로 관리 
    Stack<UIBase> currentUIStack = new Stack<UIBase>();

    public void Push(eUIType uiType)
    {
        requestUIQueue.Enqueue(Convert.ToInt32(uiType));
    }

    public void Pop(eUIType uiType)
    {
        if (currentUIStack.Count <= 0)
            return;

        /// Pop할 eUIType을 찾기 위해 임시 Stack을 설정 
        Stack<UIBase> tempStack = new Stack<UIBase>();
        UIBase uiBase;
        while (currentUIStack.Count > 0)
        {
            uiBase = currentUIStack.Pop();
            if (uiBase.UIType == uiType)
            {
                /// pop하는 UI가 전체 화면이였던 경우, 남은 UI중 전체화면인 가장 상단의 UI를 활성화 
                if (uiBase.IsFullScreen)
                {
                    UIBase otherUIBase;
                    UIBase[] uiBaseArray = currentUIStack.ToArray();
                    for (int index = uiBaseArray.Length - 1; index >= 0; --index)
                    {
                        otherUIBase = uiBaseArray[index];
                        if (otherUIBase.IsFullScreen)
                        {
                            otherUIBase.ActiveUI();
                            break;
                        }
                    }
                }

                if (!uiBase.IsCaching)
                {
                    App.Instance.Resource.UnLoad(uiBase.AssetReference, uiBase.gameObject);
                    uiBase.ClearUI();
                }
                else
                {
                    uiBase.InactiveUI();
                }
            }
            else
            {
                tempStack.Push(uiBase);
            }
        }

        // 기존 UI의 순서를 맞추기 위해 tempStack을 다시 옮김 
        while (tempStack.Count > 0)
        {
            uiBase = tempStack.Pop();
            currentUIStack.Push(uiBase);
        }
    }

    /// 매 프레임마다 현재 UI를 업데이트하며, 추가할 ui가 있는 경우 ResourceManager에서 읽어옴 
    public void UpdateFrame()
    {
        UIBase uiBase;
        UIBase[] uiBaseArray = currentUIStack.ToArray();
        for (int i = 0, icount = uiBaseArray.Length; i < icount; ++i)
        {
            uiBase = uiBaseArray[i];
            if (null != uiBase)
            {
                uiBase.UpdateFrameUI();
            }
        }

        /// 추가할 UI가 있는 경우
        if (null != requestUIQueue && requestUIQueue.Count > 0)
        {
            ///현재 로드 중인 UI가 없는 경우
            if (null == currentData)
            {
                eUIType requestUI = (eUIType)requestUIQueue.Dequeue();
                OnLoad(requestUI);
            }
        }

        CheckKeyCode();
    }

    public void Init() { }
    public void UpdateSec() { }
    
    public void Clear() 
    {
        if (null != currentUIStack && currentUIStack.Count > 0)
        {
            while (currentUIStack.Count > 0) 
            { 
                UIBase ui = currentUIStack.Peek();
                if (null != ui)
                    Pop(ui.UIType);
            }
        }
    }

    /// UI를 불러오는 함수 
    private void OnLoad(eUIType uiType)
    {
        /// 정의된 UI 데이터로 부터 추가할 UI 데이터를 얻은 후 리소스 매니저를 통해 로드 
        currentData = uIAddressable.GetUIAddressableDataByUIType(uiType);
        if (null != currentData) 
        {
            App.Instance.Resource.Load(currentData.reference, rootCanvas.transform, OnComplete, OnFail);
        }
        else
        {
            Debug.LogError(string.Format("Not Fount UI Addressable Data: {0}", uiType));
        }
    }

    private void OnComplete(GameObject go)
    {
        /// UI의 기본 좌표 및 앵커 값을 수정
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.SetParent(rootCanvas.transform);
        rectTransform.localPosition = Vector3.zero;

        /// 추가된 UI가 전체화면인 경우 기존 UI에서 전체화면인 UI를 비활성화
        UIBase addUI = go.GetComponent<UIBase>();
        /// 추가된 UI 설정
        addUI.InitUI(currentData, currentUIStack.Count);
        addUI.ActiveUI();
        addUI.UpdateFrameUI();

        if (addUI.IsFullScreen)
        {
            UIBase[] uiBaseArray = currentUIStack.ToArray();
            UIBase uiBase;
            for (int i = uiBaseArray.Length - 1; i >= 0; --i)
            {
                uiBase = uiBaseArray[i];
                if (uiBase.IsFullScreen)
                {
                    uiBase.InactiveUI();
                    break;
                }
            }
        }

        /// 추가된 UI를 Stack에 추가
        currentUIStack.Push(addUI);
        currentData = null;
    }

    private void OnFail()
    {
        currentData = null;
    }

    private void CheckKeyCode()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (null != currentUIStack && currentUIStack.Count > 0) 
            {
                UIBase uiBase = currentUIStack.Peek();
                if (!uiBase.IsIgnoreEscape)
                {
                    if (uiBase.IsEscape())
                        Pop(uiBase.UIType);
                }
            }
        }
    }
}