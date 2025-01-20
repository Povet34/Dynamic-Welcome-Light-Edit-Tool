using ChannelAnalyzers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static IExcelEditor;

public class PixelDataRecorderImpl_CulmWithFrameByIndex : IPixelDataRecorder
{
    /// <summary>
    /// 확장되지 않은 동영상 본래의 frame 만큼의 밝기 데이터
    /// </summary>
    private Dictionary<RecordKey, List<RecordValue>> originalMap = new Dictionary<RecordKey, List<RecordValue>>();

    /// <summary>
    /// originalMap을 원하는 ms만큼 확장한 데이터
    /// </summary>
    private Dictionary<RecordKey, List<RecordValue>> extendedMap = new Dictionary<RecordKey, List<RecordValue>>();

    /// <summary>
    /// 확장된 데이터의 증가 감소를 좀더 명확하게 파악할 수 있도록 표현하기 위한 데이터
    /// </summary>
    private Dictionary<RecordKey, List<RecordValue>> detailMap = new Dictionary<RecordKey, List<RecordValue>>();

    /// <summary>
    /// 확장된 frame을 규칙에 맞게 간소화한 데이터
    /// </summary>
    private Dictionary<RecordKey, List<RecordValue>> simplifyMap = new Dictionary<RecordKey, List<RecordValue>>();

    public void AddData(RecordKey key, RecordValue value)
    {
        if (!originalMap.ContainsKey(key))
        {
            originalMap[key] = new List<RecordValue>();
        }

        originalMap[key].Add(value);
    }

    public void ClearMap()
    {
        originalMap.Clear();
        simplifyMap.Clear();
        extendedMap.Clear();
        detailMap.Clear();
    }

    public void CreateRecordFile(RecordFileInfo info)
    {
        ProcessBrightnessDatas(
            info.gradualThreshold,
            info.radicalThreshold,
            info.minimumFadeCount,
            info.extendRatio,
            info.isSaveBrightnessLinear
            );

        CreateAndWriteExcel(
            $"{info.name}_{info.bit}_{extendedMap.First().Value.Count}_{info.gradualThreshold}_{info.radicalThreshold}_{info.minimumFadeCount}",
            info.bit,
            info.videoLength,
            info.resolution,
            info.isExtractColor,
            info.isExtractBrightness
            );
    }

    private void CreateAndWriteExcel(string fileName, int bit, float videoLength, Vector2Int resolution, bool isExtractColor, bool isExtractBrightness)
    {
        IExcelEditor excelEditor = Provider.Instance.GetExcelEditor();
        if (null != excelEditor)
        {
            var folderLocater = Provider.Instance.GetFolderPathLocater();
            if (null != folderLocater)
            {
                var folderPath = folderLocater.GetLocatedFolderPath("DWL_Excels");

                var path = $"{folderPath}/{fileName}.xlsx";
                int frameCount = simplifyMap.Values.FirstOrDefault().Count;

                //First Write
                {
                    ExcelEditData firstData = new ExcelEditData();
                    firstData.recordedMap = simplifyMap;
                    firstData.filePath = path;
                    firstData.frameCount = frameCount;
                    firstData.videoLength = videoLength;
                    firstData.resolution = resolution;
                    firstData.type = RecordValue.RecordType.Brightness;

                    var inferDic = ExcelDataConverter.GetInferencesDic(simplifyMap);
                    firstData.modeTable = ExcelDataConverter.GetModeTable(simplifyMap);
                    firstData.timeTable = ExcelDataConverter.GetTimeTable(inferDic);
                    firstData.dutyTable = ExcelDataConverter.GetDutyTable(inferDic);

                    excelEditor.FirstWriteToExcel(firstData);
                }

                WriteAddData(excelEditor, path, videoLength, frameCount, bit, isExtractColor, isExtractBrightness);
                //WriteDetailData(excelEditor, path, videoLength, frameCount);
            }
        }
    }

