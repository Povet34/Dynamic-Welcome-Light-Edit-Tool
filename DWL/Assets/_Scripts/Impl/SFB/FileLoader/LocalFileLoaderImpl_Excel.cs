using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalFileLoaderImpl_Excel : ILocalFileLoader
{
    string[] filter = new string[2] { "xlsx", "xls" };
    public string OpenFilePath()
    {
        var extensions = new[] {
        new ExtensionFilter("Excel Files", filter),
        new ExtensionFilter("All Files", "*" ),
    };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Excel File", "", extensions, false);

        if (paths.Length > 0)
            return paths[0];

        return null;
    }

    public string RemoveFileExtension(string fileName)
    {
        foreach (var ext in filter)
        {
            if (fileName.Contains(ext))
                return fileName.Replace($".{ext}", "");
        }
        return fileName;
    }
}
