using ChannelAnalyzers;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace Common.UI
{
    public class VideoFrameExtractorUI : UIBase
    {
        #region Enum Object : -------------------------------------------------

        private enum Texts
        {
            VideoSpeedValueText,
        }

        private enum Buttons
        {
            PlayVideoButton,
            SaveDataButton,
            LoadVideoButton,
            LoadExcelButton,
            CreateDetectedBrightnessPixelsButton,
            ShowDutySetterPopupButton,
            SplitLinesButton,
            PauseVideoButton,
            LoadSavedChannelPositionsButton,
            CreateModifyVideoButton,
            OpenDuttySetterButton,
            CopyrightLicenseButton,
        }

        private enum Toggles
        {
            ChannelNotifierViewToggle,
            DetectedChannelContainLineToggle,
            ToggleVideoSettings,
            ToggleChannelSettings,
            ToggleExtractSettings,
            ExtractRGBToggle,
            ExtractSaveBrightLinearToggle,
            ExtractBrightToggle,
            ScenarioChannelNotifierViewToggle,
            CreateRecordedScenarioVideoToggle,
            SwitchBrightnessDataTargetToggle,
        }

        private enum InputFields
        {
            InputFieldSplitLineCount,
            DataSimplifyGradualThresholdInputField,
            DataSimplifyRadicalThresholdInputField,
            DataSimplifyMinimumFadeCountInputField,
            BrightnessIntervalThresholdInputField,
            SaveMilisecDataByFrameInputField,
            ExtractBitInputField,
            ShapeWallThresholdInputField,
            PixelSpacingInputField,
            MaximumAmountInputField,
            ModifyVideoStartInputField,
            ModifyVideoEndInputField,
        }

        private enum GameObjects
        {
            GoVideoFrameExtractorUICanvas,
            GoPanelArea,
            GoVideoSettings,
            GoExtractSettings,
            GoChannelSettings,
        }

        #endregion

        #region Accessor : ----------------------------------------------------

        private UITextMeshPro videoSpeedValueText => GetText(Convert.ToInt32(Texts.VideoSpeedValueText));

        private UIButton playVideoButton => GetButton(Convert.ToInt32(Buttons.PlayVideoButton));
        private UIButton pauseVideoButton => GetButton(Convert.ToInt32(Buttons.PauseVideoButton));
        private UIButton saveDataButton => GetButton(Convert.ToInt32(Buttons.SaveDataButton));
        private UIButton loadVideoButton => GetButton(Convert.ToInt32(Buttons.LoadVideoButton));
        private UIButton loadExcelButton => GetButton(Convert.ToInt32(Buttons.LoadExcelButton));
        private UIButton createDetectedBrightnessPixelsButton => GetButton(Convert.ToInt32(Buttons.CreateDetectedBrightnessPixelsButton));
        private UIButton showDutySetterPopupButton => GetButton(Convert.ToInt32(Buttons.ShowDutySetterPopupButton));
        private UIButton splitLinesButton => GetButton(Convert.ToInt32(Buttons.SplitLinesButton));
        private UIButton loadSavedChannelPositionsButton => GetButton(Convert.ToInt32(Buttons.LoadSavedChannelPositionsButton));
        private UIButton createModifyVideoButton => GetButton(Convert.ToInt32(Buttons.CreateModifyVideoButton));
        private UIButton openDuttySetterButton => GetButton(Convert.ToInt32(Buttons.OpenDuttySetterButton));
        private UIButton copyrightLicenseButton => GetButton(Convert.ToInt32(Buttons.CopyrightLicenseButton));

        private UIToggle channelNotifierViewToggle => GetToggle(Convert.ToInt32(Toggles.ChannelNotifierViewToggle));
        private UIToggle detectedChannelContainLineToggle => GetToggle(Convert.ToInt32(Toggles.DetectedChannelContainLineToggle));
        private UIToggle toggleVideoSettings => GetToggle(Convert.ToInt32(Toggles.ToggleVideoSettings));
        private UIToggle toggleChannelSettings => GetToggle(Convert.ToInt32(Toggles.ToggleChannelSettings));
        private UIToggle toggleExtractSettings => GetToggle(Convert.ToInt32(Toggles.ToggleExtractSettings));
        private UIToggle extractRGBToggle => GetToggle(Convert.ToInt32(Toggles.ExtractRGBToggle));
        private UIToggle extractSaveBrightLinearToggle => GetToggle(Convert.ToInt32(Toggles.ExtractSaveBrightLinearToggle));
        private UIToggle extractBrightToggle => GetToggle(Convert.ToInt32(Toggles.ExtractBrightToggle));
        private UIToggle scenarioChannelNotifierViewToggle => GetToggle(Convert.ToInt32(Toggles.ScenarioChannelNotifierViewToggle));
        private UIToggle createRecordedScenarioVideoToggle => GetToggle(Convert.ToInt32(Toggles.CreateRecordedScenarioVideoToggle));
        private UIToggle switchBrightnessDataTargetToggle => GetToggle(Convert.ToInt32(Toggles.SwitchBrightnessDataTargetToggle));

        private UIInputField inputFieldSplitLineCount => GetInputField(Convert.ToInt32(InputFields.InputFieldSplitLineCount));
        private UIInputField dataSimplifyGradualThresholdInputField => GetInputField(Convert.ToInt32(InputFields.DataSimplifyGradualThresholdInputField));
        private UIInputField dataSimplifyRadicalThresholdInputField => GetInputField(Convert.ToInt32(InputFields.DataSimplifyRadicalThresholdInputField));
        private UIInputField dataSimplifyMinimumFadeCountInputField => GetInputField(Convert.ToInt32(InputFields.DataSimplifyMinimumFadeCountInputField));
        private UIInputField brightnessIntervalThresholdInputField => GetInputField(Convert.ToInt32(InputFields.BrightnessIntervalThresholdInputField));
        private UIInputField saveMilisecDataByFrameInputField => GetInputField(Convert.ToInt32(InputFields.SaveMilisecDataByFrameInputField));
        private UIInputField extractBitInputField => GetInputField(Convert.ToInt32(InputFields.ExtractBitInputField));
        private UIInputField shapeWallThresholdInputField => GetInputField(Convert.ToInt32(InputFields.ShapeWallThresholdInputField));
        private UIInputField pixelSpacingInputField => GetInputField(Convert.ToInt32(InputFields.PixelSpacingInputField));
        private UIInputField maximumAmountInputField => GetInputField(Convert.ToInt32(InputFields.MaximumAmountInputField));
        private UIInputField modifyVideoStartInputField => GetInputField(Convert.ToInt32(InputFields.ModifyVideoStartInputField));
        private UIInputField modifyVideoEndInputField => GetInputField(Convert.ToInt32(InputFields.ModifyVideoEndInputField));

        private GameObject goVideoFrameExtractorUICanvas => GetGameObject(Convert.ToInt32(GameObjects.GoVideoFrameExtractorUICanvas));
        private GameObject goPanelArea => GetGameObject(Convert.ToInt32(GameObjects.GoPanelArea));
        private GameObject goExtractSettings => GetGameObject(Convert.ToInt32(GameObjects.GoExtractSettings));
        private GameObject goVideoSettings => GetGameObject(Convert.ToInt32(GameObjects.GoVideoSettings));
        private GameObject goChannelSettings => GetGameObject(Convert.ToInt32(GameObjects.GoChannelSettings));

        #endregion

        [SerializeField] private DutySetterPoupupUI dutySetter;
        [SerializeField] private CopyrightPopupUI copyrightPopupUI;

        [SerializeField] private ChannelAnalyzer channelAnalyzer;
        [SerializeField] private ScenarioViewer scenarioViewer;

        [SerializeField] private VideoChannelInfoController videoChannelInfoController;
        [SerializeField] private GameObject videoViewerPrefab;

        [SerializeField] private UISlider playSpeedSlider;
        [SerializeField] private Dropdown modeDropDown;

        [SerializeField] private GameObject extractSideSettingsToggles;
        [SerializeField] private GameObject scenarioSideSettings;

        [Header("Channel Prefab")]
        [SerializeField] private GameObject squarePixelChannelPrefab;
        [SerializeField] private GameObject linePixelChannelPrefab;

        private VideoViewer videoViewer;
        private List<IPixelChannel> pixelChannels = new List<IPixelChannel>();

        private eModeType curModeType;
        private eExtractType curExtractType;

        private bool isChannelNotifierShow = false;
        private bool isCreateDetectedPixcelContainLines = false;

        private List<IPixelChannel> curSelectedChannels = new List<IPixelChannel>();

        protected override void Awake()
        {
            base.Awake();
            Bind<UITextMeshPro>(typeof(Texts));
            Bind<UIButton>(typeof(Buttons));
            Bind<UIToggle>(typeof(Toggles));
            Bind<UIInputField>(typeof(InputFields));
            Bind<GameObject>(typeof(GameObjects));

            RegisterEventHandler();

            ChangeMode(eModeType.DataExtract);
            ChangeExtractToggle(eExtractType.VideoSettings);

            ShowLoadingPopup(false);

            Provider.Instance.GetDuty().SetLevels(IDuty.GetFixedDutyGap());

            videoChannelInfoController.Init(ref pixelChannels, UpdateChannelPositionByController, ChangePositionByHandle);

            copyrightPopupUI.LoadActivatedDate();
        }

        public override void OnInit() { }

        public override void OnActive()
        {
            base.OnActive();
        }

        public override void OnInactive() { }
        public override void OnUpdateFrame() { }
        public override void OnUpdateSec() { }
        public override void OnClear() { }
        public override bool IsEscape() { return base.IsEscape(); }


        #region UI Event : ----------------------------------------------------

        #region Register Event Handler : ----------------------------
        private void RegisterEventHandler()
        {
            playVideoButton.BindButtonEvent(string.Empty, OnPlayVideoButton);
            pauseVideoButton.BindButtonEvent(string.Empty, OnPauseVideoButton);
            saveDataButton.BindButtonEvent(string.Empty, OnSaveDataButton);
            loadVideoButton.BindButtonEvent(string.Empty, OnLoadVideoButton);
            loadExcelButton.BindButtonEvent(string.Empty, OnLoadExcelButton);
            showDutySetterPopupButton.BindButtonEvent(string.Empty, OnShowDutySetterPopupButton);
            createDetectedBrightnessPixelsButton.BindButtonEvent(string.Empty, OnCreateDetectedBrightnessPixelsButton);
            loadSavedChannelPositionsButton.BindButtonEvent(string.Empty, OnLoadSavedChannelPositionsButton);
            splitLinesButton.BindButtonEvent(string.Empty, OnSplitLinesButton);
            createModifyVideoButton.BindButtonEvent(string.Empty, OnCreateModifyVideoButton);
            openDuttySetterButton.BindButtonEvent(string.Empty, OnOpenDuttySetterButton);
            copyrightLicenseButton.BindButtonEvent(string.Empty, OnCopyrightLicenseButton);

            channelNotifierViewToggle.BindToggleEvent(string.Empty, OnChannelNotifierViewToggle);
            detectedChannelContainLineToggle.BindToggleEvent(string.Empty, OnDetectedChannelContainLineToggle);
            scenarioChannelNotifierViewToggle.BindToggleEvent(string.Empty, OnScenarioChannelNotifierViewToggle);
            createRecordedScenarioVideoToggle.BindToggleEvent(string.Empty, OnCreateRecordedScenarioVideoToggle);
            switchBrightnessDataTargetToggle.BindToggleEvent(string.Empty, OnSwitchBrightnessDataTargetToggle);
            
            toggleChannelSettings.BindToggleEvent(string.Empty, OnToggleChannelSettings);
            toggleExtractSettings.BindToggleEvent(string.Empty, OnToggleExtractSettings);
            toggleVideoSettings.BindToggleEvent(string.Empty, OnToggleVideoSettings);

            playSpeedSlider.onValueChanged.AddListener(UpdatePlaySpeeedValueChanged);

            modeDropDown.onValueChanged.AddListener(ChangeMode_int);
        }
        #endregion

        public void OnCreateModifyVideoButton(string buttonData)
        {
            CreateModifyVideo();
        }        
        
        public void OnOpenDuttySetterButton(string buttonData)
        {
            ShowDutySetterPopup();
        }

        public void OnCopyrightLicenseButton(string buttonData)
        {
            ShowCopyrightPopup();
        }

        public void OnLoadSavedChannelPositionsButton(string buttonData)
        {
            LoadSavedChannelPositionsButton();
        }

        public void OnPlayVideoButton(string buttonData)
        {
            ReplayVideo();
        }

        public void OnPauseVideoButton(string buttonData)
        {
            PauseVideo();
        }

        public void OnSaveDataButton(string buttonData)
        {
            SaveData();
        }

        public void OnLoadVideoButton(string buttonData)
        {
            CreateNewVideoViewer();
        }

        public void OnLoadExcelButton(string buttonData)
        {
            LoadVerifiedExcel();
        }

        public void OnShowDutySetterPopupButton(string buttonData)
        {
            ShowDutySetterPopup();
        }

        public void OnCreateDetectedBrightnessPixelsButton(string buttonData)
        {
            CreateDetectedBrightnessPixels();
        }

        public void OnSplitLinesButton(string buttonData)
        {
            SplitLines();
        }

        public void OnChannelNotifierViewToggle(bool isOn, string toggleData)
        {
            ShowChannelNotifierToggle(isOn);
        }
        public void OnScenarioChannelNotifierViewToggle(bool isOn, string toggleData)
        {
            scenarioViewer.ChangeChannelViewState(isOn);
        }

        public void OnCreateRecordedScenarioVideoToggle(bool isOn, string toggleData)
        {
            scenarioViewer.CreateVideoState(isOn);
        }
        public void OnSwitchBrightnessDataTargetToggle(bool isOn, string toggleData)
        {
            scenarioViewer.SwitchBrightnessShowTarget(isOn);
        }

        public void OnDetectedChannelContainLineToggle(bool isOn, string toggleData)
        {
            DetectedChannelContainLineToggle(isOn);
        }

        public void OnToggleVideoSettings(bool isOn, string toggleData)
        {
            ChangeExtractToggle(eExtractType.VideoSettings);
        }

        public void OnToggleExtractSettings(bool isOn, string toggleData)
        {
            ChangeExtractToggle(eExtractType.ExtractSetting);
        }

        public void OnToggleChannelSettings(bool isOn, string toggleData)
        {
            ChangeExtractToggle(eExtractType.Channels);
        }

        private void UpdatePlaySpeeedValueChanged(float value)
        {
            if (playSpeedSlider)
                videoSpeedValueText.text = $"x {value.ToString("N1")}";
        }

        #endregion

        #region Video & Frame  : ----------------------------------------------

        private void RemoveViewer()
        {
            if(videoViewer)
                videoViewer.Destroy();
        }

        private void ReplayVideo()
        {
            if (curModeType == eModeType.Scenario)
            {
                scenarioViewer.Play(playSpeedSlider.value);
            }
            else if (curModeType == eModeType.DataExtract)
            {
                if (!videoViewer)
                    return;

                foreach (var ch in pixelChannels)
                {
                    if (ch.GetDragHandles().Count == 2)
                    {
                        Provider.Instance.ShowErrorPopup("Exist Segment Channels..");
                        return;
                    }

                    ch.UpdateSelection(false);
                }

                videoViewer?.Replay(playSpeedSlider.value);
            }
        }

        private void PauseVideo()
        {
            if (curModeType == eModeType.DataExtract)
            {
                videoViewer?.PauseVideo();
            }
            else if (curModeType == eModeType.Scenario)
            {
                scenarioViewer.PauseScenario();
            }
        }

        private void SaveData()
        {
            if (!videoViewer)
                return;

            if (null == Provider.Instance.GetDuty().levels || Provider.Instance.GetDuty().levels.Count == 0)
            {
                Provider.Instance.ShowErrorPopup("Please Set Duties..");
                return;
            }

            float.TryParse(dataSimplifyGradualThresholdInputField.text, out float gradualThreshold);
            float.TryParse(dataSimplifyRadicalThresholdInputField.text, out float radicalThreshold);
            int.TryParse(dataSimplifyMinimumFadeCountInputField.text, out int minimumFadeCount);
            int.TryParse(saveMilisecDataByFrameInputField.text, out int wantedMs);
            int.TryParse(extractBitInputField.text, out int bit);
            bool isExtractColor = extractRGBToggle.isOn;
            bool isExtractBrightness = extractBrightToggle.isOn;
            bool isLinearBrightness = extractSaveBrightLinearToggle.isOn;

            float extendRatio = (1000 / videoViewer.FrameRate) / wantedMs;
            videoViewer.SaveDataToCSV(gradualThreshold, radicalThreshold, minimumFadeCount, extendRatio, bit, isExtractColor, isExtractBrightness, isLinearBrightness);
        }

        private VideoViewer CreateNewVideoViewer(string paramPath = null)
        {
            if (!videoViewerPrefab)
                return null;

            RemoveAllExtractChannels();
            RemoveViewer();

            videoViewer = Create((int)RectTransformExtensions.RegionType.Center);

            return videoViewer;

            VideoViewer Create(int idx)
            {
                var path = paramPath ?? Provider.Instance.GetVideoLoader()?.OpenFilePath();
                var fileName = Path.GetFileName(path);

                if (string.IsNullOrEmpty(path))
                    return null;

                var go = Instantiate(videoViewerPrefab, goPanelArea.transform);
                if (go)
                {
                    var viewer = go.GetComponent<VideoViewer>();
                    if (viewer)
                    {
                        var videoInfo = new VideoViewer.VideoInfo();
                        videoInfo.filePath = path;
                        videoInfo.fileName = fileName;

                        var viewerInfo = new VideoViewer.ViewerInfo();
                        viewerInfo.viewerIndex = idx;
                        viewerInfo.parentTr = goPanelArea.GetComponent<RectTransform>();

                        viewer.Init(
                            viewerInfo, 
                            ref pixelChannels, 
                            AddCurChannelList,
                            CreateUseContextMenu,
                            ClearCurChannelList
                            );
                        viewer.PrepareVideo(videoInfo);

                        return viewer;
                    }
                }

                return null;
            }
        }

        private void ShowChannelNotifierToggle(bool isShow)
        {
            if (null != pixelChannels)
            {
                foreach (var channel in pixelChannels)
                    channel.ShowNotifier(isShow);

                isChannelNotifierShow = isShow;
            }
        }

        #endregion

        #region Pixel Control : -----------------------------------------------

        /// <summary>
        /// 새로운 채널의 Index
        /// </summary>
        private int GetLoacatedNewChannelIndex()
        {
            if (null == pixelChannels) return -1;
            else if (pixelChannels.Count == 0) return 0;

            ReassginChannelsIndices();

            return pixelChannels.Count;
        }

        private void RemoveEmptyValueInChannels()
        {
            if (null == pixelChannels) return;

            pixelChannels.RemoveAll(c => c == null);
        }

        private void SortByIndexInChannels()
        {
            if (null == pixelChannels) return;

            pixelChannels.OrderBy(x => x.Index).ToList();
        }

        /// <summary>
        /// 현재 ChannelList의 Index들을 다시 순서대로 적용한다.
        /// </summary>
        private void ReassginChannelsIndices()
        {
            if (null == pixelChannels) return;

            RemoveEmptyValueInChannels();
            SortByIndexInChannels();

            for (int i = 0; i < pixelChannels.Count; i++)
            {
                var pixel = pixelChannels[i];
                pixel.Index = i;
            }
        }

        private void DetectedChannelContainLineToggle(bool isOn)
        {
            isCreateDetectedPixcelContainLines = isOn;
        }

        private void SplitLines()
        {
            if (string.IsNullOrEmpty(inputFieldSplitLineCount.text))
                return;

            var strAmount = inputFieldSplitLineCount.text.Trim();
            if (!int.TryParse(strAmount, out int splitAmount))
                splitAmount = 10;

            var addList = new List<Vector2>();       //모디파잉 방지
            var removeList = new List<IPixelChannel>(); //모디파잉 방지

            int scaleFactor = 1;
            if (curModeType == eModeType.DataExtract)
                scaleFactor = videoViewer.CurrentViewImgSacle;

            if (null != curSelectedChannels)
                RebindPoint(curSelectedChannels);
            else
                RebindPoint(pixelChannels);

            foreach (var remove in removeList)
            {
                pixelChannels.Remove(remove);
                remove.Destroy();
            }

            foreach (var add in addList)
                CreateExtractPixelChannel(ePixelChannelType.Point, new List<Vector2>() { add });

            videoChannelInfoController.CreateAllChannelPositionCells();

            void RebindPoint(List<IPixelChannel> channels)
            {
                for (int ii = 0; ii < channels.Count; ii++)
                {
                    var channel = channels[ii];
                    var handles = channel.GetDragHandles();
                    if (handles.Count == 2)
                    {
                        if (splitAmount > 0)
                        {
                            var start = handles.First();
                            var end = handles.Last();

                            Vector2 startAnchorPos = start.GetComponent<RectTransform>().anchoredPosition;
                            Vector2Int startAnchorPosInt = new Vector2Int((int)startAnchorPos.x, (int)startAnchorPos.y);

                            float dist = Vector2.Distance(end.transform.position, start.transform.position) / scaleFactor;
                            Vector2 dir = (end.transform.position - start.transform.position).normalized;

                            //시작점은 일단 넣고
                            addList.Add(startAnchorPosInt);

                            // Dist를 총 갯수의 -1 만큼 분할하여, 맨끝도 채워넣울 수 있도록 한다
                            Vector2 nextPos = dist / (splitAmount - 1) * dir;
                            int posXInt = nextPos.x < 0 ? Mathf.FloorToInt(nextPos.x) : Mathf.CeilToInt(nextPos.x);
                            int posYInt = nextPos.y < 0 ? Mathf.FloorToInt(nextPos.y) : Mathf.CeilToInt(nextPos.y);

                            Vector2Int nextPosInt = new Vector2Int(posXInt, posYInt);
                            for (int oo = 1; oo < splitAmount; oo++)
                                addList.Add(startAnchorPosInt + (oo * nextPos));
                        }

                        removeList.Add(channel);
                    }
                }
            }
        }

        #region Find Brightness pixels and auto create channels

        private async void CreateDetectedBrightnessPixels()
        {
            PauseVideo();
            ShowLoadingPopup(true);

            RemoveAllExtractChannels();

            var loader = Provider.Instance.GetVideoLoader();


            var path = loader.OpenFilePath();
            var viewer = CreateNewVideoViewer(path);

            //영상의 최초/최대 밝기의 간격을 지정함. 파싱안되면 140
            if (!int.TryParse(brightnessIntervalThresholdInputField.text, out int brightInterval))
                brightInterval = 140;
            if (!int.TryParse(pixelSpacingInputField.text, out int pixelSpacing))
                pixelSpacing = 3;
            if (!float.TryParse(shapeWallThresholdInputField.text, out float shapeWallThreshold))
                shapeWallThreshold = 2f;
            if (!int.TryParse(maximumAmountInputField.text, out int maximumAmount))
                maximumAmount = 2500;

            IBrightingDetector.DetectingParameter param = new IBrightingDetector.DetectingParameter();
            param.videoPath = path;
            param.isCreateDetectedPixcelContainLines = isCreateDetectedPixcelContainLines;
            param.brightInterval = brightInterval;
            param.pixelSpacing = pixelSpacing;
            param.shapeWallThreshold = shapeWallThreshold;
            param.errorCallback = () =>
            {
                ShowLoadingPopup(false);
                Provider.Instance.ShowErrorPopup("Missing Video..");
            };

            var info = await Provider.Instance.GetBrightingDetector().ProcessVideoAsync(param);

            if (null != info)
            {
                info.SortByPositionX();
                _TestContours();

                int limit = info.centers.Count > maximumAmount ? maximumAmount : info.centers.Count;
                StartCoroutine(CreateChannelAsync(info.centers, limit, viewer));
            }

            void _TestContours()
            {
                //test
                var tester = FindObjectOfType<TestContour>();
                if (tester && tester.gameObject.activeSelf)
                {
                    tester.SetContours(info.contours);
                    if (tester.isOnlyTest)
                        return;
                }
            }
        }

        IEnumerator CreateChannelAsync(List<BrightChannelInfo> centers, int limit, VideoViewer viewer)
        {
            int chunkSize = 30;
            int count = 0;
            while (count < limit)
            {
                for (int i = 0; i < chunkSize; i++)
                {
                    if(count < limit)
                        Create(centers[count++], viewer);
                }

                yield return null;
            }

            ShowLoadingPopup(false);

            void Create(BrightChannelInfo channelInfo, VideoViewer viewer)
            {
                if (channelInfo.points.Count == 1)
                {
                    var point = channelInfo.points.First();
                    channelInfo.SetPoint(viewer.GetPositionFromVideoCoordinates(point));
                }
                else if (channelInfo.points.Count == 2)
                {
                    var reLocatedPoints = new List<Vector2>
                {
                    viewer.GetPositionFromVideoCoordinates(channelInfo.points[0]),
                    viewer.GetPositionFromVideoCoordinates(channelInfo.points[1])
                };
                    channelInfo.SetPoints(reLocatedPoints);
                }
                CreateExtractPixelChannel(channelInfo.type, channelInfo.points, false);
            }
        }

        #endregion

        private void RemoveAllExtractChannels()
        {
            foreach (var channel in pixelChannels)
                channel.Destroy();

            pixelChannels.Clear();
            videoChannelInfoController.ClearList();
        }

        private void RemoveChannel(IPixelChannel channel)
        {
            if (null != channel)
            {
                if (pixelChannels.Remove(channel))
                {
                    channel.Destroy();
                    ReassginChannelsIndices();
                }
            }
        }

        private GameObject GetChannelPrefab(ePixelChannelType type)
        {
            return type switch
            {
                ePixelChannelType.Point => squarePixelChannelPrefab,
                ePixelChannelType.Segment => linePixelChannelPrefab,
                _ => null,
            };
        }

        private async void LoadSavedChannelPositionsButton()
        {
            RemoveAllExtractChannels();

            var path = Provider.Instance.GetExcelLoader()?.OpenFilePath();
            var fileName = Path.GetFileName(path);

            if (string.IsNullOrEmpty(fileName))
                return;

            if(!videoViewer)
            {
                Provider.Instance.ShowErrorPopup("Excel Path is not vaild");
                return;
            }

            var excelReader = Provider.Instance.GetExcelReader();

            if (!string.IsNullOrEmpty(path))
            {
                var cellInfos = await excelReader.ReadExcelDataAsync(path);
                if (null != cellInfos)
                {
                    List<Vector2> positions = new List<Vector2>();

                    foreach (var key in cellInfos.simplifiedMap.Keys)
                        positions.Add(videoViewer.GetPositionFromVideoCoordinates(key.pos));

                    if (null != positions)
                    {
                        foreach (var pos in positions)
                            CreateExtractPixelChannel(ePixelChannelType.Point, new List<Vector2> { pos });
                    }
                }
            }
        }

        private void CreateUseContextMenu(ePixelChannelType type, Vector2 pos) 
        {
            CreateExtractPixelChannel(type, new List<Vector2> {pos});
        }


        private void CreateExtractPixelChannel(ePixelChannelType type, List<Vector2> initPositions = null, bool isPaure = true)
        {
            if (isPaure)
                PauseVideo();

            if (!videoViewer)
                return;

            var prefab = GetChannelPrefab(type);
            if (prefab)
            {
                var go = Instantiate(prefab, goVideoFrameExtractorUICanvas.transform);
                if (go)
                {
                    var pixel = go.GetComponent<IPixelChannel>();
                    if (null != pixel)
                    {
                        var initPos = null == initPositions ? videoViewer.GetComponent<RectTransform>().rect.center : initPositions.First();

                        pixel.Init(GetLoacatedNewChannelIndex(), Color.white, initPos, videoViewer.CurrentViewImgSacle, isChannelNotifierShow, ChangeSelectedChannel, ChangePositionByHandle);
                        pixel.SetParent(videoViewer.GetRawImageTr());
                        pixel.SetHandlePos(initPositions);

                        pixelChannels.Add(pixel);

                        if (pixel.PixelChannelType == ePixelChannelType.Point)
                            videoChannelInfoController.CreateChannelCell(pixel.Index, pixel.GetFirstHandlePos());
                    }
                }
            }

            videoViewer.OnCreatedChannel();
        }

        private void ChangeSelectedChannel(IPixelChannel selection)
        {
            if (null != selection)
            {
                ClearCurChannelList();
                AddCurChannelList(selection);

                PauseVideo();
            }
        }


        /// <summary>
        /// 채널 핸들의 변경값을 Controller로 변경한다.
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <param name="newPos"></param>
        private void ChangePositionByHandle(int channelIndex, Vector2Int newPos)
        {
            videoChannelInfoController.ChangePositionByHandle(channelIndex, videoViewer.GetPositionInVideoCoordinatesByPosition(newPos));
        }

        //단순히 채널만 추가한다
        private void AddCurChannelList(IPixelChannel channel)
        {
            if (null != channel && null != curSelectedChannels)
            {
                curSelectedChannels.Add(channel);
                channel.UpdateSelection(true);
            }
        }

        private void ClearCurChannelList()
        {
            if (null != curSelectedChannels)
            {
                foreach (var ch in curSelectedChannels)
                    ch.UpdateSelection(false);

                curSelectedChannels.Clear();
            }
        }

        private void DeletePickedChannel()
        {
            if (null == curSelectedChannels || curSelectedChannels.Count == 0)
                return;

            for (int i = 0; i < curSelectedChannels.Count; i++)
            {
                var ch = curSelectedChannels[i];
                RemoveChannel(ch);
            }

            videoChannelInfoController.RefreshChannlInfos();
        }

        #endregion

        #region Verification : ------------------------------------------------

        private void RemoveAllViewChannels()
        {
            if (scenarioViewer)
                scenarioViewer.RemoveAllViewChannels();
        }

        private void LoadVerifiedExcel()
        {
            if (scenarioViewer)
                scenarioViewer.LoadVerifiedExcel();
        }

        private void ShowScenarioViewer(bool isShow)
        {
            if (scenarioViewer)
            {
                scenarioViewer.Show(isShow, new ScenarioViewer.ViewerInfo(scenarioChannelNotifierViewToggle.isOn));
            }
        }

        #endregion

        #region Mode Control : ------------------------------------------------

        private void InteractAll(bool isOn)
        {
            playVideoButton.interactable = isOn;
            saveDataButton.interactable = isOn;
            loadVideoButton.interactable = isOn;
            loadExcelButton.interactable = isOn;
            openDuttySetterButton.interactable = isOn;
            pauseVideoButton.interactable = isOn;
            showDutySetterPopupButton.interactable = isOn;
            createDetectedBrightnessPixelsButton.interactable = isOn;
            detectedChannelContainLineToggle.interactable = isOn;

            channelNotifierViewToggle.interactable = isOn;
            splitLinesButton.interactable = isOn;
            inputFieldSplitLineCount.interactable = isOn;

            SetInteractableSettingInputs(isOn);
        }

        public enum eModeType
        {
            DataExtract,
            Scenario,
            Analyze,
        }

        private void ChangeMode_int(int mode)
        {
            ChangeMode((eModeType)mode);
        }

        private void ChangeMode(eModeType modeType)
        {
            ShowUI(true);
            curModeType = modeType;

            channelAnalyzer.Hide();
            InteractAll(false);
            ShowScenarioViewer(false);

            RemoveViewer();
            RemoveAllExtractChannels();
            RemoveAllViewChannels();

            switch (modeType)
            {
                case eModeType.DataExtract:
                    {
                        //createSquarePixelChannalButton.interactable = true;
                        //createLinePixelChannalButton.interactable = true;
                        loadVideoButton.interactable = true;
                        playVideoButton.interactable = true;
                        saveDataButton.interactable = true;
                        createDetectedBrightnessPixelsButton.interactable = true;
                        channelNotifierViewToggle.interactable = true;
                        splitLinesButton.interactable = true;
                        inputFieldSplitLineCount.interactable = true;
                        showDutySetterPopupButton.interactable = true;
                        detectedChannelContainLineToggle.interactable = true;
                        pauseVideoButton.interactable = true;
                        openDuttySetterButton.interactable = true;

                        ShowSideToggles();
                        ShowTopLeftButtons();

                        //ShowCreateChannelButtons(true);
                        SetInteractableSettingInputs(true);
                    }
                    break;
                case eModeType.Scenario:
                    {
                        ShowScenarioViewer(true);

                        ShowSideToggles();
                        ShowTopLeftButtons();

                        playVideoButton.interactable = true;
                        pauseVideoButton.interactable = true;
                        loadExcelButton.interactable = true;
                    }
                    break;
                case eModeType.Analyze:
                    {
                        channelAnalyzer.Show();
                        ShowTopLeftButtons();
                    }
                    break;
            }
        }

        private void ShowTopLeftButtons()
        {
            loadVideoButton.gameObject.SetActive(curModeType == eModeType.DataExtract);
            saveDataButton.gameObject.SetActive(curModeType == eModeType.DataExtract);
            createDetectedBrightnessPixelsButton.gameObject.SetActive(curModeType == eModeType.DataExtract);
            loadSavedChannelPositionsButton.gameObject.SetActive(curModeType == eModeType.DataExtract);
            openDuttySetterButton.gameObject.SetActive(curModeType == eModeType.DataExtract);
            copyrightLicenseButton.gameObject.SetActive(curModeType == eModeType.DataExtract);

            loadExcelButton.gameObject.SetActive(curModeType == eModeType.Scenario);
        }

        private void ShowSideToggles()
        {
            extractSideSettingsToggles.gameObject.SetActive(curModeType == eModeType.DataExtract);
            scenarioSideSettings.gameObject.SetActive(curModeType == eModeType.Scenario);
        }

        private void SetInteractableSettingInputs(bool isOn)
        {
            dataSimplifyGradualThresholdInputField.interactable = isOn;
            dataSimplifyMinimumFadeCountInputField.interactable = isOn;
            dataSimplifyRadicalThresholdInputField.interactable = isOn;
            brightnessIntervalThresholdInputField.interactable = isOn;
            shapeWallThresholdInputField.interactable = isOn;
            pixelSpacingInputField.interactable = isOn;
            saveMilisecDataByFrameInputField.interactable = isOn;
            extractBitInputField.interactable = isOn;
            maximumAmountInputField.interactable = isOn;
        }

        private void ShowUI(bool isShow)
        {
            gameObject.SetActive(isShow);
        }

        private void ShowLoadingPopup(bool isShow)
        {
            Provider.Instance.ShowLoadingPopup(isShow);
        }

        #endregion

        #region Extract Toggle Control : --------------------------------------

        public enum eExtractType
        {
            VideoSettings,
            Channels,
            ExtractSetting,
        }

        private void ChangeExtractToggle(eExtractType type)
        {
            ShowAll(false);
            curExtractType = type;

            switch (type)
            {
                case eExtractType.VideoSettings:
                    goVideoSettings.gameObject.SetActive(true);
                    break;
                case eExtractType.Channels:
                    goChannelSettings.gameObject.SetActive(true);
                    videoChannelInfoController.gameObject.SetActive(true);
                    break;
                case eExtractType.ExtractSetting:
                    goExtractSettings.gameObject.SetActive(true);
                    break;
            }

            CheckModeToggles();

            void ShowAll(bool isShow)
            {
                videoChannelInfoController.gameObject.SetActive(isShow);

                goExtractSettings.gameObject.SetActive(isShow);
                goChannelSettings.gameObject.SetActive(isShow);
                goVideoSettings.gameObject.SetActive(isShow);
            }

            void CheckModeToggles()
            {
                toggleChannelSettings.SetIsOnWithoutNotify(curExtractType == eExtractType.Channels);
                toggleExtractSettings.SetIsOnWithoutNotify(curExtractType == eExtractType.ExtractSetting);
                toggleVideoSettings.SetIsOnWithoutNotify(curExtractType == eExtractType.VideoSettings);
            }
        }
        #endregion

        #region Duty Settiing Popup : -----------------------------------------

        void ShowDutySetterPopup()
        {
            dutySetter?.Show();
        }

        #endregion

        #region Copyright/License Popup : -------------------------------------

        void ShowCopyrightPopup()
        {
            copyrightPopupUI?.Show();
        }

        #endregion

        #region Keyborad : ----------------------------------------------------

        private void DeleteChannel()
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DeletePickedChannel();
            }
        }

        #endregion

        #region Channel List Controller : -------------------------------------

        /// <summary>
        /// Controller에서 Pos를 변경하게되는경우, pixelChannel을 업데이트한다.
        /// </summary>
        private void UpdateChannelPositionByController(int chIdx, Vector2Int pos)
        {
            if (null == pixelChannels)
                return;

            foreach (var ch in pixelChannels)
            {
                if (ch.Index == chIdx)
                {
                    ch.SetHandlePos(new List<Vector2> { videoViewer.GetPositionFromVideoCoordinates(pos) });
                    break;
                }
            }
        }

        #endregion

        #region Create Modify Video : -----------------------------------------

        void CreateModifyVideo()
        {
            if (!int.TryParse(modifyVideoStartInputField.text, out int start))
                start = 0;
            if (!int.TryParse(modifyVideoEndInputField.text, out int end))
                end = 100000;
            videoViewer.CreateModifiedVideo(start, end);
        }

        #endregion

        private void Update()
        {
            DeleteChannel();
        }
    }
}