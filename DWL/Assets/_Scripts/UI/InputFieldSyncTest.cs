using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputFieldSyncTest : MonoBehaviour, IPointerDownHandler
{
    public TMP_InputField ip;
    public TextMeshProUGUI text;

    bool isProgrammaticChange = false;

    private void Awake()
    {
        ip.onValueChanged.AddListener(OnDutyInput);
    }

    void OnDutyInput_Programmatically(string dutyText)
    {
        isProgrammaticChange = true;
        ip.text = dutyText;
        isProgrammaticChange = false;
    }

    void OnDutyInput(string value)
    {
        if (isProgrammaticChange)
        {
            Debug.Log($"OnDutyInput : {value}");
        }
        else
        {
            text.text = value;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDutyInput_Programmatically(text.text);
    }
}
