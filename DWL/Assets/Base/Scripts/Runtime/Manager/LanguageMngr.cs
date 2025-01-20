using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neofect.BodyChecker.Language;


[SerializeField]
public enum eLanguage
{
    None = -1,
    ko_KR,
    en_US,
}

public class LanguageMngr : MonoBehaviour, IMngr
{
    public const string LANGUAGE_FILE_NAME = "language";
    private const string EMPTY = "empty";

    public CsvCreator csvCreator;

    private List<Dictionary<string, string>> languageDic = new List<Dictionary<string, string>>();
    

    private eLanguage curLanguage = eLanguage.ko_KR;
    public eLanguage CurLanguage { get { return curLanguage; } }

    List<UITextMeshPro> localTextMeshLst = new List<UITextMeshPro>();

    public void Init()
    {
        curLanguage = GetLanguage();
        LoadLanguageFile();
    }

    public void UpdateFrame() { }
    public void UpdateSec() { }
    public void Clear() { }

    public string GetLanguageText(string key)
    {
        if (languageDic[Convert.ToInt32(curLanguage)].ContainsKey(key))
            return languageDic[Convert.ToInt32(curLanguage)][key];
        else
            return EMPTY;
    }

    #region Local Text : ------------------------------------------------------
    public void RefreshLocalTextMesh()
    {
        if (null != localTextMeshLst && localTextMeshLst.Count > 0)
        {
            foreach (var localText in localTextMeshLst)
                localText.ApplyLocalText();
        }
    }

    public void AddLocalTextMesh(UITextMeshPro textMesh)
    {
        if (null != textMesh)
            localTextMeshLst.Add(textMesh);
    }
    #endregion

    private void LoadLanguageFile()
    {
        languageDic.Clear();

        for (int i = 0; i < csvCreator.languageColumnList.Count - 1; i++)
            languageDic.Add(new Dictionary<string, string>());

        TextAsset csv = Resources.Load<TextAsset>(LANGUAGE_FILE_NAME);
        string localeText = (csv == null) ? string.Empty : csv.text;

        if (localeText.Length > 0)
        {
            var csvReader = new CsvParser();
            csvReader.ReadStream(localeText);

            for (int i = 0; i < csvReader.GetRowCount(); i++)
            {
                var rowList = csvReader.GetRow(i);
                var key = rowList[0];
                for (int j = 1; j < csvCreator.languageColumnList.Count; j++)
                {
                    var replaced = rowList[j].Replace("\\n", "\n");
                    languageDic[j - 1].Add(key, replaced);
                }
            }
        }
        else
        {
            NDebug.Log($"no {LANGUAGE_FILE_NAME} file!");
        }
    }

    private eLanguage GetLanguage()
    {
        return UnityEngine.Application.systemLanguage switch
        {
            SystemLanguage.Korean => eLanguage.ko_KR,
            SystemLanguage.English => eLanguage.en_US,
            _ => eLanguage.en_US,
        };
    }

    public bool IsLoaded()
    {
        return languageDic.Count > 0;
    }
}