using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GoogleSheetsToUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Neofect.BodyChecker.Language
{
    [CreateAssetMenu(fileName = "CsvCreator", menuName = "Scriptable Object Asset/Create CsvCreator Asset")]
    public class CsvCreator : ScriptableObject
    {
        public const string LANGUAGE_CSV_FILE_NAME = "language.csv";
        public static readonly string LANGUAGE_FILE_FULL_PATH = $"{Application.dataPath}\\Resources\\{LANGUAGE_CSV_FILE_NAME}";

        public string associatedSheet = "";

        public string languageWorksheet = "";
        public string languageStartCell = "A1";
        public string languageEndCell = "C10";
        public List<string> languageColumnList = new List<string>();
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CsvCreator))]
    public class LanguageDownloaderEditor : Editor
    {
        CsvCreator data;

        void OnEnable()
        {
            data = (CsvCreator)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("associatedSheet"));
            EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("languageWorksheet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("languageStartCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("languageEndCell"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("languageColumnList"));
            if (GUILayout.Button("Create language file"))
            {
                UpdateSheet(data.associatedSheet, data.languageWorksheet, data.languageStartCell, data.languageEndCell, CreateLanguageFile);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateSheet(string sheetId, string worksheetName, string startCell, string endCell, UnityAction<GstuSpreadSheet> callback)
        {
            SpreadsheetManager.Read(new GSTU_Search(sheetId, worksheetName, startCell, endCell), callback, false);
        }

        #region language file
        private int parsingIndex = 0;
        bool isSameKeyExist = false;
        Dictionary<string, int> sameKeyDic;
        string sameKey = "";

        void CreateLanguageFile(GstuSpreadSheet ss)
        {
            try
            {
                InitSameKeyExist();
                parsingIndex = 0;
                using (System.IO.StreamWriter file = new System.IO.StreamWriter($"{CsvCreator.LANGUAGE_FILE_FULL_PATH}", false, System.Text.Encoding.GetEncoding("utf-8")))
                {
                    for (int i = 0; i < ss.columns["LanguageKey"].Count; i++)
                    {
                        string str = "";
                        for (int cIndex = 0; cIndex < data.languageColumnList.Count; cIndex++)
                        {
                            var t = ss.columns[data.languageColumnList[cIndex]][i].value;
                            str += $"{GetStringFormat(t)},";
                        }
                        str = str.Substring(0, str.Length - 1);
                        file.WriteLine(str);
                        parsingIndex = i;
                        CheckSameKeyExist(ss.columns["LanguageKey"][i].value);
                    }
                }
                AssetDatabase.Refresh();
                if (isSameKeyExist)
                    EditorUtility.DisplayDialog("경고", $"중복된 키가 있습니다.({sameKey})", "확인");
                else
                    EditorUtility.DisplayDialog("알림", $"{CsvCreator.LANGUAGE_CSV_FILE_NAME} 파일이 생성되었습니다.", "확인");

            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
            }
        }

        void InitSameKeyExist()
        {
            isSameKeyExist = false;
            sameKeyDic = new Dictionary<string, int>();
            sameKey = "";
        }

        void CheckSameKeyExist(string key)
        {
            if (sameKeyDic.ContainsKey(key))
            {
                isSameKeyExist = true;
                sameKey = key;
            }
            else
            {
                sameKeyDic.Add(key, 1);
            }
        }

        string GetStringFormat(string text)
        {
            text = text.Replace("\"", "\\\"");
            if (text.Contains(","))
            {
                return $"\"{text}\"";
            }
            else
            {
                return text;
            }
        }
        #endregion
    }
#endif
}
