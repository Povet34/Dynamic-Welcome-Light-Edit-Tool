using NPOI.SS.UserModel;
using System.Collections.Generic;
using UnityEngine;

public interface IExcelEditor
{
    void FirstWriteToExcel(ExcelEditData data);
    void AdditionalWriteExcel(ExcelEditData data);
    ISheet GetSheet(string sheetName);

    public struct ExcelEditData
    {
        //video Data
        public string filePath;
        public int frameCount;
        public float videoLength;
        public Vector2Int resolution;

        //Extract Data
        public RecordValue.RecordType type;
        public Dictionary<RecordKey, List<RecordValue>> recordedMap;
        public float bitConvertRatio;

        //Excel Data
        public string sheetName;
        public bool isModify;

        //Table Data
        public Dictionary<int, int> modeTable;
        public Dictionary<int, List<int>> timeTable;
        public Dictionary<int, List<int>> dutyTable;

        public void ChangeTypeAndSheetName(RecordValue.RecordType type, string sheetName, float bitConvertRatio)
        {
            this.type = type;
            this.sheetName = sheetName;
            this.bitConvertRatio = bitConvertRatio;
        }

        public void SetTables(Dictionary<int, int> mode, Dictionary<int, List<int>> time, Dictionary<int, List<int>> duty)
        {
            modeTable = mode;
            timeTable = time;
            dutyTable = duty;
        }

        public void ChangeRecordMap(Dictionary<RecordKey, List<RecordValue>> recordedMap)
        {
            this.recordedMap = recordedMap;
        }
    }
}

