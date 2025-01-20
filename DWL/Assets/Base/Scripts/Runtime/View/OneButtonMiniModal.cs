using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneButtonMiniModal : MonoBehaviour
{
    public Text descriptionText;
    public Text buttonText;

    private Action closeCallback;

    public void Show(string description, Action closeCallback = null)
    {
        gameObject.SetActive(true);

        this.closeCallback = closeCallback;
        descriptionText.text = description;
        //buttonText.text = LanguageMngr.Instance.GetLanguage("CONFIRM");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        closeCallback?.Invoke();
        Hide();
    }
}