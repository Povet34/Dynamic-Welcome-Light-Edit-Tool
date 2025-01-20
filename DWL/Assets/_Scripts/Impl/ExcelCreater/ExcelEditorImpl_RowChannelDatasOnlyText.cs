using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using UnityEngine;
using static IExcelEditor;
using System;

public class ExcelEditorImpl_RowChannelDatasOnlyText : IExcelEditor
{
    IWorkbook workbook;

    #region Threading
    
    Task saveTask;

    public void FirstWriteToExcel(ExcelEditData data)
    {
        if (saveTask == null || saveTask.IsCompleted)
        {
            Provider.Instance.StartCoroutine(CoFirstWriteToExcel(data));
        }
        else
        {
            Provider.Instance.StartCoroutine(WaitForPreviousSaveToCompleteAndStartNew(data, FirstWriteToExcel));
        }
    }

    public void AdditionalWriteExcel(ExcelEditData data)
    {
        if (saveTask == null || saveTask.IsCompleted)
        {
            Provider.Instance.StartCoroutine(CoAdditionalWriteToExcel(data));
        }
        else
        {
            Provider.Instance.StartCoroutine(WaitForPreviousSaveToCompleteAndStartNew(data, AdditionalWriteExcel));
        }
    }

    private IEnumerator WaitForPreviousSaveToCompleteAndStartNew(ExcelEditData data, Action<ExcelEditData> action)
    {
        yield return new WaitUntil(() => saveTask == null || saveTask.IsCompleted);
        action(data);
    }

    private IEnumerator CoFirstWriteToExcel(ExcelEditData data)
    {
        Provider.Instance.ShowLoadingPopup(true);
        saveTask = Task.Run(() => FirstWriteExcelBody(data));

        while (!saveTask.IsCompleted)
        {
            yield return null;
        }

        Provider.Instance.ShowLoadingPopup(false);
    }

    private IEnumerator CoAdditionalWriteToExcel(ExcelEditData data)
    {
        Provider.Instance.ShowLoadingPopup(true);

        saveTask = Task.Run(() => AdditionalWriteExcelBody(data));

        while (!saveTask.IsCompleted)
        {
            yield return null;
        }

        Provider.Instance.ShowLoadingPopup(false);
    }

    #endregion

