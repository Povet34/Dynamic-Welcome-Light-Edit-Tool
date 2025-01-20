using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[AddComponentMenu("UI/Custom/UIImage")]
public class UIImage : Image 
{
    public void DownloadSpirte(string imagePath, Action func = null, bool isLocalSave = false)
    {
        StartCoroutine(DownloadTexture(imagePath, func, isLocalSave));
    }

    private IEnumerator DownloadTexture(string imagePath, Action func, bool isLocalSave)
    {
        if (!string.IsNullOrEmpty(imagePath))
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imagePath);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite createSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0, 0));
                this.sprite = createSprite;
                func?.Invoke();

                if (isLocalSave)
                {
                    string fileName = Path.GetFileName(imagePath);
                    string localPath = GetLocalFilePath(fileName);
                    byte[] bytes = localPath.ToLower().Contains(".jpg") ? texture.EncodeToJPG() : texture.EncodeToPNG();

                    using (FileStream SourceStream = File.Open(localPath, FileMode.Create))
                        SourceStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                Debug.Log(uwr.error);
            }
        }
    }

    private string GetLocalFilePath(string fileName)
    {
        string path = UnityEngine.Application.temporaryCachePath + "/_Image";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path + "/" + fileName;
    }

    public void SetSprite(string path, bool isSaveSprite = true)
    { 
        //if (string.IsNullOrEmpty(path))
        //{     
        //    return;
        //}

        //string fileName = Path.GetFileName(path);
        //Sprite sp = MyDownloader.Instance.GetLoadedSprite(fileName); 
        //if (null != sp)
        //{
        //    this.sprite = sp;
        //    color = new Color(color.r, color.g, color.b, originalAlpha);
        //}
        //else //if (this.gameObject.activeInHierarchy)
        //{
        //    if (null != imageDownloaded)
        //        imageDownloaded.Cancel();

        //    color = new Color(color.r, color.g, color.b, 0);
        //    imageDownloaded = new Definitions.Callback<UIImageDownloader.DownloadResult>(OnImageDownloaded);
        //    UIImageDownloader.Instance.AddDownloadQueue(this, path, isSaveSprite, imageDownloaded.Call); 
        //};
        
        //
        // if (this.downloadCoroutine != null)
        // {
        //     StopCoroutine(this.downloadCoroutine);
        //     this.downloadCoroutine = null;
        // }
        //
        // string fileName = Path.GetFileName(path);
        // Sprite sp = MyDownloader.Instance.GetLoadedSprite(fileName);
        // if (null != sp)
        // {
        //     this.sprite = sp;
        //     color = new Color(color.r, color.g, color.b, originalAlpha);
        // }
        // else if (this.gameObject.activeInHierarchy)
        // {
        //     color = new Color(color.r, color.g, color.b, 0);
        //     this.downloadCoroutine = StartCoroutine(DownloadTexture(path, isSaveSprite));
        // }
    }

//    private IEnumerator DownloadTexture(string path, bool isSaveSprite)
//    {
//        string localPath;
//        string filePath = path;
//        string fileName = Path.GetFileName(path);

//        if (!path.Contains("http"))
//            localPath = path;
//        else
//            localPath = GetLocalFilePath(fileName);

//        bool isLocal = false;
//        if (File.Exists(localPath))
//        {
//            isLocal = true;
//#if UNITY_EDITOR
//            filePath = "file:///" + localPath;
//#elif UNITY_ANDROID
//            filePath = "file://" + localPath;
//#elif UNITY_IPHONE
//            filePath = "file://" + localPath;
//#endif
//        }

//        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(filePath);
//        yield return uwr.SendWebRequest();

//        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
//            Debug.LogWarning(uwr.error);
//        else
//        {
//            Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
//            Sprite downloadedSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
//            this.sprite = downloadedSprite;
//            StartCoroutine(FadeInAlpha());

//            if (!isLocal)
//            {
//                if (isSaveSprite)
//                {
//                    if (isSaveSprite)
//                    {
//                        string _path = GetLocalFilePath(fileName, true);
//                        byte[] bytes = _path.ToLower().Contains(".jpg") ? texture.EncodeToJPG() : texture.EncodeToPNG();

//                        using (FileStream SourceStream = File.Open(_path, FileMode.Create))
//                            SourceStream.Write(bytes, 0, bytes.Length);
//                    }
//                }
//            }

//            MyDownloader.Instance.SetLoadedSprite(fileName, downloadedSprite);
//        }
//    }

    //private string GetLocalFilePath(string fileName, bool isCreateFolder = false)
    //{
    //    string path = Application.temporaryCachePath + "/_Image";
    //    if (isCreateFolder && !Directory.Exists(path))
    //        Directory.CreateDirectory(path);
    //    return path + "/" + fileName;
    //}
}