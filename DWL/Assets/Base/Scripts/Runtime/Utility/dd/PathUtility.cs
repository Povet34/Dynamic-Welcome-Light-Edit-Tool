using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Neofect.BodyChecker.Language;

namespace Common.Utility
{
    public class PathUtility : MonoBehaviour
    {
        private const string ASSET_DLL_FOLDER_ROOT = "/dll_root";
        private const string ASSET_DLL_FOLDER_PLUGINS = "/dll_plugins";

        private static string ProgramParentFolder 
        { 
            get 
            { 
                return Directory.GetParent(UnityEngine.Application.dataPath).FullName;
            } 
        }

        public static void CreateFolder(string path)
        {
            if(Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        public static void DeleteAndCreateFolder(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }

        public static string GetProgramParentFolder()
        {
            return ProgramParentFolder;
        }

        public static string GetLocalDBFolder(string addPath = "")
        {
            return $"{UnityEngine.Application.persistentDataPath}\\Local" + addPath;
        }















        #region download
        public static string GetDownloadFolder()
        {
            return $"{ProgramParentFolder}\\Download";
        }

        public static string GetInstallerFolder()
        {
            return $"{ProgramParentFolder}\\Download\\installer";
        }

        public static string GetInstallerFile()
        {
            return $"{GetInstallerFolder()}\\installer.exe";
        }
        #endregion

        #region program installed
        public static string GetProgramRootFolder()
        {
            return $"{ProgramParentFolder}";
        }

        public static string GetProgramPluginsFolder()
        {
            return $"{ProgramParentFolder}\\{UnityEngine.Application.productName}_Data\\Plugins\\x86_64";
        }

        public static bool IsProgramDllFolder(string assetServerFolder)
        {
            if (assetServerFolder == ASSET_DLL_FOLDER_ROOT || assetServerFolder == ASSET_DLL_FOLDER_PLUGINS)
                return true;
            else
                return false;
        }

        public static string GetProgramDestinationFolder(string assetServerFolder)
        {
            if (assetServerFolder == ASSET_DLL_FOLDER_ROOT)
                return GetProgramRootFolder();
            else if (assetServerFolder == ASSET_DLL_FOLDER_PLUGINS)
                return GetProgramPluginsFolder();
            else
                return "";
        }
        #endregion

        #region save
        public static string GetSaveFolder()
        {
            return $"{UnityEngine.Application.persistentDataPath}\\Save";
        }

        public static string GetMainDBFolder()
        {
            return $"{GetSaveFolder()}\\DB";
        }

        public static string GetReportFolder()
        {
            return $"{GetSaveFolder()}\\Report";
        }

        public static string GetRecordingFolder()
        {
            return $"{GetSaveFolder()}\\Recording";
        }

        public static string GetScreenShotFolder()
        {
            return $"{GetSaveFolder()}\\ScreenShot";
        }

        public static string GetReportFile(int fileIndex)
        {
            return $"{GetReportFolder()}\\temp{fileIndex:00}.png";
        }
        #endregion

        #region resource
        public static string GetLanguageFile()
        {
            return $"{UnityEngine.Application.dataPath}\\01.Main\\Csv\\Resources\\{LanguageMngr.LANGUAGE_FILE_NAME}";
        }

        public static string GetPressureFile()
        {
            return $"{UnityEngine.Application.dataPath}\\01.Main\\Csv\\Resources\\{CsvManager.PRESSURE_FILE_NAME}";
        }

        public static string GetJointMaxAngleFile()
        {
            return $"{UnityEngine.Application.dataPath}\\01.Main\\Csv\\Resources\\{CsvManager.JOINT_SETTING_NAME}";
        }
        #endregion

        

        public static string GetPdfFile()
        {
            return $"{UnityEngine.Application.streamingAssetsPath}\\sample.pdf";
        }
    }
}