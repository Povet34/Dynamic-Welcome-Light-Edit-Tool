using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class DataTableMngr : MonoBehaviour, IMngr
{
    public void Init()
    {
        LoadAppColorVariation();
    }

    public void UpdateFrame() { }
    public void UpdateSec() { }
    public void Clear() { }

    #region Player Prefabs : --------------------------------------------------
    public const string PLAYER_PREFAB_KEY_AUTO_LOGIN    = "AutoLogin";
    public const string PLAYER_PREFAB_APP_COLOR_ID      = "AppColorID";

    public const string PLAYER_PREFAB_AUTH_EMAIL        = "AuthEmail";
    public const string PLAYER_PREFAB_AUTH_PASSWORD     = "AuthPassword";

    public void SetStringData(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public string GetStringData(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            if (PlayerPrefs.HasKey(key))
                return PlayerPrefs.GetString(key);
        }

        return string.Empty;
    }

    public bool HasStringData(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    #endregion

    #region App Color : ---------------------------------------------
    public const string APP_COLOR_NAME = "app_color";

    public enum eAppColor
    {
        None = -1,
        PrimaryColor1,
        PrimaryColor2,
        PrimaryColor3,
        SecondaryColor1,
        SecondaryColor2,
        SecondaryColor3,
        GrayScale1,
        GrayScale2,
        GrayScale3,
        GrayScale4,
        GrayScale5,
        GrayScale6,
        GrayScale7,
        GrayScale8,
        GrayScale9,
        GrayScale10,
    }

    public class AppColor
    {
        public string PrimaryColor1 { get; set; }
        public string PrimaryColor2 { get; set; }
        public string PrimaryColor3 { get; set; }

        public string SecondaryColor1 { get; set; }
        public string SecondaryColor2 { get; set; }
        public string SecondaryColor3 { get; set; }

        public string GrayScale1 { get; set; }
        public string GrayScale2 { get; set; } 
        public string GrayScale3 { get; set; }
        public string GrayScale4 { get; set; }
        public string GrayScale5 { get; set; }
        public string GrayScale6 { get; set; }
        public string GrayScale7 { get; set; }
        public string GrayScale8 { get; set; }
        public string GrayScale9 { get; set; }
        public string GrayScale10 { get; set; }

        public AppColor(string primaryColor1, string primaryColor2, string primaryColor3, string secondaryColor1, string secondaryColor2, string secondaryColor3, 
            string grayScale1, string grayScale2, string grayScale3, string grayScale4, string grayScale5, string grayScale6, string grayScale7, string grayScale8, string grayScale9, string grayScale10)
        {
            PrimaryColor1 = primaryColor1;
            PrimaryColor2 = primaryColor2;
            PrimaryColor3 = primaryColor3;
            SecondaryColor1 = secondaryColor1;
            SecondaryColor2 = secondaryColor2;
            SecondaryColor3 = secondaryColor3;
            GrayScale1 = grayScale1;
            GrayScale2 = grayScale2;
            GrayScale3 = grayScale3;
            GrayScale4 = grayScale4;
            GrayScale5 = grayScale5;
            GrayScale6 = grayScale6;
            GrayScale7 = grayScale7;
            GrayScale8 = grayScale8;
            GrayScale9 = grayScale9;
            GrayScale10 = grayScale10;
        }

        public Color GetColor(eAppColor appColor)
        {
            string newColor = string.Empty;
            switch (appColor)
            {
                case eAppColor.PrimaryColor1:
                    newColor = this.PrimaryColor1;
                    break;
                case eAppColor.PrimaryColor2:
                    newColor = this.PrimaryColor2;
                    break;
                case eAppColor.PrimaryColor3:
                    newColor = this.PrimaryColor3;
                    break;
                case eAppColor.SecondaryColor1:
                    newColor = this.SecondaryColor1;
                    break;
                case eAppColor.SecondaryColor2:
                    newColor = this.SecondaryColor2;
                    break;
                case eAppColor.SecondaryColor3:
                    newColor = this.SecondaryColor3;
                    break;
                case eAppColor.GrayScale1:
                    newColor = this.GrayScale1;
                    break;
                case eAppColor.GrayScale2:
                    newColor = this.GrayScale2;
                    break;
                case eAppColor.GrayScale3:
                    newColor = this.GrayScale3;
                    break;
                case eAppColor.GrayScale4:
                    newColor = this.GrayScale4;
                    break;
                case eAppColor.GrayScale5:
                    newColor = this.GrayScale5;
                    break;
                case eAppColor.GrayScale6:
                    newColor = this.GrayScale6;
                    break;
                case eAppColor.GrayScale7:
                    newColor = this.GrayScale7;
                    break;
                case eAppColor.GrayScale8:
                    newColor = this.GrayScale8;
                    break;
                case eAppColor.GrayScale9:
                    newColor = this.GrayScale9;
                    break;
                case eAppColor.GrayScale10:
                    newColor = this.GrayScale10;
                    break;
            }

            if (ColorUtility.TryParseHtmlString(newColor, out Color _newColor))
                return _newColor;

            return Color.white;
        }
    }

    private Dictionary<int, AppColor> appColorDic = new Dictionary<int, AppColor>();

    public AppColor GetAppColor(int appColorId)
    {
        if (null != appColorDic && appColorDic.ContainsKey(appColorId)) 
            return appColorDic[appColorId];

        return null;
    }

    private void LoadAppColorVariation()
    {
        appColorDic.Clear();
        TextAsset file = Resources.Load<TextAsset>(APP_COLOR_NAME);
        if (null != file)
        {
            string[] lines = file.text.Split("\r\n");
            if (lines.Length > 0)
            {
                // 헤더 분석
                string[] headers = lines[0].Split(',');
                List<int> validColumns = new List<int>();
                for (int index = 0; index < headers.Length; index++) 
                {
                    if (!headers[index].Contains("#"))
                        validColumns.Add(index);
                }

                for (int index = 1; index < lines.Length; index++)
                {
                    int id = 0;
                    string[] fields = lines[index].Split(',');
                    List<string> colors = new List<string>();
                    foreach (int columnIndex in validColumns)
                    { 
                        if (columnIndex < fields.Length)
                        {
                            if (columnIndex == 0) // 첫뻔재 유효한 열은 ID로 가정
                            {
                                if (!int.TryParse(fields[columnIndex], out id))
                                {
                                    break;
                                }
                            }
                            else
                            {
                                colors.Add(fields[columnIndex]);
                            }
                        }
                    }

                    if (id != 0 && colors.Count >= 16)
                    {
                        AppColor appColor = new AppColor(colors[0], colors[1], colors[2], colors[3], colors[4], colors[5], colors[6], colors[7], colors[8], colors[9],
                            colors[10], colors[11], colors[12], colors[13], colors[14], colors[15]);
                        appColorDic[id] = appColor;
                    }
                }
            }
        }
    }
    #endregion
}