    private void FirstWriteExcelBody(ExcelEditData data)
    {
        workbook = new XSSFWorkbook();
        WriteSimplifiedChannelsSheet(data.recordedMap, data.frameCount);
        WritePositionSheet(data.recordedMap);
        WriteVideoDataSheet(data.frameCount, data.videoLength, data.resolution);
        WriteDutySheet();

        WriteSingleTable(data.modeTable, Definitions.ORIGIN_EXCEL_MODE_TABLE);
        WriteMultiTable(data.timeTable, Definitions.ORIGIN_EXCEL_TIME_TABLE);
        WriteMultiTable(data.dutyTable, Definitions.ORIGIN_EXCEL_DUTY_TABLE);

        using (FileStream fs = new FileStream(data.filePath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(fs);
        }
    }

    private void AdditionalWriteExcelBody(ExcelEditData data)
    {
        using (FileStream fs = new FileStream(data.filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            if (null == workbook)
            {
                if (data.isModify)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else
                {
                    NDebug.LogWarning("기존의 Workbook이 존재하지 않음.");
                    return;
                }
            }
        }

        // Create a new sheet
        ISheet sheet = workbook.CreateSheet(Definitions.CHANNEL_DATA_EXCEL_TEMP_SHEET_NAME);
        WriteAdditionalChannelSheet(sheet, data.recordedMap, data.frameCount, data.sheetName, data.type, data.bitConvertRatio, data.isModify);

        if (null != data.modeTable)
            WriteSingleTable(data.modeTable, Definitions.MODIFIED_EXCEL_MODE_TABLE);

        if (null != data.timeTable)
            WriteMultiTable(data.timeTable, Definitions.MODIFIED_EXCEL_TIME_TABLE);

        if (null != data.dutyTable)
            WriteMultiTable(data.dutyTable, Definitions.MODIFIED_EXCEL_DUTY_TABLE);

        using (FileStream fsOut = new FileStream(data.filePath, FileMode.Create, FileAccess.Write))
        workbook.Write(fsOut);
    }

    /// <summary>
    /// 간소화 데이터 저장
    /// </summary>
    /// <param name="recordedMap"></param>
    /// <param name="frameCount"></param>
    private void WriteSimplifiedChannelsSheet(Dictionary<RecordKey, List<RecordValue>> recordedMap, int frameCount)
    {
        ISheet sheet = workbook.CreateSheet(Definitions.CHANNEL_DATA_EXCEL_TEMP_SHEET_NAME);
        WriteAdditionalChannelSheet(sheet, recordedMap, frameCount, Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_ORIGIN_SIMPLIFIED, RecordValue.RecordType.Brightness);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sheet"></param>
    /// <param name="recordedMap"></param>
    /// <param name="frameCount"></param>
    /// <param name="validSheetName"></param>
    /// <param name="type"></param>
    /// <param name="bitConvertRatio"></param>
    /// <param name="isModify"></param>
    private void WriteAdditionalChannelSheet(ISheet sheet, Dictionary<RecordKey, List<RecordValue>> recordedMap, int frameCount, string validSheetName, RecordValue.RecordType type, float bitConvertRatio = 1, bool isModify = false)
    {
        IRow rangeHeaderRow = sheet.CreateRow(0);
        rangeHeaderRow.CreateCell(0).SetCellValue(Definitions.CHANNEL_DATA_EXCEL_SHEET_CHANNELTIME);

        for (int i = 0; i < frameCount; i++)
        {
            rangeHeaderRow.CreateCell(i + 1).SetCellValue($"{i + 1}");
        }

        foreach (var key in recordedMap.Keys)
        {
            var chCellIndex = key.index + 1;
            IRow channelRow = sheet.CreateRow(chCellIndex);
            channelRow.CreateCell(0).SetCellValue($"Ch{key.index.ToString("D2")}");

            for (int frame = 0; frame < recordedMap[key].Count; frame++)
            {
                IRow dataRow = sheet.GetRow(chCellIndex);
                if (dataRow == null)
                    dataRow = sheet.CreateRow(chCellIndex);

                switch (type)
                {
                    case RecordValue.RecordType.Brightness:
                        int brightness = (int)(recordedMap[key][frame].Brightness_Int * bitConvertRatio);
                        dataRow.CreateCell(frame + 1).SetCellValue(brightness);
                        break;
                    case RecordValue.RecordType.Color:
                        Color32 color = recordedMap[key][frame].color;
                        dataRow.CreateCell(frame + 1).SetCellValue($"{(int)(color.r * bitConvertRatio)},{(int)(color.g * bitConvertRatio)},{(int)(color.b * bitConvertRatio)}");
                        break;
                    case RecordValue.RecordType.Details:
                        float brightnessDecimal = recordedMap[key][frame].Brightness_Decimal * bitConvertRatio;
                        dataRow.CreateCell(frame + 1).SetCellValue(brightnessDecimal);
                        break;
                }
            }
        }

        TransposeChannelSheet(sheet, frameCount, validSheetName, isModify);
    }

    private void WritePositionSheet(Dictionary<RecordKey, List<RecordValue>> recordedMap)
    {
        ISheet sheet = workbook.CreateSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_POSITIONS);

        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("Channel");
        headerRow.CreateCell(1).SetCellValue("X");
        headerRow.CreateCell(2).SetCellValue("Y");

        foreach (var key in recordedMap.Keys)
        {
            int channelIndex = key.index;

            if (recordedMap[key].Count > 0)
            {
                IRow dataRow = sheet.CreateRow(channelIndex + 1);
                dataRow.CreateCell(0).SetCellValue($"Ch{channelIndex}");
                dataRow.CreateCell(1).SetCellValue(key.pos.x);
                dataRow.CreateCell(2).SetCellValue(key.pos.y);
            }
        }
    }

    private void WriteVideoDataSheet(int frameCount, float length, Vector2Int resolution)
    {
        ISheet sheet = workbook.CreateSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_VIDEO_DATA);

        //FrameCount
        IRow frameHeader = sheet.CreateRow(0);
        frameHeader.CreateCell(0).SetCellValue(Definitions.EXCEL_VIDEO_DATA_HEADER_FRAMECOUNT);
        frameHeader.CreateCell(1).SetCellValue(frameCount);

        //Video Length
        IRow lengthHeader = sheet.CreateRow(1);

        var cell = lengthHeader.CreateCell(0);
        cell.CellStyle = GetSampleStyle(workbook, new Color32(100, 100, 0, 0));
        cell.SetCellValue(Definitions.EXCEL_VIDEO_DATA_HEADER_VIDEOLENGTH);

        lengthHeader.CreateCell(1).SetCellValue(length);


        //Video Resolution
        IRow resolutionHeader = sheet.CreateRow(2);
        resolutionHeader.CreateCell(0).SetCellValue(Definitions.EXCEL_VIDEO_DATA_HEADER_RESOLUTION);
        resolutionHeader.CreateCell(1).SetCellValue($"{resolution.x},{resolution.y}");

        //Video File Path..?
    }

    private void WriteDutySheet()
    {
        ISheet sheet = workbook.CreateSheet(Definitions.CHANNEL_DATA_EXCEL_SHEET_DUTY);
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("DutyIndex");
        headerRow.CreateCell(1).SetCellValue("Level");

        var duty = Provider.Instance.GetDuty();
        for (int i = 0; i < duty.levels.Count; i++)
        {
            IRow dataRow = sheet.CreateRow(i + 1);
            dataRow.CreateCell(0).SetCellValue(i);
            dataRow.CreateCell(1).SetCellValue(duty.levels[i]);
        }
    }

    /// <summary>
    /// ch : int 형태의 데이터를 저장
    /// </summary>
    private void WriteSingleTable(Dictionary<int, int> singles, string sheetName)
    {
        ISheet sheet = workbook.GetSheet(sheetName);
        if (null == sheet)
            sheet = workbook.CreateSheet(sheetName);

        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("CH");
        headerRow.CreateCell(1).SetCellValue("Mode");

        foreach (var element in singles)
        {
            IRow dataRow = sheet.CreateRow(element.Key + 1);
            dataRow.CreateCell(0).SetCellValue(element.Key);
            dataRow.CreateCell(1).SetCellValue(element.Value);
        }
    }

    /// <summary>
    /// ch : list<int> 형태의 데이터를 저장
    /// </summary>
    private void WriteMultiTable(Dictionary<int, List<int>> multies, string sheetName)
    {
        ISheet sheet = workbook.GetSheet(sheetName);
        if (null == sheet)
            sheet = workbook.CreateSheet(sheetName);

        //int maxLength = multies.OrderByDescending(kv => kv.Value.Count).First().Value.Count;
        int maxLength = Definitions.EXCEL_TABLE_BUFFER;

        var normalizedDictionary = multies.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.Count == maxLength
                ? kv.Value
                : kv.Value.Concat(Enumerable.Repeat(0, maxLength - kv.Value.Count)).ToList()
        );

        foreach (var element in normalizedDictionary)
        {
            if (multies[element.Key].Count > 0)
            {
                IRow dataRow = sheet.CreateRow(element.Key);
                dataRow.CreateCell(0).SetCellValue($"Ch{element.Key}");
                for (int i = 0; i < normalizedDictionary[element.Key].Count; i++)
                {
                    dataRow.CreateCell(i + 1).SetCellValue(normalizedDictionary[element.Key][i]);
                }
            }
        }
    }

