using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Common.Utility;

public class PathUtilityEditor : MonoBehaviour
{
    [MenuItem("Neofect/OpenSaveFolder")]
    static void OpenSaveFolder()
    {
        var path = PathUtility.GetSaveFolder();
        if (Directory.Exists(path))
            System.Diagnostics.Process.Start(path);
        else
            Debug.Log($"{path} doesn't exist");
    }

    [MenuItem("Neofect/OpenProgramFolder")]
    static void OpenProgramFolder()
    {
        var path = PathUtility.GetProgramParentFolder();
        if(Directory.Exists(path))
            System.Diagnostics.Process.Start(path);
        else
            Debug.Log($"{path} doesn't exist");
    }
}