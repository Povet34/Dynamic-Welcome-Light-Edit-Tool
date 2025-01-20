using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateSettingsEditor : ScriptableObject
{
    private const string PATH = "Assets/Resources/";

    [MenuItem("Neofect/Create SmartBodyCheckerSettings Asset")]
    static void CreateAsset()
    {
        var fileName = string.Concat(PATH, OptionSettings.NAME, ".asset");
        if(File.Exists(fileName))
        {
            Debug.Log("�̹� ������ �����մϴ�.");
        }
        else
        {
            var settings = CreateInstance<OptionSettings>();
            AssetDatabase.CreateAsset(settings, fileName);
            AssetDatabase.SaveAssets();
        }
    }
}
