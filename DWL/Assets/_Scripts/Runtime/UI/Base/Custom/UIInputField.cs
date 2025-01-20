using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[AddComponentMenu("UI/Custom/UIInputField")]
public class UIInputField : TMP_InputField, UIResetCallback
{
    protected override void Start()
    {
        if (null == base.fontAsset)
        {
            TMP_FontAsset fontAsset = Resources.Load("NotoSansCJKkr-Medium", typeof(TMP_FontAsset)) as TMP_FontAsset;
            base.fontAsset = fontAsset;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public virtual void ResetCallback()
    {
        base.onEndEdit.RemoveAllListeners();
        base.onSubmit.RemoveAllListeners();
        base.onValueChanged.RemoveAllListeners();

        base.onSelect.RemoveAllListeners();
        base.onDeselect.RemoveAllListeners();
    }

    public void SetCallback(UnityAction<string> onEndEditCallback, UnityAction<string> onSubmitCallback = null, UnityAction<string> onValueChangedCallback = null,
        UnityAction<string> onSelectCallback = null, UnityAction<string> onDeselectCallback = null)
    {
        if (onEndEditCallback != null)
            base.onEndEdit.AddListener(onEndEditCallback);
        if (null != onSubmitCallback)
            base.onSubmit.AddListener(onSubmitCallback);
        if (null != onValueChangedCallback)
            base.onValueChanged.AddListener(onValueChangedCallback);

        if (null != onSelectCallback)
            base.onSelect.AddListener(onSelectCallback);
        if (null != onDeselectCallback)
            base.onDeselect.AddListener(onDeselectCallback);
    }
}