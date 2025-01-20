using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using static UnityEngine.UI.Slider;

namespace Common.UI
{
    public class VideoViewer : MonoBehaviour,
        IDragHandler,
        IScrollHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        public struct ViewerInfo
        {
            public int viewerIndex;
            public RectTransform parentTr;
            public Rect ViewRect => RectTransformExtensions.GetSubPanelArea(parentTr, (RectTransformExtensions.RegionType)viewerIndex);

            public ViewerInfo(int idx, RectTransform rt)
            {
                viewerIndex = idx;
                parentTr = rt;
            }
        }

        public struct VideoInfo
        {
            public string filePath;
            public string fileName;
            public float videoLength;
            public Vector2Int resolution;
        }

        [Header("UI")]
        [SerializeField] UIRawImage videoPanelRawImage;
        [SerializeField] UITextMeshPro currentTimerText;
        [SerializeField] UITextMeshPro videoLengthText;
        [SerializeField] UISlider videoTimeSlider;

        [Header("Others")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject CreateChannelContextMenuPrefab;

        private List<IPixelChannel> allChannels = new List<IPixelChannel>();
        private Coroutine extractRoutine;

        private Texture2D videoFrameTexture;
        private RectTransform rawImageRt;

        private SliderEvent silderEvent = new SliderEvent();
        private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private VideoRatioCalculator videoRatioCalculator;

        private IAreaSelection areaSelection;
        private IVideoExtractor videoExtractor;
        private IPixelDataRecorder recorder;

        private ViewerInfo viewerInfo;
        private VideoInfo videoInfo;

        private ulong currentFrame;
        private ulong preFrame; //바로 이전의 currentFrame을 저장한다.
        private ulong totalFrames;

        private Action<ePixelChannelType, Vector2> createChannelCallback;

        private int currentScale = 1;
        public int CurrentViewImgSacle => currentScale;

        private bool isKeyDown_LeftControl;
        private bool isDragging;
        private bool isVideoEnd;
        private bool isRecordingVideo;

        private float frameRate;
        public float FrameRate => frameRate;

        private float playSpeed = 1f;
        public Transform GetRawImageTr() => rawImageRt;

        protected void Awake()
        {
            RegisterEventHandler();
            rawImageRt = videoPanelRawImage.GetComponent<RectTransform>();
        }

        #region UI Event : ----------------------------------------------------

        #region Register Event Handler : ----------------------------
        private void RegisterEventHandler()
        {
            silderEvent.AddListener(HandleSliderChange);
            videoTimeSlider.SetCallback(silderEvent, OnDragStart, OnDragEnd);
        }
        #endregion

        private void OnToggleVideoPlayPause(bool isOn, string toggleData)
        {
            if (!videoPlayer.isPrepared)
            {
                NDebug.Log("VideoPlayer is not prepared.");
                return;
            }

            if (isOn)
                PauseVideo();
        }

        private void HandleSliderChange(float value)
        {
            if (videoPlayer.frameCount > 0 && isDragging)
            {
                currentFrame = (ulong)(value * totalFrames);
                videoPlayer.frame = (long)currentFrame;

                UpdateCurrentSilderTimer();

                if (preFrame != currentFrame)
                {
                    UpdateVideoTexture();
                    preFrame = currentFrame;
                }
            }
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

        #endregion

        public void Init(
            ViewerInfo viewerInfo, 
            ref List<IPixelChannel> allChannels, 
            Action<IPixelChannel> addCurChannelList,
            Action<ePixelChannelType, Vector2> createChannelCallback,
            Action clearCurChannelList)
        {
            videoExtractor = Provider.Instance.GetVideoExtractor();

            RefreshViewerSize(viewerInfo);
            this.allChannels = allChannels;
            this.createChannelCallback = createChannelCallback;
            areaSelection = AreaSelectionImpl_PixelChannel.Create(transform, allChannels, addCurChannelList, clearCurChannelList);
        }

        public void ReplayFirstFrame()
        {
            SetFrame();

            if (isVideoEnd)
                PrepareVideo(videoInfo.filePath);
            else
            {
                ResumeVideo();
            }
        }

        public void Replay(float playSpeed)
        {
            this.playSpeed = playSpeed;

            recorder = Provider.Instance.GetBrightnessDataRecorder();

            ResetTransform();
            ReplayFirstFrame();
        }

        public void PrepareVideo(VideoInfo videoInfo)
        {
            this.videoInfo = videoInfo;
            PrepareVideo(videoInfo.filePath);
        }

        #region Video & Frame  : ----------------------------------------------

        public void RefreshViewerSize(ViewerInfo viewerInfo)
        {
            this.viewerInfo = viewerInfo;

            var rt = GetComponent<RectTransform>();
            if (rt)
            {
                rt.SetSubPanel(viewerInfo.ViewRect);
                var name = rt.name;
                gameObject.name = $"{nameof(VideoViewer)}_{(RectTransformExtensions.RegionType)viewerInfo.viewerIndex}";
            }
        }

        public void SetFrame(int frame = 0)
        {
            currentFrame = (uint)frame;
            videoPlayer.frame = (uint)frame;
        }

        private void PrepareVideo(string path)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;

            if (videoPlayer.isPlaying)
                videoPlayer.Stop();

            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.APIOnly;

            videoPlayer.url = path;
            videoPlayer.Prepare();
        }

        private void UpdateSliderValue()
        {
            if (!isDragging && videoPlayer.isPlaying)
                videoTimeSlider.value = (float)currentFrame / (float)totalFrames;
            else if(isVideoEnd) //이미 비디오가 끝나있으면, 슬라이드는 마지막까지 민다. 근데 퍼즈가 될 경우는 참작해야함.
                videoTimeSlider.value = (float)currentFrame / (float)totalFrames;
        }

        private void UpdateVideoTexture()
        {
            videoFrameTexture = videoExtractor.ExtractFrame(videoPlayer);
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached += EndReached;

            SetFrame();
            HandleSliderChange(0);

            PlayVideo(true);
        }

        private void PlayVideo(bool isFirstLoad = false)
        {
            if (videoPlayer == null)
            {
                Debug.LogError("VideoPlayer is not assigned.");
                return;
            }


            //채널 사이즈를 1로 변경한다.
            foreach (var ch in allChannels)
                ch.ChangeSize(1);

            frameRate = 0;

            videoInfo.resolution = new Vector2Int((int)videoPlayer.width, (int)videoPlayer.height);
            videoInfo.videoLength = (float)videoPlayer.length;
            videoLengthText.text = TimeUtil.GetTimeString_SSMSMS(videoInfo.videoLength);

            if (null != extractRoutine)
                StopCoroutine(extractRoutine);
            extractRoutine = StartCoroutine(ExtractFrames());

            if (isFirstLoad)
                StartCoroutine(LoadedPause());

            videoPlayer.Play();
        }

        public void PauseVideo()
        {
            if(!videoPlayer.isPaused)
                videoPlayer.Pause();
        }

        private void ResumeVideo()
        {
            PlayVideo();
        }

        private IEnumerator LoadedPause()
        {
            yield return new WaitForSeconds(0.04f);
            videoPlayer.frame = 3;
            UpdateVideoTexture();

            PauseVideo();

            while (!videoPlayer.isPlaying)
            {
                yield return null;
            }

            ReplayFirstFrame();
        }

        private IEnumerator ExtractFrames()
        {
            while (!videoPlayer.isPlaying || null == videoPlayer.url)
            {
                yield return waitForEndOfFrame;
            }

            ResetTransform();

            isVideoEnd = false;
            videoPlayer.playbackSpeed = playSpeed;

            frameRate = videoPlayer.frameRate + Definitions.GetProperRateOffset(videoInfo.resolution.x);
            totalFrames = (ulong)(videoPlayer.frameCount * (1f / playSpeed));

            float frameDuration = 1f / frameRate;
            var waitForSec = new WaitForSeconds(frameDuration);

            videoRatioCalculator = new VideoRatioCalculator(videoPlayer, rawImageRt, Vector2.zero);

            while (currentFrame <= totalFrames)
            {
                if (!videoPlayer.isPlaying && !isVideoEnd)
                    yield return new WaitUntil(() => videoPlayer.isPlaying);

                currentFrame++;

                if(_canRecord(recordStart))
                {
                    VideoRecordingManager.Instance.StartTextureRecording();
                    yield return null;
                }
                else if(_canRecord(recordEnd))
                {
                    VideoRecordingManager.Instance.StopTextrueRecording();
                }

                UpdateVideoTexture();
                UpdateCurrentSilderTimer();
                ExtractFrame();

                yield return waitForSec;
            }

            //안꺼졌을 경우를 상정해서 꺼주자.
            VideoRecordingManager.Instance.StopTextrueRecording();
            ResetRecordValues();

            isVideoEnd = true;

            bool _canRecord(uint recordTime)
            {
                return recordTime != uint.MaxValue && recordTime == currentFrame;
            }
        }

        private void EndReached(VideoPlayer vp)
        {
            isVideoEnd = true;
        }

        private void ExtractFrame()
        {
            if (!videoFrameTexture)
                return;

            videoPanelRawImage.texture = videoFrameTexture;

            UpdateSliderValue();
            GeneratePixelChannels((int)currentFrame);

            RecordPixelDatPerFrame();
        }

        #endregion

        #region Pixel Control : -----------------------------------------------

        public void OnCreatedChannel()
        {
            GeneratePixelChannels((int)currentFrame);
        }

        private void GeneratePixelChannels(int frameOrder)
        {
            if (null != allChannels)
            {
                for (int ii = 0; ii < allChannels.Count; ii++)
                {
                    var pixel = allChannels[ii];

                    if (null != pixel)
                    {
                        List<Vector2> positions = new List<Vector2>();
                        var handles = pixel.GetDragHandles();
                        for (int oo = 0; oo < handles.Count; oo++)
                        {
                            var pos = videoRatioCalculator.GetPositionInVideoCoordinates(handles[oo]);
                            positions.Add(pos);
                        }
                        pixel.SetTextrue(videoFrameTexture, frameOrder, positions);
                    }
                }
            }
        }

        /// <summary>
        /// 비디오 해상도의 좌표를 ui로 우겨넣기위해 변형함.
        /// </summary>
        /// <param name="videoCoord"></param>
        /// <returns></returns>
        public Vector2 GetPositionFromVideoCoordinates(Vector2 videoCoord)
        {
            return videoRatioCalculator.GetPositionFromVideoCoordinates(videoCoord);
        }

        /// <summary>
        /// 비디오 Panel의 내부의 ui target 위치 (일그러진 videoPanel과 video의 해상도를 대응하는 pos를 찾을 때 사용한다.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2Int GetPositionInVideoCoordinatesByPosition(Vector2Int pos)
        {
            return videoRatioCalculator.GetPositionInVideoCoordinatesByPosition(pos);
        }

        private void UpdateChannelInfos()
        {
            if (videoPlayer.isPlaying)
                return;

            if (null != allChannels)
            {
                foreach (var pixel in allChannels)
                {
                    if (null != pixel)
                    {
                        var posInt = Vector2Int.zero;
                        if (null != pixel.GetDragHandles() && pixel.GetDragHandles().Count > 0)
                        {
                            var pos = videoRatioCalculator.GetPositionInVideoCoordinates(pixel.GetDragHandles().First());
                            posInt = new Vector2Int((int)pos.x, (int)pos.y);
                        }
                        pixel.UpdateChannelInfo(pixel.Index, posInt);
                    }
                }
            }
        }

        private void CheckKeyClicking()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isKeyDown_LeftControl = true;
                areaSelection.Cancel();
            }
            else if (Input.GetKeyUp(KeyCode.LeftControl))
                isKeyDown_LeftControl = false;
        }

        #endregion

        #region Data Recording : ----------------------------------------------

        private void RecordPixelDatPerFrame()
        {
            if (null != allChannels)
            {
                foreach (var pixel in allChannels)
                {
                    var pixelDataList = pixel.GetPixelDatas();

                    for (int i = 0; i < pixelDataList.Count; i++)
                    {
                        var data = pixelDataList[i];
                        var key = new RecordKey(pixel.Index, data.pos);
                        var value = new RecordValue(data.color);
                        recorder?.AddData(key, value);
                    }
                }
            }
        }

        public void SaveDataToCSV(float gradualThreshold, float radicalThreshold, int minimumFadeCount, float extendRatio, int bit, bool isExtractColor, bool isExtractBrightness, bool isSaveBrightnessLinear)
        {
            RecordFileInfo info = new RecordFileInfo();
            info.name = videoInfo.fileName;
            info.videoLength = videoInfo.videoLength;
            info.resolution = videoInfo.resolution;
            info.radicalThreshold = radicalThreshold;
            info.gradualThreshold = gradualThreshold;
            info.minimumFadeCount = minimumFadeCount;
            info.extendRatio = extendRatio;
            info.bit = bit;
            info.videoLength = videoInfo.videoLength;
            info.isExtractColor = isExtractColor;
            info.isExtractBrightness = isExtractBrightness;
            info.isSaveBrightnessLinear = isSaveBrightnessLinear;

            recorder?.CreateRecordFile(info);
        }

        #endregion

        #region Zoom : --------------------------------------------------------

        const float ZOOM_SPEED = 1f; // 확대/축소 속도
        const float MIN_ZOOM = 1f;   // 최소 확대 값
        const float MAX_ZOOM = 5f;  // 최대 확대 값

        const float MAX_CHANNEL_SIZE = 10F;
        const float MIN_CHANNEL_SIZE = 0.6F;

        Vector2 prePos = Vector2.zero;
        float scalingRatio = 1f;

        public void OnDrag(PointerEventData eventData)
        {
            if (!isKeyDown_LeftControl || videoPlayer.isPlaying)
                return;

            PanVideoPanel(eventData.delta);
            ClampAndReposition();
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (!isKeyDown_LeftControl || videoPlayer.isPlaying)
                return;

            var viewerRect = GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewerRect, eventData.position, eventData.pressEventCamera, out prePos);

            float scroll = eventData.scrollDelta.y;
            float currentScale = rawImageRt.localScale.x;
            float newScale = Mathf.Clamp(currentScale + scroll, MIN_ZOOM, MAX_ZOOM);

            rawImageRt.localScale = new Vector3(newScale, newScale, 1f);

            Vector2 newLocalMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRt, eventData.position, eventData.pressEventCamera, out newLocalMousePos);

            if (0 < scroll && newScale != currentScale)
            {
                Vector2 position = rawImageRt.anchoredPosition;
                position -= (prePos - newLocalMousePos) * rawImageRt.localScale.x;
                rawImageRt.anchoredPosition = position;
                prePos = position;
            }

            ClampAndReposition();

            foreach (var ch in allChannels)
            {
                ch.ChangeSize(CurrentViewImgSacle);
            }
        }   

        private void PanVideoPanel(Vector2 delta)
        {
            if (videoPlayer.isPlaying) return;

            rawImageRt.anchoredPosition += delta;
            ClampAndReposition();
        }

        private void ClampAndReposition()
        {
            float scale = rawImageRt.localScale.x;
            currentScale = Mathf.RoundToInt(Mathf.Clamp(scale, MIN_ZOOM, MAX_ZOOM));

            float maxX = (rawImageRt.rect.width * scale - rawImageRt.rect.width) / 2;
            float maxY = (rawImageRt.rect.height * scale - rawImageRt.rect.height) / 2;

            Vector2 position = rawImageRt.anchoredPosition;
            position = new Vector2(Mathf.Clamp(position.x, -maxX, maxX), Mathf.Clamp(position.y, -maxY, maxY));

            rawImageRt.anchoredPosition = position;
        }
        private void ResetTransform()
        {
            rawImageRt.localScale = Vector3.one;
            rawImageRt.anchoredPosition = Vector2.zero;
            ClampAndReposition();
        }

        #endregion

        #region Create Channel : ----------------------------------------------

        CreateChannelContextMenu contextMenu;

        private void CreateChannelByContextMenu()
        {
            var viewerRt = GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewerRt, Input.mousePosition, null, out localPoint);

            if (!contextMenu) 
            {
                var go = Instantiate(CreateChannelContextMenuPrefab, viewerRt);
                contextMenu = go.GetComponent<CreateChannelContextMenu>();
                contextMenu.Init(createChannelCallback, new RectResizeHelper(viewerRt, rawImageRt));
            }

            contextMenu.ReplaceAndShow(localPoint);
        }

        #endregion

        #region Create Modify Video : -----------------------------------------

        uint recordStart;
        uint recordEnd;

        public void CreateModifiedVideo(int start, int end)
        {
            var frameCount = videoPlayer.frameCount;
            var videoLengh = videoPlayer.length;

            int ms = Mathf.RoundToInt((float)(videoLengh * 1000)); //밀리세컨드로 확장
            
            var startRatio = Mathf.InverseLerp(0, ms, start);
            var endRatio = Mathf.InverseLerp(0, ms, end);

            var startFrame = Mathf.Lerp(0, frameCount, startRatio);
            var endFrame = Mathf.Lerp(0, frameCount, endRatio);

            recordStart = (uint)Mathf.RoundToInt(startFrame);
            recordEnd = (uint)Mathf.RoundToInt(endFrame);

            Replay(1f);
        }

        private void ResetRecordValues()
        {
            recordStart = uint.MaxValue;
            recordEnd = uint.MaxValue;
        }

        #endregion

        private void Update()
        {
            UpdateChannelInfos();
            CheckKeyClicking();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            contextMenu?.Show(false);

            if(eventData.button == PointerEventData.InputButton.Right) 
            {
                CreateChannelByContextMenu();
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (isKeyDown_LeftControl || videoPlayer.isPlaying)
                    return;

                areaSelection.PointerDown();
                contextMenu?.Show(false);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            areaSelection.Cancel();
        }

        void UpdateCurrentSilderTimer()
        {
            float ratio = (float)currentFrame / (float)totalFrames;
            currentTimerText.text = TimeUtil.GetTimeString_SSMSMS(videoInfo.videoLength * ratio);
        }
    }
}