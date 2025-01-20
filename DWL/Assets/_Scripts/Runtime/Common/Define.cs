using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class Define
{
    public enum eScene
    {
        NONE = -1,
        INIT,
        LOBBY,
        GAME,
    }

    public enum eCameraMode
    {
        QuarterView,
    }

    public static readonly string[] DEV_ENV_ARR = new string[] { "Dev", "Test", "Product" };

    #region UI Generate : -----------------------------------------------------
    public const string GENERATE_UI_FOLDER_PATH     = "Assets/_Scripts/Runtime/UI/Content/";
    #endregion

    #region Data Parser : -----------------------------------------------------
    public const string EXTENSION_EXCEL = ".xlsx";
    public const string EXTENSION_CSV = ".csv";

    /// <summary>
    /// enum 데이터
    /// </summary>
    public const string ENUMS_TABLE_NAME = "enums_table.xlsx";
    /// <summary>
    /// 언어별 text 데이터
    /// </summary>
    public const string TEXT_TABLE_NAME = "text_table.xlsx";
    /// <summary>
    /// 상품 판매 관련, 텍스트 제외 모든 데이터
    /// </summary>
    public const string DATA_TABLE_NAME = "data_table.xlsx";
    /// <summary>
    /// 마켓 판매 상품 및 인 게임 내 판매 상품 등록 & 관리
    /// </summary>
    public const string SHOP_TABLE_NAME = "shop_table.xlsx";

    public const string GENERATE_ENUM_TABLE_PATH = "Assets/Script/Generate/GenerateEnumTable.cs";
    public const string GENERATE_DATA_TABLE_PATH = "Assets/Script/Generate/GenerateDataTable.cs";
    public const string GENERATE_DATA_MANAGER_PATH = "Assets/Script/Generate/GenerateDataManager.cs";
    public const string DATA_MANAGER_PATH = "Assets/Script/Manager/DataManager.cs";

    public static string GetDataSheetPath(string devEnv)
    {
        string path = Path.Combine(UnityEngine.Application.dataPath, "DataTable", "DataSheet", devEnv);
        return path;
    }

    public static string GetDataFilePath(string devEnv)
    {
        string path = Path.Combine(UnityEngine.Application.dataPath, "DataTable", "DataFile", devEnv);
        return path;
    }
    #endregion

    public static bool IsError(this string err, Error errorCode)
    {
        return (Error)Convert.ToInt32(err) == errorCode;
    }

    public static Error ToError(this string err)
    {
        return (Error)Convert.ToInt32(err);
    }
}