using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoButtonMiniModalTexts
{
    public string description;
    public string continueButton;
    public string closeButton;

    public TwoButtonMiniModalTexts(string description, string continueButton, string closeButton)
    {
        this.description = description;
        this.continueButton = continueButton;
        this.closeButton = closeButton;
    }
}

public class TwoButtonMiniModalActions
{
    public Action continueAction;
    public Action closeAction;
    public Action xButtonAction;

    public TwoButtonMiniModalActions(Action continueAction, Action closeAction, Action xButtonAction)
    {
        this.continueAction = continueAction;
        this.closeAction = closeAction;
        this.xButtonAction = xButtonAction;
    }
}

public class TwoButtonMiniModalButtonColors
{
    public TwoButtonsMiniModal.ButtonColor continueColor;
    public TwoButtonsMiniModal.ButtonColor closeColor;

    public TwoButtonMiniModalButtonColors(TwoButtonsMiniModal.ButtonColor continueColor, TwoButtonsMiniModal.ButtonColor closeColor)
    {
        this.continueColor = continueColor;
        this.closeColor = closeColor;
    }
}

public class TwoButtonsMiniModal : MonoBehaviour
{
    public enum ButtonColor
    { 
        Blue,
        White
    }

    public Text descriptionText;
    public Text continueButtonBlueText;
    public Text continueButtonWhiteText;
    public Text closeButtonBlueText;
    public Text closeButtonWhiteText;

    public GameObject continueButtonBlue;
    public GameObject continueButtonWhite;
    public GameObject closeButtonBlue;
    public GameObject closeButtonWhite;

    private Action continueAction;
    private Action closeAction;
    private Action xButtonAction;

    public void Show(TwoButtonMiniModalTexts twoButtonMiniModalTexts, TwoButtonMiniModalActions twoButtonMiniModalActions, TwoButtonMiniModalButtonColors twoButtonMiniModalButtonColors)
    {
        continueAction = twoButtonMiniModalActions.continueAction;
        closeAction = twoButtonMiniModalActions.closeAction;
        xButtonAction = twoButtonMiniModalActions.xButtonAction;

        gameObject.SetActive(true);

        descriptionText.text = twoButtonMiniModalTexts.description;
        continueButtonBlueText.text = twoButtonMiniModalTexts.continueButton;
        continueButtonWhiteText.text = twoButtonMiniModalTexts.continueButton;
        closeButtonBlueText.text = twoButtonMiniModalTexts.closeButton;
        closeButtonWhiteText.text = twoButtonMiniModalTexts.closeButton;

        SetContinueButtonColor(twoButtonMiniModalButtonColors.continueColor);
        SetCloseButtonColor(twoButtonMiniModalButtonColors.closeColor);
    }

    void SetContinueButtonColor(ButtonColor continueButtonColor)
    {
        if (continueButtonColor == ButtonColor.Blue)
        {
            continueButtonBlue.SetActive(true);
            continueButtonWhite.SetActive(false);
        }
        else
        {
            continueButtonBlue.SetActive(false);
            continueButtonWhite.SetActive(true);
        }
    }

    void SetCloseButtonColor(ButtonColor closeButtonColor)
    {
        if (closeButtonColor == ButtonColor.Blue)
        {
            closeButtonBlue.SetActive(true);
            closeButtonWhite.SetActive(false);
        }
        else
        {
            closeButtonBlue.SetActive(false);
            closeButtonWhite.SetActive(true);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnClickContinue()
    {
        Hide();
        continueAction?.Invoke();
    }

    public void OnClickClose()
    {
        Hide();
        closeAction?.Invoke();
    }

    public void OnClickXButton()
    {
        Hide();
        xButtonAction?.Invoke();
    }
}
