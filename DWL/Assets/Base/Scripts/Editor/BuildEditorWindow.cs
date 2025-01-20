using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Common.Utility;

public class BuildEditorWindow : EditorWindow
{
    private SerializedObject serializedSettings;

    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Neofect/Open BuildEditorWindow")]
    private static void OpenBuildEditorWindow()
    {
        var builder = GetWindow<BuildEditorWindow>();
        builder.Show();
    }

    private void OnEnable()
    {
        serializedSettings = new SerializedObject(OptionSettings.GetInstance());
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Height(position.height));

        //EditorStyles.label.richText = true;
        //GUI.skin.button.richText = true;

        DrawDummyOption();
        DrawAPKBuilding();

        GUILayout.EndScrollView();

        serializedSettings.ApplyModifiedProperties();
        serializedSettings.Update();
    }

    private void DrawDummyOption()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("> 더미 옵션", EditorStyles.whiteLabel);
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("existDummyOne"), new GUIContent("더미 1"));
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("existDummyTwo"), new GUIContent("더미 2"));
        EditorGUILayout.LabelField("> 언어 선택", EditorStyles.whiteLabel);
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("language"), new GUIContent("언어"));
        EditorGUILayout.LabelField("> 빌드 옵션", EditorStyles.whiteLabel);
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("env"), new GUIContent("개발환경"));
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("med"), new GUIContent("타겟의학"));
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("versionName"), new GUIContent("버전이름"));
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("versionCode"), new GUIContent("버전코드"));
        EditorGUILayout.PropertyField(serializedSettings.FindProperty("isDllShifting"), new GUIContent("DLL이동"));
    }

    private void DrawAPKBuilding()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("> 빌드하기", EditorStyles.whiteLabel);
        if (GUILayout.Button("빌드하기"))
        {
            SetBuildOption();
            Build();
        }
    }

    void SetBuildOption()
    {
        if(OptionSettings.GetInstance().env == DeveoplomentEnvironmnet.Live)
            PlayerSettings.productName = "SmartBodyChecker";
        else if (OptionSettings.GetInstance().env == DeveoplomentEnvironmnet.Stage)
            PlayerSettings.productName = "SmartBodyCheckerStage";
        else
            PlayerSettings.productName = "SmartBodyCheckerDev";
        PlayerSettings.bundleVersion = OptionSettings.GetInstance().versionName;
    }
    
    void Build()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose location", "", "");
        if(path.Length != 0)
        {
            string[] levels = new string[] { "Assets/01.Main/Scenes/Main.unity" };

            // Build player.
            BuildPipeline.BuildPlayer(levels, path + $"/build/{Application.productName}.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

            //if(OptionSettings.GetInstance().isDllShifting)
            //    MoveFiles(path);
        }
    }

    private static void MoveFiles(string path)
    {
        MoveRootFiles(path);
        MovePluginsFiles(path);
        Debug.Log("All dll Files moved");
    }

    private static void MoveRootFiles(string path)
    {
        var desDllRootPath = Path.Combine(path, "dll_root");
        PathUtility.CreateFolder(desDllRootPath);

        var srcDllRootPath = Path.Combine(path, "build");
        var dllRootDirectoryInfo = new DirectoryInfo(srcDllRootPath);

        var dllRootfileInfos = dllRootDirectoryInfo.GetFiles("*.dll");
        foreach (var fileInfo in dllRootfileInfos)
        {
            if (fileInfo.Name == "UnityPlayer.dll")
                continue;
            File.Move(fileInfo.FullName, Path.Combine(desDllRootPath, fileInfo.Name));
        }

        var onnxRootfileInfos = dllRootDirectoryInfo.GetFiles("*.onnx");
        foreach (var fileInfo in onnxRootfileInfos)
        {
            File.Move(fileInfo.FullName, Path.Combine(desDllRootPath, fileInfo.Name));
        }
    }

    private static void MovePluginsFiles(string path)
    {
        var desDllPluginsPath = Path.Combine(path, "dll_plugins");
        PathUtility.CreateFolder(desDllPluginsPath);

        var srcDllPluginsPath = Path.Combine(path, "build", $"{Application.productName}_Data", "Plugins", "x86_64");
        var dllPluginsDirectoryInfo = new DirectoryInfo(srcDllPluginsPath);

        var dllPluginsFileInfos = dllPluginsDirectoryInfo.GetFiles("*.dll");
        foreach (var fileInfo in dllPluginsFileInfos)
        {
            if (fileInfo.Name == "sqlite3.dll" || fileInfo.Name == "PrintLibDll.dll")
                continue;
            File.Move(fileInfo.FullName, Path.Combine(desDllPluginsPath, fileInfo.Name));
        }
    }
}
