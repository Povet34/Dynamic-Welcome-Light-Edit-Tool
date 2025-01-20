using ChannelAnalyzers;

public class Provider : Singleton<Provider>
{
    #region IPixelDataRecorder

    public IPixelDataRecorder GetBrightnessDataRecorder()
    {
        return new PixelDataRecorderImpl_CulmWithFrameByIndex();
    }

    #endregion

    #region IVideoExtractor

    public IVideoExtractor GetVideoExtractor()
    {
        return new VideoExtractorImpl_ByCachedRenderTexture();
    }

    #endregion

    #region IExcelCreator

    public IExcelEditor GetExcelEditor()
    {
        return new ExcelEditorImpl_RowChannelDatasOnlyText();
    }

    #endregion

    #region IExcelReader

    public IExcelReader GetExcelReader()
    {
        return new ExcelReaderImpl();
    }

    #endregion

    #region ILocalFileLoader

    private ILocalFileLoader videoLoader;

    public ILocalFileLoader GetVideoLoader()
    {
        if(null == videoLoader)
            videoLoader = new LocalFileLoaderImpl_Video();

        return videoLoader;
    }

    private ILocalFileLoader excelLoader;

    public ILocalFileLoader GetExcelLoader()
    {
        if (null == excelLoader)
            excelLoader = new LocalFileLoaderImpl_Excel();

       return excelLoader;
    }

    #endregion

    #region IFolderPathLocater

    public IFolderPathLocater GetFolderPathLocater()
    {
#if UNITY_EDITOR
        return new FolderPathLocaterImpl_Editor();
#elif UNITY_STANDALONE
        return new FolderPathLocaterImpl_Exe();
#endif
    }

    #endregion

    #region IBrightingDetector

    public IBrightingDetector GetBrightingDetector()
    {
        return new BrightingDetectorImpl_BrightIntervalThreshold();
    }

    #endregion

    #region IDuty

    IDuty duty;

    public IDuty GetDuty(bool newInstance = false)
    {
        if (null == duty || newInstance)
            duty = new DutyImpl();

        return duty;
    }

    #endregion

    #region IGraphBuilder
    
    public IGraphBuilder GetGraphBuilder() 
    {
        return FindObjectOfType<GraphBuilderImpl_OnlyView>();
    }

    #endregion

    #region Error Popup

    ToastPopup toastPopup;
    ToastPopup GetErrorPopup()
    {
        if (!toastPopup)
            toastPopup = FindObjectOfType<ToastPopup>();

        return toastPopup;
    }

    public void ShowErrorPopup(string msg)
    {
        GetErrorPopup().ShowPopup(new PopupSetting(msg), null);
    }

    #endregion

    #region Loading Popup

    LoadingPopup loadingPopup;
    public LoadingPopup GetLoadingPopup()
    {
        if (!loadingPopup)
            loadingPopup = FindObjectOfType<LoadingPopup>();

        return loadingPopup;
    }

    public void ShowLoadingPopup(bool isShow)
    {
        GetLoadingPopup().Show(isShow);
    }

    #endregion

    #region TEXTDB

    TextDB textDB;

    public string GetText(string key)
    {
        if (null == textDB)
            textDB = new TextDB();

        return textDB.GetText(key);
    }


    #endregion
}
