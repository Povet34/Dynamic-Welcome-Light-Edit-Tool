using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPopup : Popup
{
    [SerializeField] private UITextMeshPro titleText;
    [SerializeField] private UITextMeshPro descText;

    [SerializeField] private UIButton button01;
    [SerializeField] private UIButton button02;
    [SerializeField] private UITextMeshPro buttonText01;
    [SerializeField] private UITextMeshPro buttonText02;

    private Action callbackPositive;
    private Action callbackNegative;
    private Action callbackClose;

    private ObjectColor[] objectColorArr;

    private void Start()
    {
        button01.BindButtonEvent("1", OnButton);
        button02.BindButtonEvent("2", OnButton);

        objectColorArr = this.GetComponentsInChildren<ObjectColor>();
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

    public override void ShowPopup(PopupSetting settings, Action callbackClose)
    {
        this.gameObject.SetActive(true);
        titleText.text = settings.Title;
        descText.text = settings.Desc;

        if (settings.ButtonTexts.Length >= 2)
        {
            if (!string.IsNullOrEmpty(settings.ButtonTexts[0]))
                buttonText01.text = settings.ButtonTexts[0];

            if (!string.IsNullOrEmpty(settings.ButtonTexts[1]))
                buttonText02.text = settings.ButtonTexts[1];
        }

        this.callbackPositive = settings.CallbackPositive;
        this.callbackNegative = settings.CallbackNegative;
        button01.gameObject.SetActive(settings.ButtonType == PopupSetting.eButtonType.OneButton 
            || settings.ButtonType == PopupSetting.eButtonType.TwoButton);
        button02.gameObject.SetActive(settings.ButtonType == PopupSetting.eButtonType.TwoButton);

        this.callbackClose = callbackClose;
    }

    public override void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }

    #region UI Event : --------------------------------------------------------
    public void OnButton(string buttonData)
    {
        if (!string.IsNullOrEmpty(buttonData))
        {
            if (buttonData.Equals("1"))
            {
                callbackPositive?.Invoke();
            }
            else if (buttonData.Equals ("2")) 
            {
                callbackNegative?.Invoke();
            }

            callbackClose?.Invoke();
        }
    }
    #endregion
}
