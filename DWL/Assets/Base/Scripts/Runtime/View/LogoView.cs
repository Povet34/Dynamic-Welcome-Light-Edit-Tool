using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Neofect.BodyChecker.Utility;

public class LogoView : MonoBehaviour
{
    [SerializeField] private Text versionText;
    [SerializeField] private Text descriptionText;

    private bool isDownloadingInstaller;

    private bool isDownLoadingAssetFile;
    private bool isAssetFileCreated;
    private int downloadingAssetFileCurrentCount;
    private int downloadingAssetFileMaxCount;

    //private List<JsonAssetManager> jsonFileAssetList = new List<JsonAssetManager>();

    //private const string UDI = "8809483601948";

    private void Awake()
    {
        versionText.text = $"v.{OptionSettings.GetInstance().versionName}";
    }

    private void Update()
    {
        if (isDownloadingInstaller)
        {
            //SetDescription($"{LanguageManager.Instance.GetLanguage("LOGO_DOWNLOAD_INSTALL_FILE")}({NetworkManagers.Instance.GetProgress() * 100:0}%)");
        }
        else if (isDownLoadingAssetFile)
        {
            //SetDescription($"{LanguageManager.Instance.GetLanguage("LOGO_DOWNLOAD_EXTRA_FILE")}({NetworkManagers.Instance.GetProgress() * 100:0}%)-{downloadingAssetFileCurrentCount}/{downloadingAssetFileMaxCount}");
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        SetDescription(string.Empty);
        StartCoroutine(AsyncStart());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetDescription(string message)
    {
        descriptionText.text = message;
    }

    private IEnumerator AsyncStart()
    {
        yield return new WaitForSeconds(1.0f);

        if (UnityEngine.Application.isEditor)
        {
        //    if (DebugManager.Instance.isDontSkipLogoViewInEditor)
        //        StartCoroutine(AsyncCheckInternetConnection());
        //    else
        //    {
        //        //ViewManager.Instance.logoView.Hide();
        //        //ViewManager.Instance.logInView.Show();
        //    }
        //}
        //else
        //{
        //    if (DebugManager.Instance.isSkipLogoViewInDevice)
        //    {
        //        //ViewManager.Instance.logoView.Hide();
        //        //ViewManager.Instance.logInView.Show();
        //    }
        //    else
        //        StartCoroutine(AsyncCheckInternetConnection());
        }
    }

    private IEnumerator AsyncToLoginView(string message)
    {
        SetDescription(message);
        yield return new WaitForSeconds(1.0f);
        Hide();
        //ViewManager.Instance.logInView.Show();
    }

    private IEnumerator AsyncCheckInternetConnection()
    {
        //SetDescription(LanguageMngr.Instance.GetLanguage("LOGO_CHECKING_CONNECTION"));
        //yield return new WaitForSeconds(1.0f);
        //if (Application.internetReachability == NetworkReachability.NotReachable || DebugManager.Instance.isConnectionNotReachable)
        //    yield return AsyncToLoginView(LanguageMngr.Instance.GetLanguage("LOGO_NOT_CONNECTED"));
        //else
        //    yield return AsyncRequestVersion();
        return null;
    }

    #region Check Version : ---------------------------------------------------
    //private IEnumerator AsyncRequestVersion()
    //{
    //    SetDescription(LanguageMngr.Instance.GetLanguage("LOGO_CHECKING_VERSION"));
    //    yield return new WaitForSeconds(1.0f);
    //    //NetworkManagers.Instance.RequestVersion(CallbackVersion);
    //}

    //private bool ExistOtherVersion()
    //{
    //    //return (SmartBodyCheckerSettings.GetInstance().versionCode != JsonManager.Instance.jsonClient.versionCode);
    //    return false;
    //}

    //public void CallbackVersion(string response, bool isSuccess)
    //{
    //    if (isSuccess)
    //    {
    //        //JsonManager.Instance.ParseVersionInfo(response);
    //        if (ExistOtherVersion())
    //        {
    //            if (ExistInstaller())
    //                StartCoroutine(AsyncInstallFile());
    //            else
    //                StartCoroutine(AsyncDownloadInstaller());
    //        }
    //        else
    //        {
    //            StartCoroutine(AsyncRequestAssetList());
    //        }
    //    }
    //    else
    //    {
    //        StartCoroutine(AsyncToLoginView(LanguageMngr.Instance.GetLanguage("LOGO_CHECK_CONNECTION")));
    //    }
    //}

    //private bool ExistInstaller()
    //{
    //    bool exist = false;
    //    string filePath = PathUtility.GetInstallerFile();
    //    if (File.Exists(filePath))
    //    {
    //        var fileStream = File.OpenRead(filePath);
    //        //if (HashManager.IsSameHash(fileStream, JsonManager.Instance.jsonClient.sha256Checksum, HashManager.HashType.SHA256))
    //        //    exist = true;
    //        fileStream.Close();
    //    }
    //    return exist;
    //}
    #endregion

    #region Download & Install Latest Version Program : -----------------------
    private IEnumerator AsyncDownloadInstaller()
    {
        //SetDescription($"{LanguageManager.Instance.GetLanguage("LOGO_DOWNLOAD_INSTALL_FILE")}(0%)");
        yield return new WaitForSeconds(1.0f);
        //isDownloadingInstaller = true;
        //NetworkManagers.Instance.DownloadInstaller(CallbackInstallerDownloaded, CallbackInstallerCreated);
    }

    private void CallbackInstallerDownloaded()
    {
        //isDownloadingInstaller = false;
        //SetDescription(LanguageMngr.Instance.GetLanguage("LOGO_CREATING_FILE"));
    }

    private void CallbackInstallerCreated(string response, bool isSuccess)
    {
        //if (isSuccess)
        //{
        //    StartCoroutine(AsyncInstallFile());
        //}
        //else
        //{
        //    StartCoroutine(AsyncToLoginView(LanguageMngr.Instance.GetLanguage("LOGO_CHECK_CONNECTION")));
        //}
    }

    private IEnumerator AsyncInstallFile()
    {
//        SetDescription(LanguageMngr.Instance.GetLanguage("LOGO_INSTALL_NEW_VERSION"));
//        yield return new WaitForSeconds(1.5f);

//        var path = PathUtility.GetInstallerFile();
//        if (File.Exists(path))
//            System.Diagnostics.Process.Start(path);
//        else
//            Debug.LogError("다운로드 받은 파일이 없습니다.");

//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//        Application.Quit();
//#endif
        return  null;
    }
    #endregion

    #region check dll files
    private IEnumerator AsyncRequestAssetList()
    {
        //SetDescription(LanguageMngr.Instance.GetLanguage("LOGO_CHECKING_EXTRA_FILE"));
        yield return new WaitForSeconds(1.0f);
        //NetworkManagers.Instance.RequestAssetList(CallbackRequestAssetList);
    }

    private void CallbackRequestAssetList(string response, bool isSuccess)
    {
        if (isSuccess)
        {
            //JsonManager.Instance.ParseAssetList(response);
            StartCoroutine(AsyncCheckAndDownloadAsset());
        }
        else
        {
            //StartCoroutine(AsyncToLoginView(LanguageMngr.Instance.GetLanguage("LOGO_CHECK_CONNECTION")));
        }
    }

    private IEnumerator AsyncCheckAndDownloadAsset()
    {
        yield return AsyncAddJsonAssetFileList();
        yield return AsyncDownloadAndCreateAssetFiles();
        //StartCoroutine(AsyncToLoginView(LanguageMngr.Instance.GetLanguage("LOGO_GO_LOGIN_VIEW")));
    }

    private IEnumerator AsyncAddJsonAssetFileList()
    {
        // check md5 hash
        //jsonFileAssetList.Clear();
        //foreach (var jsonFileAsset in JsonManager.Instance.jsonAssetManagerList)
        //{
        //    SetDescription($"{LanguageManager.Instance.GetLanguage("LOGO_CHECKING_EXTRA_FILE")}({count}/{JsonManager.Instance.jsonAssetManagerList.Count})");
        //    if (PathUtility.IsProgramDllFolder(jsonFileAsset.fileDirectory))
        //    {
        //        string filePath = Path.Combine(PathUtility.GetProgramDestinationFolder(jsonFileAsset.fileDirectory), jsonFileAsset.fileName);
        //        if (File.Exists(filePath))
        //        {
        //            var fileStream = File.OpenRead(filePath);
        //            if (HashManager.IsSameHash(fileStream, jsonFileAsset.md5, HashManager.HashType.MD5) == false)
        //                jsonFileAssetList.Add(jsonFileAsset);
        //            else
        //            {
        //                Debug.Log($"exist : {jsonFileAsset.fileDirectory}, {jsonFileAsset.fileName}");
        //            }
        //            fileStream.Close();
        //        }
        //        else
        //        {
        //            jsonFileAssetList.Add(jsonFileAsset);
        //        }
        //    }
        //    count++;
        //    yield return new WaitForEndOfFrame();
        //    yield return new WaitForSeconds(0.05f);
        //}
        yield return null;
    }

    private IEnumerator AsyncDownloadAndCreateAssetFiles()
    {
        //if (jsonFileAssetList.Count > 0)
        //{
        //    downloadingAssetFileMaxCount = jsonFileAssetList.Count;
        //    downloadingAssetFileCurrentCount = 1;
        //    SetDescription($"{LanguageManager.Instance.GetLanguage("LOGO_DOWNLOAD_EXTRA_FILE")}({0}%)-{downloadingAssetFileCurrentCount}/{downloadingAssetFileMaxCount}");

        //    PathUtility.CreateFolder(PathUtility.GetProgramPluginsFolder());

        //    // 다운로드 & 파일복사( overwrite )
        //    foreach (var jsonFileAsset in jsonFileAssetList)
        //    {
        //        isDownLoadingAssetFile = true;
        //        isAssetFileCreated = false;
        //        string destinationFilePath = Path.Combine(PathUtility.GetProgramDestinationFolder(jsonFileAsset.fileDirectory), jsonFileAsset.fileName);

        //        NetworkManagers.Instance.DownloadAssetFile(jsonFileAsset.downloadUrl, CallbackAssetFileDownloaded, CallbackAssetFileCreated, destinationFilePath);

        //        yield return new WaitUntil(() => isAssetFileCreated); // 참이 될 때까지, 기다리기
        //        downloadingAssetFileCurrentCount++;
        //    }
        //}

        yield return null;
    }

    private void CallbackAssetFileDownloaded()
    {
        isDownLoadingAssetFile = false;
    }

    private void CallbackAssetFileCreated(string response, bool isSuccess)
    {
        if (isSuccess)
        {
            isAssetFileCreated = true;
        }
        else
        {
            //StartCoroutine(AsyncToLoginView(LanguageMngr.Instance.GetLanguage("LOGO_CHECK_CONNECTION")));
        }
    }
    #endregion

    public void OnClickGoLogin()
    {
        Hide();
        //ViewManager.Instance.logInView.Show();
    }
}