    void WriteDetailData(IExcelEditor excelEditor, string path, float videoLength, int frameCount)
    {
        ExcelEditData addData = new ExcelEditData();
        addData.filePath = path;
        addData.videoLength = videoLength;
        addData.type = RecordValue.RecordType.Details;
        addData.recordedMap = detailMap;
        addData.frameCount = frameCount;
        addData.bitConvertRatio = Definitions.CONVERT_8BTI_TO_8BIT;
        addData.sheetName = Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_INTERNAL_COMPUTATION_ORIGIN_DEATIL_BRIGHTNESS;
        excelEditor.AdditionalWriteExcel(addData);
    }

    void WriteAddData(IExcelEditor excelEditor, string path, float videoLength, int frameCount, int bit, bool isExtractColor, bool isExtractBrightness)
    {
        //Additional Write
        {
            ExcelEditData addData = new ExcelEditData();
            addData.filePath = path;
            addData.videoLength = videoLength;
            addData.type = RecordValue.RecordType.Brightness;
            addData.recordedMap = extendedMap;
            addData.frameCount = frameCount;
            addData.bitConvertRatio = Definitions.CONVERT_8BTI_TO_8BIT;
            addData.sheetName = Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED;
            excelEditor.AdditionalWriteExcel(addData);

            addData.ChangeTypeAndSheetName(RecordValue.RecordType.Color, Definitions.CHANNEL_DATA_EXCEL_SHEET_COLOR, Definitions.CONVERT_8BTI_TO_8BIT);
            excelEditor.AdditionalWriteExcel(addData);

            //Bit
            {

                if (bit == 10)
                {
                    if (isExtractColor)
                    {
                        addData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED}({10})", Definitions.CONVERT_8BTI_TO_10BIT);
                        excelEditor.AdditionalWriteExcel(addData);
                    }

                    if (isExtractBrightness)
                    {
                        addData.ChangeTypeAndSheetName(RecordValue.RecordType.Color, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_COLOR}({10})", Definitions.CONVERT_8BTI_TO_10BIT);
                        excelEditor.AdditionalWriteExcel(addData);
                    }
                }
                else if (bit == 12)
                {
                    if (isExtractColor)
                    {
                        addData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED}({12})", Definitions.CONVERT_8BTI_TO_12BIT);
                        excelEditor.AdditionalWriteExcel(addData);
                    }

                    if (isExtractBrightness)
                    {
                        addData.ChangeTypeAndSheetName(RecordValue.RecordType.Color, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_COLOR}({12})", Definitions.CONVERT_8BTI_TO_12BIT);
                        excelEditor.AdditionalWriteExcel(addData);
                    }
                }
                else if (bit == 16)
                {
                    if (isExtractColor)
                    {
                        addData.ChangeTypeAndSheetName(RecordValue.RecordType.Brightness, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED}({16})", Definitions.CONVERT_8BTI_TO_12BIT);
                        excelEditor.AdditionalWriteExcel(addData);
                    }

                    if (isExtractBrightness)
                    {
                        addData.ChangeTypeAndSheetName(RecordValue.RecordType.Color, $"{Definitions.CHANNEL_DATA_EXCEL_SHEET_COLOR}({16})", Definitions.CONVERT_8BTI_TO_12BIT);
                        excelEditor.AdditionalWriteExcel(addData);
                    }
                }
            }

