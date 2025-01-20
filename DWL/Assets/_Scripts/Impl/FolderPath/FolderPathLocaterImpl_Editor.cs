using UnityEngine;
using System.IO;

public class FolderPathLocaterImpl_Editor : IFolderPathLocater
{
    public string GetLocatedFolderPath(string folderName)
    {
        string folderPath = @"C:\" + folderName;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        return folderPath;
    }
}
