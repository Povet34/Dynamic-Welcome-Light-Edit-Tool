using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[AddComponentMenu("UI/Custom/UIText")]
public class UITextMeshPro : TextMeshProUGUI
{
    public bool IsApplyLocalText;
    
    public string LocalTextKey;

    public bool IsAutoFitable;

    public override string text 
    { 
        get => base.text; 
        set
        {
            if (null != value)
            {
                base.text = value;

                if (IsAutoFitable)
                {
                    ContentSizeFitter sizeFitter = GetComponent<ContentSizeFitter>();
                    if (null != sizeFitter)
                        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)sizeFitter.transform);
                }
            }
        }
    }

    protected override void Start()
    {
        if (IsApplyLocalText && UnityEngine.Application.isPlaying)
        {
            App.Instance.Language.AddLocalTextMesh(this);
            ApplyLocalText();
        }
        
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void ApplyLocalText()
    {
        if (!string.IsNullOrEmpty(LocalTextKey))
            text = App.Instance.Language.GetLanguageText(LocalTextKey);
    }
}