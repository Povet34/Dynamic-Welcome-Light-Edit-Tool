using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static ScenarioViewer;

public class ExcelReaderImpl : IExcelReader
{
    public async Task<IExcelReader.Data> ReadExcelDataAsync(string filePath)
    {
        Provider.Instance.ShowLoadingPopup(true);

        Dictionary<RecordKey, List<RecordValue>> simplifiedMap = new Dictionary<RecordKey, List<RecordValue>>();
        Dictionary<RecordKey, List<RecordValue>> originBrightnessMap = new Dictionary<RecordKey, List<RecordValue>>();
        Dictionary<RecordKey, List<RecordValue>> modifiedBrightnessMap = new Dictionary<RecordKey, List<RecordValue>>();

        ScenarioInfo resultInfo = new ScenarioInfo();
        List<int> duties;

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            IWorkbook workbook = await Task.Run(() => new XSSFWorkbook(fs));

            var positionSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_POSITIONS);
            await Task.Run(() => ReadPositionData(simplifiedMap, positionSheet));
            await Task.Run(() => ReadPositionData(originBrightnessMap, positionSheet));
            await Task.Run(() => ReadPositionData(modifiedBrightnessMap, positionSheet));

            var modified = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_SIMPLIFIED);
            var simplifiedSheet = modified == null ? workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_ORIGIN_SIMPLIFIED) : modified;

            var originBrightnessSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED);
            var modifiedBrightnessSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_BRIGHTNESS);

            await Task.Run(() => ReadBrightnessData(simplifiedMap, simplifiedSheet));
            await Task.Run(() => ReadBrightnessData(originBrightnessMap, originBrightnessSheet));

            if(null != modifiedBrightnessSheet)
                await Task.Run(() => ReadBrightnessData(modifiedBrightnessMap, modifiedBrightnessSheet));

            duties = ReadDutyLevelsData(workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_DUTY));

            var videoInfoSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_VIDEO_DATA);
            resultInfo = await Task.Run(() => ReadVideoData(videoInfoSheet));
        }

        Provider.Instance.ShowLoadingPopup(false);

        return new IExcelReader.Data(simplifiedMap, originBrightnessMap, modifiedBrightnessMap, resultInfo, duties);
    }

    public IExcelReader.Data ReadExcelData(string filePath)
    {
        Dictionary<RecordKey, List<RecordValue>> simplifiedMap = new Dictionary<RecordKey, List<RecordValue>>();
        Dictionary<RecordKey, List<RecordValue>> originBrightnessMap = new Dictionary<RecordKey, List<RecordValue>>();
        Dictionary<RecordKey, List<RecordValue>> modifiedBrightnessMap = new Dictionary<RecordKey, List<RecordValue>>();

        ScenarioInfo resultInfo = new ScenarioInfo();
        List<int> duties;

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(fs);

            var positionSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_POSITIONS);
            ReadPositionData(simplifiedMap, positionSheet);
            ReadPositionData(originBrightnessMap, positionSheet);
            ReadPositionData(modifiedBrightnessMap, positionSheet);

            var modified = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_SIMPLIFIED);
            var simplifiedSheet = modified == null ? workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_ORIGIN_SIMPLIFIED) : modified;

            var originBrightnessSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED);
            var modifiedBrightnessSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_BRIGHTNESS);

            ReadBrightnessData(simplifiedMap, simplifiedSheet);
            ReadBrightnessData(originBrightnessMap, originBrightnessSheet);

            if (null != modifiedBrightnessSheet)
                ReadBrightnessData(modifiedBrightnessMap, modifiedBrightnessSheet);

            duties = ReadDutyLevelsData(workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_DUTY));

            var videoInfoSheet = workbook.GetSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_VIDEO_DATA);
            resultInfo = ReadVideoData(videoInfoSheet);
        }

        return new IExcelReader.Data(simplifiedMap, originBrightnessMap, modifiedBrightnessMap, resultInfo, duties);
    }

    private List<int> ReadDutyLevelsData(ISheet sheet)
    {
        if (null != sheet)
        {
            List<int> duties = new List<int>();

            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                IRow excelRow = sheet.GetRow(row);
                int duty = Convert.ToInt32(excelRow.GetCell(1).NumericCellValue);
                duties.Add(duty);
            }

            return duties;
        }
        return null;
    }

    private void ReadPositionData(Dictionary<RecordKey, List<RecordValue>> result, ISheet sheet)
    {
        for (int row = 1; row <= sheet.LastRowNum; row++)
        {
            IRow excelRow = sheet.GetRow(row);

            if (int.TryParse(excelRow.GetCell(0).StringCellValue.Replace("Ch", ""), out int index))
            {
                int posX = Convert.ToInt32(excelRow.GetCell(1).NumericCellValue);
                int posY = Convert.ToInt32(excelRow.GetCell(2).NumericCellValue);

                RecordKey key = new RecordKey(index, new Vector2Int(posX, posY));

                if (!result.ContainsKey(key))
                    result[key] = new List<RecordValue>();
            }
        }
    }

    private void ReadBrightnessData(Dictionary<RecordKey, List<RecordValue>> result, ISheet sheet)
    {
        int channelCount = -1;
        for (int row = 1; row <= sheet.LastRowNum; row++)
        {
            IRow excelRow = sheet.GetRow(row);

            if(excelRow.GetCell(0) == null)
            {
                NDebug.LogError($"[ReadBrightnessData - {sheet.SheetName}] Currnet nullptr Row: {row}");
                continue;
            }

            if (int.TryParse(excelRow.GetCell(0).StringCellValue, out int frame))
            {
                //채널 Count는 가장 첫줄에 있는 갯수를 보고 그냥 판단한다.
                if (channelCount == -1) channelCount = excelRow.Cells.Count;

                for (int ch = 1; ch < channelCount; ch++)
                {
                    var cell = excelRow.GetCell(ch);
                    if(null != cell)
                    {
                        int brightness = Convert.ToInt32(cell.NumericCellValue);
                        RecordValue value = new RecordValue(brightness);
                        RecordKey targetKey = result.Keys.FirstOrDefault(key => key.index == ch - 1);

                        if (result.ContainsKey(targetKey))
                            result[targetKey].Add(value);
                    }
                    else
                    {
                        NDebug.LogError($"[{sheet.SheetName}]Sheet의 {row}줄({frame}Frame)에서 {ch}채널의 Cell이 문제임. 총 채널의 갯수 : {excelRow.Cells.Count}");
                    }
                }
            }
        }
    }

    private ScenarioInfo ReadVideoData(ISheet sheet)
    {
        if (null != sheet)
        {
            ScenarioInfo videoInfo = new ScenarioInfo();

            //FrameCount
            videoInfo.frameCount = (int)sheet.GetRow(0).GetCell(1).NumericCellValue;

            //Video Length
            videoInfo.videoLength = (float)sheet.GetRow(1).GetCell(1).NumericCellValue;

            //Video Resolution
            string[] resolutionParts = sheet.GetRow(2).GetCell(1).StringCellValue.Split(',');
            videoInfo.resolution = new Vector2Int(int.Parse(resolutionParts[0]), int.Parse(resolutionParts[1]));

            return videoInfo;
        }

        return default(ScenarioInfo);
    }
}
