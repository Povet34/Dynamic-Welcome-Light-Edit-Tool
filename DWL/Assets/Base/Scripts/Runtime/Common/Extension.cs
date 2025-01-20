using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Neofect.Utility;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using TMPro;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Utility.GetOrAddComponent<T>(go);
    }

    public static void BindButtonEvent(this UIButton button, string buttonData, UnityAction<string> clickCallback, UnityAction<string, PointerEventData> downCallback = null,
        UnityAction<string, PointerEventData> upCallback = null)
    {
        button.SetCallback(buttonData, clickCallback, downCallback, upCallback);
    }

    public static void BindToggleEvent(this UIToggle toggle, string toggleData, UnityAction<bool, string> clickCallback)
    {
        toggle.SetCallback(toggleData, clickCallback);
    }

    public static void BindInputFieldEvent(this UIInputField inputField, UnityAction<string> onEndEditCallback,
        UnityAction<string> onSubmitCallback, UnityAction<string> onValueChangedCallback,
        UnityAction<string> onSelectCallback = null, UnityAction<string> onDeselectCallback = null)
    {
        inputField.SetCallback(onEndEditCallback, onSubmitCallback, onValueChangedCallback, onSelectCallback, onDeselectCallback);
    }

    public static void Active(this GameObject go, bool isActive)
    {
        go?.SetActive(isActive);
    }

    public static void SetText(this UITextMeshPro meshPro, string text)
    {
        if (null != meshPro)
            meshPro.text = text;
    }
}