            //Internal Compute
            {
                addData.ChangeRecordMap(detailMap);
                addData.ChangeTypeAndSheetName(RecordValue.RecordType.Details, Definitions.CHANNEL_DATA_EXCEL_SHEET_NAME_INTERNAL_COMPUTATION_ORIGIN_DEATIL_BRIGHTNESS, Definitions.CONVERT_8BTI_TO_8BIT);
                excelEditor.AdditionalWriteExcel(addData);
            }
        }
    }

    private void ProcessBrightnessDatas(float gradualThreshold, float radicalThreshold, int minimumFadeCount, float extendRatio, bool isLinear)
    {
        foreach (var entity in originalMap)
            detailMap[entity.Key] = isLinear ? GetExtendDataLinearList(entity.Value, extendRatio, gradualThreshold) : GetExtendDataList(entity.Value, extendRatio);

        //ExtendedMap 채워넣기
        foreach (var entity in detailMap)
        {
            extendedMap[entity.Key] = new List<RecordValue>();
            foreach (var fl in entity.Value)
            {
                var record = new RecordValue(fl.Brightness_Int, fl.color);
                extendedMap[entity.Key].Add(record);
            }
        }

        simplifyMap = new Dictionary<RecordKey, List<RecordValue>>(detailMap);

        var curver = new SimplifyCurver();

        if (isLinear) //Brightness가 linear로 뽑힐 때는 확장된map에서도 간소화 공식을 적용할 수 있다.
        {
            foreach (var entity in detailMap)
            {
                var transformedList = curver.ExcuteSimplify(RecordValue.GetFloatList(entity.Value), radicalThreshold, gradualThreshold, minimumFadeCount, IDuty.GetSavedDuties());

                if (null != transformedList)
                    simplifyMap[entity.Key] = RecordValue.GetIntToRecords(transformedList);
            }
        }
        else //선형적으로 하지 않으면 , originalMap에서 간소화를 하고, 간소화데이터를 확장하는 방법을 써야한다.
        {
            foreach (var entity in originalMap)
            {
                var transformedList = curver.ExcuteSimplify(RecordValue.GetFloatList(entity.Value), radicalThreshold, gradualThreshold, minimumFadeCount, IDuty.GetSavedDuties());

                if (null != transformedList)
                    simplifyMap[entity.Key] = RecordValue.GetIntToRecords(transformedList);
            }

            var temp = new Dictionary<RecordKey, List<RecordValue>>(simplifyMap);
            foreach (var entity in temp)
                simplifyMap[entity.Key] = GetExtendDataList(entity.Value, extendRatio);
        }
    }

    private List<RecordValue> GetExtendDataLinearList(List<RecordValue> inputs, float ratio, float gradualThreshold)
    {
        List<RecordValue> result = new List<RecordValue>();

        float cachedFloating = 0.0f;
        int repetitions = (int)ratio;
        float remainFloat = ratio - repetitions;

        //Do Linear Algo
        for (int ii = 0; ii < inputs.Count; ii++)
        {
            var value = inputs[ii];
            var nextValue = ii == inputs.Count - 1 ? value : inputs[ii + 1];

            //int subAve = Mathf.RoundToInt((nextValue.GetBrightness_Int - value.GetBrightness_Int + Definitions.LINEAR_INTERPOLATION_BETWEEN_ORIGIN_DATA_OFFSET) / repetitions);
            float subAve = ((float)nextValue.Brightness_Int - value.Brightness_Int) / repetitions;
            float addValue = value.Brightness_Int;

            for (int o = 0; o < repetitions; o++)
            {
                if (o == repetitions - 1)
                    addValue = _ClampMinMax(value.Brightness_Int + (subAve * o));
                else
                    addValue = value.Brightness_Int + (subAve * o);

                result.Add(new RecordValue(addValue, value.color));
            }

            cachedFloating += remainFloat;
            if (cachedFloating >= 1.0f)
            {
                addValue = _ClampMinMax(value.Brightness_Int + (subAve * repetitions));
                result.Add(new RecordValue(addValue, value.color));
                cachedFloating -= 1.0f;
            }

            //Clamp Last
            float _ClampMinMax(float input)
            {
                float result = input;
                if (subAve < 0)
                {
                    result = Mathf.Max(input, nextValue.Brightness_Int);
                    if (result == nextValue.Brightness_Int)
                        result += gradualThreshold;
                }
                else if (subAve > 0)
                {
                    result = Mathf.Min(input, nextValue.Brightness_Int);
                    if (result == nextValue.Brightness_Int)
                        result -= gradualThreshold;
                }

                return result;
            }
        }

        return result;
    }

    private List<RecordValue> GetExtendDataList(List<RecordValue> inputs, float ratio)
    {
        List<RecordValue> result = new List<RecordValue>();

        float cachedFloating = 0.0f;
        int repetitions = (int)ratio;
        float remainFloat = ratio - repetitions;

        //Not linear Algo..
        foreach (var value in inputs)
        {
            for (int i = 0; i < repetitions; i++)
            {
                result.Add(value);
            }

            cachedFloating += remainFloat;
            if (cachedFloating >= 1.0f)
            {
                result.Add(value);
                cachedFloating -= 1.0f;
            }
        }

        return result;
    }
}
