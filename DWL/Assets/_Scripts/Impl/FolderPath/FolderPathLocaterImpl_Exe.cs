using System.IO;
using UnityEngine;

public class FolderPathLocaterImpl_Exe : IFolderPathLocater
{
    public string GetLocatedFolderPath(string folderName)
    {
        string executablePath = Application.dataPath;
        string directoryPath = Path.GetDirectoryName(executablePath);
        string newFolderPath = Path.Combine(directoryPath, folderName);

        if (!Directory.Exists(newFolderPath))
        {
            Directory.CreateDirectory(newFolderPath);
        }

        return newFolderPath;
    }
}
