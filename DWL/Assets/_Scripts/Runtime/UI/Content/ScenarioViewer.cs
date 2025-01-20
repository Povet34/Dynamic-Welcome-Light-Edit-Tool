using Common.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.Slider;

public class ScenarioViewer : MonoBehaviour
{
    public struct ScenarioInfo
    {
        public int frameCount;
        public float videoLength;
        public Vector2Int resolution;
        public float interval;
    }

    public struct ViewerInfo
    {
        public bool isShoChannelIndex;

        public ViewerInfo(bool isShoChannelIndex)
        {
            this.isShoChannelIndex = isShoChannelIndex;
        }
    }

    private List<ScenarioChannel> scenarioChannels = new List<ScenarioChannel>();
    private Dictionary<ScenarioChannel, List<RecordValue>> originBindMap;
    private Dictionary<ScenarioChannel, List<RecordValue>> modifiedBindMap;

    private VideoRatioCalculator scenarioRatioCalculator;

    [SerializeField] private GameObject sliderWrapperObj;
    [SerializeField] private UISlider videoTimeSlider;
    [SerializeField] private RectTransform videoPanelRawImage;

    [SerializeField] private UITextMeshPro currentTimerText;
    [SerializeField] private UITextMeshPro videoLengthText;

    [SerializeField] private RectTransform GoPanelAreaTr;
    private SliderEvent silderEvent = new SliderEvent();
    private RectTransform scenarioPanelRt;
    private Coroutine playRoutine;

    private bool isDragging;
    private float playSpeed = 1f;
    private int currnetFrame;
    private bool isPlaying;
    private bool isShowOriginData;

    [Header("Prefabs")]
    [SerializeField] private GameObject viewPixelChannelPrefab;

    ScenarioInfo scenarioInfo;
    ViewerInfo viewerInfo;

    private void Awake()
    {
        silderEvent.AddListener(HandleSliderChange);
        videoTimeSlider.SetCallback(silderEvent, OnDragStart, OnDragEnd);

        scenarioPanelRt = videoPanelRawImage.GetComponent<RectTransform>();
    }

    public void Show(bool isShow, ViewerInfo info)
    {
        RemoveAllViewChannels();
        gameObject.SetActive(isShow);
        sliderWrapperObj.SetActive(false);

        viewerInfo = info;
    }

