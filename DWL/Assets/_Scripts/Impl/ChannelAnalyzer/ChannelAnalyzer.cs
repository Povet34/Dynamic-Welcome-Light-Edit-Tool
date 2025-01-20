using Common.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace ChannelAnalyzers
{
    public class ChannelAnalyzer : MonoBehaviour
    {
        [SerializeField] private VideoFrameExtractorUI extractorUI;
        [SerializeField] private ChannelsController channelsController;
        [SerializeField] private GraphBuildersController graphController;
        [SerializeField] private UIButton loadExcelButton;

        private List<AChannelInfo> channelInfos;
        private List<int> savedDuties;

        private string filePath;
        public string FilePath => filePath;

        private void Awake()
        {
            loadExcelButton.SetCallback(null, OnLoadExcelButton);
        }

        #region Show & Hide : -------------------------------------

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            channelsController.DestroyAllChannelCells();
            graphController.DestroyAllGraphBuilders();

            gameObject.SetActive(false);
        }

        #endregion

        #region Initialize : --------------------------------------

        private void OnLoadExcelButton(string buttonData)
        {
            FindAndReadExcel();
        }

        private async void FindAndReadExcel()
        {
            FindExcelFile();
            bool isSuccess = await ReadExcel();

            if (isSuccess)
            {
                InitChannelsController();
                InitGraphBuildersController();
            }
        }

        void InitGraphBuildersController()
        {
            GraphControllerInfo info = new GraphControllerInfo();
            info.channelInfos = channelInfos;
            info.savedDuties = savedDuties;

            info.errorCallback = Provider.Instance.ShowErrorPopup;
            info.saveCallback = SaveExcel;

            graphController.Init(info);
        }

        void InitChannelsController()
        {
            var info = new AChannelsControllerInfo();
            info.channelInfos = channelInfos;
            info.showGraphCallback = ShowGraph;

            channelsController.Init(info);
        }

        #endregion

        #region Excel File Save&Load : ----------------------------

        public void FindExcelFile()
        {
            filePath = Provider.Instance.GetExcelLoader().OpenFilePath();
        }

        public async Task<bool> ReadExcel()
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var readData = await Provider.Instance.GetExcelReader().ReadExcelDataAsync(filePath);
            {
                var recordMap = readData.simplifiedMap;
                channelInfos = ExcelDataConverter.ConvertToChannelCellInfo(recordMap);
                savedDuties = readData.duties;

                return true;
            }
        }

        public void SaveExcel(List<AChannelInfo> list)
        {
            var excelCreator = Provider.Instance.GetExcelEditor();

            SaveLinearBightnessToExcel(excelCreator, list, savedDuties);
            SaveSimplifiedMapToExcel(excelCreator, list);
        }

        public void SaveSimplifiedMapToExcel(IExcelEditor excelEditor, List<AChannelInfo> list)
        {
            var recordMap = ExcelDataConverter.ConvertToSimplifiedRecordMap(list);

            var excelData = new IExcelEditor.ExcelEditData();
            excelData.recordedMap = recordMap;
            excelData.frameCount = recordMap.FirstOrDefault().Value.Count;
            excelData.filePath = filePath;
            excelData.videoLength = 0;
            excelData.sheetName = Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_SIMPLIFIED;
            excelData.type = RecordValue.RecordType.Brightness;
            excelData.bitConvertRatio = Definitions.CONVERT_8BTI_TO_8BIT;
            excelData.isModify = true;

            var inferDic = ExcelDataConverter.GetInferencesDic(recordMap);
            excelData.modeTable = ExcelDataConverter.GetModeTable(recordMap);
            excelData.timeTable = ExcelDataConverter.GetTimeTable(inferDic);
            excelData.dutyTable = ExcelDataConverter.GetDutyTable(inferDic);

            excelEditor.AdditionalWriteExcel(excelData);
        }

        private void SaveLinearBightnessToExcel(IExcelEditor excelEditor, List<AChannelInfo> list, List<int> savedDuties)
        {
            var recordMap = ExcelDataConverter.ConvertToBrightnessRecordMap(list, savedDuties);

            var excelData = new IExcelEditor.ExcelEditData();
            excelData.recordedMap = recordMap;
            excelData.frameCount = recordMap.FirstOrDefault().Value.Count;
            excelData.filePath = filePath;
            excelData.videoLength = 0;
            excelData.sheetName = Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_BRIGHTNESS;
            excelData.type = RecordValue.RecordType.Brightness;
            excelData.bitConvertRatio = Definitions.CONVERT_8BTI_TO_8BIT;
            excelData.isModify = true;

            excelEditor.AdditionalWriteExcel(excelData);
            StartCoroutine(_SaveOtherBitData(excelEditor, excelData));
        }


        IEnumerator _SaveOtherBitData(IExcelEditor excelEditor, IExcelEditor.ExcelEditData excelData)
        {
            yield return new WaitForSeconds(3f);
            //10
            {
                var sheet = excelEditor.GetSheet($"{Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED}({10})");
                if (null != sheet)
                {
                    excelData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({10})", Definitions.CONVERT_8BTI_TO_10BIT);
                    excelEditor.AdditionalWriteExcel(excelData);
                    yield break;
                }

                sheet = excelEditor.GetSheet($"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({10})");
                if (null != sheet)
                {
                    excelData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({10})", Definitions.CONVERT_8BTI_TO_10BIT);
                    excelEditor.AdditionalWriteExcel(excelData);
                    yield break;
                }
            }

            //12
            {
                var sheet = excelEditor.GetSheet($"{Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED}({12})");
                if (null != sheet)
                {
                    excelData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({12})", Definitions.CONVERT_8BTI_TO_12BIT);
                    excelEditor.AdditionalWriteExcel(excelData);
                    yield break;
                }

                sheet = excelEditor.GetSheet($"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({12})");
                if (null != sheet)
                {
                    excelData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({12})", Definitions.CONVERT_8BTI_TO_12BIT);
                    excelEditor.AdditionalWriteExcel(excelData);
                }
            }

            //16
            {
                var sheet = excelEditor.GetSheet($"{Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED}({16})");
                if (null != sheet)
                {
                    excelData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({16})", Definitions.CONVERT_8BTI_TO_16BIT);
                    excelEditor.AdditionalWriteExcel(excelData);
                    yield break;
                }

                sheet = excelEditor.GetSheet($"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({16})");
                if (null != sheet)
                {
                    excelData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED}({16})", Definitions.CONVERT_8BTI_TO_16BIT);
                    excelEditor.AdditionalWriteExcel(excelData);
                    yield break;
                }
            }
        }

        #endregion

        #region Graph : -------------------------------------------

        void ShowGraph(AChannelInfo channelInfo, bool isShow)
        {
            graphController.ShowTargetGraph(channelInfo, isShow);
        }

        #endregion
    }
}

