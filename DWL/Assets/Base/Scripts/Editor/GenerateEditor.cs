using System.Data;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using ExcelDataReader;

public partial class GenerateEditor : EditorWindow
{
    private enum eToolbarType
    {
        None = -1,
        Data,
        UI,
    }

    static Color BASIC_TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f);
    static Color TIP_TEXT_COLOR = new Color(1f, 0.6f, 0.5f);

    int tabIndex = -1;
    string[] tabSubjectArr = { "Data", "UI" };

    [MenuItem("Tools/GenerateEditor")]
    static void ShowWindow()
    {
        GenerateEditor window = (GenerateEditor)EditorWindow.GetWindow(typeof(GenerateEditor));
        window.maxSize = window.minSize = new Vector2(450, 600);
        window.Show();
    }

    private void OnGUI()
    {
        // 실제 창 코드는 여기에 표시됩니다 (프레임 마다)
        tabIndex = GUILayout.Toolbar(tabIndex, tabSubjectArr);
        switch ((eToolbarType)tabIndex)
        {
            case eToolbarType.None:
                break;
            case eToolbarType.Data:
                ExecuteDataTableExcelToCsv();
                ExecuteEnumTable();
                ExecuteDataTable();
                ExcuteDataManager();
                break;
            case eToolbarType.UI:
                GenerateUI();
                break;
        }
    }

    #region Data Table Excel > CSV : ------------------------------------------
    void ExecuteDataTableExcelToCsv()
    {
        AddTip("[ConvertDataTableExcelToCsv]");
        AddTip("Data Table Excel 전체 파일을 CSV 파일로 변환");
        EditorGUILayout.BeginHorizontal();

        foreach (var devEnv in Define.DEV_ENV_ARR)
        {
            if (GUILayout.Button(devEnv, GUILayout.Width(80)))
            {
                FileInfo[] fileInfoArr = LoadAllExcelFileInfo(devEnv);
                foreach (FileInfo fileInfo in fileInfoArr)
                {
                    if (fileInfo.Name.Equals(Define.DATA_TABLE_NAME))
                        ConvertDataTableExcelToCsv(devEnv, fileInfo);
                }
            }
        }

        if (GUILayout.Button("All", GUILayout.Width(80)))
        {
            foreach (var devEnv in Define.DEV_ENV_ARR)
            {
                FileInfo[] fileInfoArr = LoadAllExcelFileInfo(devEnv);
                foreach (FileInfo fileInfo in fileInfoArr)
                {
                    if (fileInfo.Name.Equals(Define.DATA_TABLE_NAME))
                        ConvertDataTableExcelToCsv(devEnv, fileInfo);
                }
            }
        }
        
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //AssetDatabase.Refresh();
    }

    void ConvertDataTableExcelToCsv(string devEnv, FileInfo fileInfo)
    {
        using (var reader = ExcelReaderFactory.CreateReader(fileInfo.Open(FileMode.Open, FileAccess.ReadWrite)))
        {
            var result = reader.AsDataSet();

            string directoryPath = Define.GetDataFilePath(devEnv);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            // 시트 개수만큼 반복
            StringBuilder sb = new StringBuilder();
            for (int tableIndex = 0; tableIndex < result.Tables.Count; tableIndex++)
            {
                string fileFullPath = Path.Combine(directoryPath, result.Tables[tableIndex].ToString()) + Define.EXTENSION_CSV;
                FileStream fs = File.Create(fileFullPath);
                StreamWriter sw = new StreamWriter(fs);

                for (int rowIndex = 0; rowIndex < result.Tables[tableIndex].Rows.Count; rowIndex++)
                {
                    sb.Clear();
                    for (int columnIndex = 0; columnIndex < result.Tables[tableIndex].Columns.Count; columnIndex++)
                    {
                        if (!string.IsNullOrEmpty(result.Tables[tableIndex].Rows[rowIndex][columnIndex].ToString()))
                        {
                            sb.Append(result.Tables[tableIndex].Rows[rowIndex][columnIndex].ToString());
                            sb.Append(",");
                        }
                    }

                    sw.WriteLine(sb.ToString());
                }

                sw.Close();
                fs.Close();
            }
        }
    }
    #endregion

    #region Generate Enum Table : ---------------------------------------------
    void ExecuteEnumTable()
    {
        AddTip("[GenerateEnumTable]");
        AddTip("enums_table.xlsx 전체 파일을 GenerateEnumTable.cs 파일 enum으로 변환");
        EditorGUILayout.BeginHorizontal();

        foreach (var devEnv in Define.DEV_ENV_ARR)
        {
            if (GUILayout.Button(devEnv, GUILayout.Width(80)))
            {
                FileInfo[] fileInfoArr = LoadAllExcelFileInfo(devEnv);
                foreach (FileInfo fileInfo in fileInfoArr)
                {
                    if (fileInfo.Name.Equals(Define.ENUMS_TABLE_NAME))
                        GenerateEnumTable(fileInfo);
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //AssetDatabase.Refresh();
    }

    void GenerateEnumTable(FileInfo fileInfo)
    {
        using (var reader = ExcelReaderFactory.CreateReader(fileInfo.Open(FileMode.Open, FileAccess.ReadWrite)))
        {
            DataSet dataSet = reader.AsDataSet();
            if (null != dataSet && null != dataSet.Tables && dataSet.Tables.Count > 0)
            {
                StringBuilder generateCode = new StringBuilder();
                StringBuilder enumContent = new StringBuilder();
                DataTableCollection tables = dataSet.Tables;
                for (int tableIndex = 0; tableIndex < tables.Count; tableIndex++)
                {
                    for (int rowIndex = 0; rowIndex < tables[tableIndex].Rows.Count; rowIndex++)
                    {
                        // 1행은 Name, Value, Desc 2행은 1행에 대한 설명으로 정의 (변경 되면 코드 변경 필요)
                        if (rowIndex >= 2)
                        {
                            for (int columnIndex = 0; columnIndex < tables[tableIndex].Columns.Count; columnIndex++)
                            {
                                if (!string.IsNullOrEmpty(tables[tableIndex].Rows[rowIndex][columnIndex].ToString()))
                                {
                                    if (tables[tableIndex].Rows[0][columnIndex].ToString().Equals("Name"))
                                    {
                                        enumContent.Append("\t");
                                        enumContent.Append(tables[tableIndex].Rows[rowIndex][columnIndex].ToString());
                                        enumContent.Append("\t");
                                        enumContent.Append("=");
                                        enumContent.Append(" ");
                                    }

                                    if (tables[tableIndex].Rows[0][columnIndex].ToString().Equals("Value"))
                                    {
                                        enumContent.Append(tables[tableIndex].Rows[rowIndex][columnIndex].ToString());
                                        enumContent.Append(",");
                                        enumContent.Append("\t");
                                        enumContent.Append("//");
                                    }

                                    if (tables[tableIndex].Rows[0][columnIndex].ToString().Equals("Desc"))
                                    {
                                        enumContent.Append(" ");
                                        enumContent.Append(tables[tableIndex].Rows[rowIndex][columnIndex].ToString());

                                        if (tables[tableIndex].Rows.Count - 1 > rowIndex)
                                            enumContent.Append("\r\n");
                                    }
                                }
                            }
                        }
                    }

                    string enumName = tables[tableIndex].ToString();
                    // 아래 code 해당 위치 변경 시 실제 생성 되는 코드 위치에 영향을 준다.
                    string code = 
$@"public enum {enumName}
{{
{enumContent}
}}

";
                    enumContent.Clear();
                    generateCode.Append(code);
                }

                File.WriteAllText(Define.GENERATE_ENUM_TABLE_PATH, generateCode.ToString(), Encoding.UTF8);
                generateCode.Clear();
            }
        }
    }
    #endregion

    #region Generate Data Table : ---------------------------------------------
    void ExecuteDataTable()
    {
        AddTip("[GenerateDataTable]");
        AddTip("data_table.xlsx 전체 파일을 GenerateDataTable.cs 파일 class 변환");
        EditorGUILayout.BeginHorizontal();
        
        // dev 기준 table 생성
        string envDev = Define.DEV_ENV_ARR[0];
        if (GUILayout.Button(envDev, GUILayout.Width(80)))
        {
            FileInfo[] fileInfoArr = LoadAllExcelFileInfo(envDev);
            foreach (FileInfo fileInfo in fileInfoArr)
            {
                if (fileInfo.Name.Equals(Define.DATA_TABLE_NAME))
                    GenerateDataTable(fileInfo);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //AssetDatabase.Refresh();
    }

    void GenerateDataTable(FileInfo fileInfo)
    {
        using (var reader = ExcelReaderFactory.CreateReader(fileInfo.Open(FileMode.Open, FileAccess.ReadWrite)))
        {
            DataSet dataSet = reader.AsDataSet();
            if (null != dataSet && null != dataSet.Tables && dataSet.Tables.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                
                for (int tableIndex = 0; tableIndex < dataSet.Tables.Count; tableIndex++)
                {
                    string code = GenerateDataTableContent(dataSet.Tables[tableIndex]);
                    sb.Append(code);
                }

                File.WriteAllText(Define.GENERATE_DATA_TABLE_PATH, sb.ToString(), Encoding.UTF8);
                sb.Clear();
            }
        }
    }

    string GenerateDataTableContent(DataTable dataTable)
    {
        if (null != dataTable)
        {
            StringBuilder contents = new StringBuilder();
            for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
            {
                // 1행은 네이밍, 2행은 설명, 3행은 타입으로  정의 (변경 되면 코드 변경 필요)
                if (!string.IsNullOrEmpty(dataTable.Rows[2][columnIndex].ToString()))
                {
                    contents.Append("\t");
                    contents.Append("public");
                    contents.Append(" ");
                    contents.Append(dataTable.Rows[2][columnIndex].ToString());
                    contents.Append(" ");
                }

                if (!string.IsNullOrEmpty(dataTable.Rows[0][columnIndex].ToString()))
                {
                    contents.Append(dataTable.Rows[0][columnIndex].ToString());
                    contents.Append(";");

                    if (!string.IsNullOrEmpty(dataTable.Rows[2][columnIndex].ToString())
                        && !string.IsNullOrEmpty(dataTable.Rows[0][columnIndex].ToString()))
                    {
                        int length = dataTable.Rows[2][columnIndex].ToString().Length + dataTable.Rows[0][columnIndex].ToString().Length;
                        if (length >= 4 && length < 8)
                            contents.Append("\t\t\t\t\t\t");
                        else if (length >= 8 && length < 12)
                            contents.Append("\t\t\t\t\t");
                        else if (length >= 12 && length < 16)
                            contents.Append("\t\t\t\t");
                        else if (length >= 16 && length < 20)
                            contents.Append("\t\t\t");
                        else if (length >= 20 && length < 24)
                            contents.Append("\t\t");
                        else if (length >= 24)
                            contents.Append("\t");
                    }
                    else
                        contents.Append("\t");

                    contents.Append("//");
                    contents.Append(" ");
                }

                if (!string.IsNullOrEmpty(dataTable.Rows[1][columnIndex].ToString()))
                {
                    contents.Append(dataTable.Rows[1][columnIndex].ToString());

                    if (dataTable.Columns.Count - 1 > columnIndex)
                        contents.Append("\r\n");
                }
            }

            string className = dataTable.ToString();
            // 아래 code 해당 위치 변경 시 실제 생성 되는 코드 위치에 영향을 준다.
            string generateCode =
$@"public class {className}
{{
{contents}
}}

";
            return generateCode;
        }

        return string.Empty;
    }
    #endregion

    #region Generate Data Manager : -------------------------------------------
    void ExcuteDataManager()
    {
        AddTip("[GenerateDataManager]");
        AddTip("data_table.xlsx 전체 파일을 GenerateDataManager.cs 파일 class 변환");
        AddTip("DataManager.cs 사용할 데이터 생성(GenerateDataTable 생성 후 가능)");
        EditorGUILayout.BeginHorizontal();

        // dev 기준 table 생성
        string envDev = Define.DEV_ENV_ARR[0];
        if (GUILayout.Button(envDev, GUILayout.Width(80)))
        {
            FileInfo[] fileInfoArr = LoadAllExcelFileInfo(envDev);
            foreach (FileInfo fileInfo in fileInfoArr)
            {
                if (fileInfo.Name.Equals(Define.DATA_TABLE_NAME))
                    GenerateDataManager(fileInfo);
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        //AssetDatabase.Refresh();
    }

    void GenerateDataManager(FileInfo fileInfo)
    {
        using (var reader = ExcelReaderFactory.CreateReader(fileInfo.Open(FileMode.Open, FileAccess.ReadWrite)))
        {
            DataSet dataSet = reader.AsDataSet();
            if (null != dataSet && null != dataSet.Tables && dataSet.Tables.Count > 0)
            {
                StringBuilder sbDicts = new StringBuilder();
                StringBuilder sbMethodNames = new StringBuilder();
                StringBuilder sbMethods = new StringBuilder();
                
                for (int tableIndex = 0; tableIndex < dataSet.Tables.Count; tableIndex++)
                {
                    if (tableIndex != 0)
                        sbMethodNames.Append("\t\t\t");

                    string tableName = dataSet.Tables[tableIndex].ToString();
                    string pureName = tableName.Remove(0, 1);
                    sbMethodNames.Append($@"Load{pureName}Data();");
                    if (tableIndex < dataSet.Tables.Count - 1)
                        sbMethodNames.Append("\r\n");

                    sbDicts.Append(GenerateDataManagerDict(dataSet.Tables[tableIndex]));
                    sbMethods.Append(GenerateDataManagerMethod(dataSet.Tables[tableIndex]));
                }

                string generateCode =
$@"namespace Geuchiman.Framework
{{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class DataManager : AbstractSingleton<DataManager>
    {{
        {sbDicts}
    
        void LoadDataTable()
        {{
            {sbMethodNames}
        }}

        {sbMethods}
    }}
}}";
                File.WriteAllText(Define.GENERATE_DATA_MANAGER_PATH, generateCode, Encoding.UTF8);
                sbMethods.Clear();
                sbMethodNames.Clear();
                sbDicts.Clear();
            }
        }
    }

    string GenerateDataManagerDict(DataTable dataTable)
    {
        if (null != dataTable)
        {
            string className = dataTable.ToString();
            string dictName = className.Remove(0, 1).ToLower() + "Dict";
            return string.Format("Dictionary<int, {0}> {1} = new Dictionary<int, {0}>();\r\n", className, dictName);
        }

        return string.Empty;
    }

    string GenerateDataManagerMethod(DataTable dataTable)
    {
        if (null != dataTable)
        {
            StringBuilder sb = new StringBuilder();
            for (int columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
            {
                // 1행은 네이밍, 2행은 설명, 3행은 타입으로  정의 (변경 되면 코드 변경 필요)
                if (!string.IsNullOrEmpty(dataTable.Rows[2][columnIndex].ToString())
                    && !string.IsNullOrEmpty(dataTable.Rows[0][columnIndex].ToString()))
                {
                    if (columnIndex != 0)
                        sb.Append("\t\t\t\t\t");
                    string type = dataTable.Rows[2][columnIndex].ToString();
                    if (type.Equals("int"))
                        sb.Append($@"info.{dataTable.Rows[0][columnIndex]} = ParseInt(propArr, index++);");
                    else if (type.Equals("long"))
                        sb.Append($@"info.{dataTable.Rows[0][columnIndex]} = ParseLong(propArr, index++);");
                    else if (type.Equals("float"))
                        sb.Append($@"info.{dataTable.Rows[0][columnIndex]} = ParseFloat(propArr, index++);");
                    else if (type.Equals("string"))
                        sb.Append($@"info.{dataTable.Rows[0][columnIndex]} = ParseString(propArr, index++);");
                    else if (type.StartsWith("E"))
                        sb.Append($@"info.{dataTable.Rows[0][columnIndex]} = ({type})ParseInt(propArr, index++);");
                    
                    sb.Append("\r\n");
                }
            }

            string tableName = dataTable.ToString();
            string pureName = tableName.Remove(0, 1);
            string dictName = tableName.Remove(0, 1).ToLower() + "Dict";

            string methodCode = 
        $@"void Load{pureName}Data()
        {{
            {dictName}.Clear();
            string data = dataAssetList.GetTextData(""{tableName}"");
            if (null != data)
            {{
                string[] lineArr = data.Split(new char[] {{ '\n' }});
			    currentHeader = lineArr[0].Split(new char[] {{ CHAR_DELIMITER }});
                for (int lineIndex = 2; lineIndex < lineArr.Length; lineIndex++)
                {{
                    string[] propArr = lineArr[lineIndex].Split(new char[] {{ CHAR_DELIMITER }});
                    if (propArr.Length <= 1)
                        continue;

                    int index = 0;
                    {tableName} info = new {tableName}();
                    {sb}
#if UNITY_EDITOR
                    if ({dictName}.ContainsKey(info.ID))
                        BLogger.Instance.LogError(""{tableName} - duplicated ID"" + info.ID);
#endif
                    {dictName}[info.ID] = info;
                }}
            }}
        }}
        ";
            string getCode =
        $@"public Dictionary<int, {tableName}> Get{dictName}()
        {{
            return {dictName};
        }}

        public {tableName} Get{pureName}Data(int id)
        {{
            {tableName} info = null;
            if ({dictName}.TryGetValue(id, out info))
                return info;

            return null;
        }}";

            string generateCode =
        $@"#region {tableName}
        {methodCode}
        {getCode}
        #endregion";

            return generateCode;
        }

        return string.Empty;
    }
    #endregion

    FileInfo[] LoadAllExcelFileInfo(string devEnv)
    {
        string directoryPath = Define.GetDataSheetPath(devEnv);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (null != directoryInfo)
        {
            FileInfo[] fileInfoArr = directoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
            return fileInfoArr;
        }

        return null;
    }

    FileInfo[] LoadAllCsvFileInfo(string devEnv)
    {
        string directoryPath = Define.GetDataFilePath(devEnv);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
        if (null != directoryInfo)
        {
            FileInfo[] fileInfoArr = directoryInfo.GetFiles("*.csv", SearchOption.AllDirectories);
            return fileInfoArr;
        }

        return null;
    }

    static void AddTip(string text)
    {
        GUI.color = TIP_TEXT_COLOR;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(text);
        EditorGUILayout.EndHorizontal();
        GUI.color = BASIC_TEXT_COLOR;
    }
}