    public void Play(float playSpeed)
    {
        if (null == originBindMap)
        {
            NDebug.LogError("플레이할거 없음.");
            return;
        }

        if (!isShowOriginData)
        {
            if (modifiedBindMap.Count == 0)
                Provider.Instance.ShowErrorPopup("수정된 데이터 없음");
        }

        this.playSpeed = playSpeed;

        if (null != playRoutine)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlayFrame());
    }

    private void HandleSliderChange(float value)
    {
        if (scenarioInfo.frameCount > 0 && isDragging)
        {
            currnetFrame = (int)(value * scenarioInfo.frameCount);
            SetFrame((int)currnetFrame);
        }
    }

    private void UpdateSliderValue()
    {
        if (!isDragging && isPlaying)
            videoTimeSlider.value = (float)currnetFrame / (float)scenarioInfo.frameCount;
    }

    public void SetFrame(int frame)
    {
        currnetFrame = frame;

        if (isShowOriginData)
        {
            foreach (var ch in originBindMap)
                ch.Key.SetColor((ch.Value[frame].Brightness_Int / 255f) * Color.white);
        }
        else
        {
            foreach (var ch in modifiedBindMap)
                ch.Key.SetColor((ch.Value[frame].Brightness_Int / 255f) * Color.white);
        }

        UpdateSliderValue();
    }

    public void PauseScenario()
    {
        isPlaying = false;
    }

    #region Verification : ------------------------------------------------

    private IEnumerator PlayFrame()
    {
        //Init
        foreach (var ch in scenarioChannels)
            ch.SetColor(Color.black);

        isPlaying = true;

        if(isRecord)
            StartRecord();

        //loop
        int count = 1;
        var waitForSec = new WaitForSeconds(scenarioInfo.interval * (1f / playSpeed));

        while (scenarioInfo.frameCount > count)
        {
            if (!isPlaying)
                yield return new WaitUntil(() => isPlaying);

            SetFrame(count);
            UpdateCurrentSilderTimer();

            yield return waitForSec;
            count++;
        }

        if (isRecord)
            StopRecord();
    }

    public async void LoadVerifiedExcel()
    {
        var filePath = Provider.Instance.GetExcelLoader().OpenFilePath();
        bool success = await ReadExcel(filePath);
        if (success)
        {
            sliderWrapperObj.SetActive(true);
            videoLengthText.text = TimeUtil.GetTimeString_SSMSMS(scenarioInfo.videoLength);
        }
    }

    private async Task<bool> ReadExcel(string filePath)
    {
        scenarioRatioCalculator = null;
        scenarioInfo = default(ScenarioInfo);

        if (!string.IsNullOrEmpty(filePath))
        {
            var readData = await Provider.Instance.GetExcelReader().ReadExcelDataAsync(filePath);
            if (null != readData)
            {
                originBindMap = new Dictionary<ScenarioChannel, List<RecordValue>>();
                modifiedBindMap = new Dictionary<ScenarioChannel, List<RecordValue>>();

                scenarioInfo = readData.scenarioInfo;

                scenarioRatioCalculator =
                    new VideoRatioCalculator(
                        scenarioInfo.resolution.x,
                        scenarioInfo.resolution.y,
                        scenarioPanelRt,
                        Vector2.zero);

                scenarioInfo.interval = scenarioInfo.videoLength / readData.originBrightnessMap.First().Value.Count;
            }
            else
            {
                NDebug.LogError("Can't read excel");
            }

            RemoveAllViewChannels();

            bool hasModify = null != readData.modifiedBrightnessMap;

            foreach (var element in readData.originBrightnessMap)
            {
                var ch = CreateViewPixelChannel(element.Key.pos, element.Key.index);
                originBindMap[ch] = element.Value;

                if (hasModify)
                    modifiedBindMap[ch] = readData.modifiedBrightnessMap[element.Key];
            }
        }

        return null != scenarioRatioCalculator;
    }

    public void RemoveAllViewChannels()
    {
        if (null != scenarioChannels)
        {
            foreach (var ch in scenarioChannels)
                Destroy(ch.gameObject);

            scenarioChannels.Clear();
        }
    }

    private ScenarioChannel CreateViewPixelChannel(Vector2 initPosition, int index)
    {
        if (viewPixelChannelPrefab)
        {
            var go = Instantiate(viewPixelChannelPrefab, scenarioPanelRt);
            if (go)
            {
                var ch = go.GetComponent<ScenarioChannel>();
                if (null != ch)
                {
                    ch.Init(scenarioRatioCalculator.GetPositionFromVideoCoordinates(initPosition), index, viewerInfo.isShoChannelIndex);
                    scenarioChannels.Add(ch);

                    return ch;
                }
            }
        }

        return null;
    }


    private void OnDragStart(float value)
    {
        isDragging = true;
        OnToggleVideoPlayPause(isDragging, string.Empty);
    }

    private void OnDragEnd(float value)
    {
        isDragging = false;
    }


    private void OnToggleVideoPlayPause(bool isOn, string toggleData)
    {
        if (isOn)
            PauseScenario();
    }

    void UpdateCurrentSilderTimer()
    {
        float ratio = (float)currnetFrame / (float)scenarioInfo.frameCount;
        currentTimerText.text = TimeUtil.GetTimeString_SSMSMS(scenarioInfo.videoLength * ratio);
    }

    #endregion

    #region Side Settings Receive : --------------------------------------

    public void ChangeChannelViewState(bool isShow)
    {
        viewerInfo.isShoChannelIndex = isShow;

        foreach (var ch in scenarioChannels)
            ch.ShowIndex(isShow);
    }

    public void CreateVideoState(bool isRecord)
    {
        this.isRecord = isRecord;
    }

    public void SwitchBrightnessShowTarget(bool isShowOriginData)
    {
        this.isShowOriginData = isShowOriginData;
    }

    #endregion

    #region Video Record : -----------------------------------------------

    private bool isRecord;

    private readonly Vector2 RecordViewerSize = new Vector2(1760, 990);
    private readonly Vector2 RecordViewerPos = new Vector2(0, 0);

    private readonly Vector2 OriginViewerSize = new Vector2(1450, 800);
    private readonly Vector2 OriginViewerPos = new Vector2(155, 13);

    private void StartRecord()
    {
        GoPanelAreaTr.SetAsLastSibling();

        GoPanelAreaTr.sizeDelta = RecordViewerSize;
        GoPanelAreaTr.anchoredPosition = RecordViewerPos;

        sliderWrapperObj.SetActive(false);
        VideoRecordingManager.Instance.StartScreenRecording();
    }

    private void StopRecord()
    {
        VideoRecordingManager.Instance.StopScreenRecording();

        GoPanelAreaTr.sizeDelta = OriginViewerSize;
        GoPanelAreaTr.anchoredPosition = OriginViewerPos;

        sliderWrapperObj.SetActive(true);
        GoPanelAreaTr.SetAsFirstSibling();
    }

    #endregion
}