    /// <summary>
    /// 데이터를 먼저 가로로 만들어서 기입 후, 전치시킴. (temp)Sheet -> rename
    /// </summary>
    /// <param name="tempSheet"></param>
    /// <param name="frameCount"></param>
    /// <param name="validSheetName"></param>
    /// <param name="isModify"></param>
    private void TransposeChannelSheet(ISheet tempSheet, int frameCount, string validSheetName, bool isModify)
    {
        ISheet transposeSheet = isModify ? workbook.GetSheet(validSheetName) : workbook.CreateSheet(validSheetName);
        if (null == transposeSheet)
            transposeSheet = workbook.CreateSheet(validSheetName);

        // 원본 시트의 데이터를 읽어서 새로운 시트에 전치하여 기록
        for (int i = 0; i <= frameCount; i++) // <= frameCount 를 사용하여 헤더도 포함
        {
            IRow sourceRow = tempSheet.GetRow(i);
            if (sourceRow != null)
            {
                for (int j = 0; j < sourceRow.LastCellNum; j++) // LastCellNum은 실제 셀의 개수를 반환
                {
                    ICell sourceCell = sourceRow.GetCell(j);
                    IRow targetRow = transposeSheet.GetRow(j) ?? transposeSheet.CreateRow(j); // 대상 행이 없으면 생성
                    ICell targetCell = targetRow.CreateCell(i);

                    // 셀 타입에 따른 값을 전치 시트에 복사
                    if (sourceCell != null)
                    {
                        switch (sourceCell.CellType)
                        {
                            case CellType.String:
                                targetCell.SetCellValue(sourceCell.StringCellValue);
                                break;
                            case CellType.Numeric:
                                targetCell.SetCellValue(sourceCell.NumericCellValue);
                                break;
                                // 기타 셀 타입에 대한 처리가 필요하면 여기에 추가
                        }
                    }
                }
            }
        }

        workbook.RemoveSheetAt(workbook.GetSheetIndex(tempSheet.SheetName));
    }

    //스타일 가져오기
    private ICellStyle GetSampleStyle(IWorkbook workbook, Color32 brackgroundColor)
    {
        XSSFCellStyle style = (XSSFCellStyle)workbook.CreateCellStyle();
        style.Alignment = HorizontalAlignment.Center;
        style.VerticalAlignment = VerticalAlignment.Center;

        XSSFColor color = new XSSFColor(new byte[] { brackgroundColor.r, brackgroundColor.g, brackgroundColor.b });
        style.SetFillForegroundColor(color);

        return style;

        //cell.CellStyle = style; 이렇게 스타일을 지정해주면됨.
    }

    public ISheet GetSheet(string sheetName)
    {
        return workbook.GetSheet(sheetName);
    }
}
