using System.Collections.Generic;
using System.Threading.Tasks;

public interface IExcelReader
{
    public class Data
    {
        public Dictionary<RecordKey, List<RecordValue>> simplifiedMap;
        public Dictionary<RecordKey, List<RecordValue>> originBrightnessMap;
        public Dictionary<RecordKey, List<RecordValue>> modifiedBrightnessMap;

        public ScenarioViewer.ScenarioInfo scenarioInfo;

        public List<int> duties;

        public Data(
            Dictionary<RecordKey, List<RecordValue>> simplifiedMap,
            Dictionary<RecordKey, List<RecordValue>> originBrightnessMap,
            Dictionary<RecordKey, List<RecordValue>> modifiedBrightnessMap,
            ScenarioViewer.ScenarioInfo scenarioInfo,
            List<int> duties)
        {
            this.simplifiedMap = simplifiedMap;
            this.scenarioInfo = scenarioInfo;
            this.originBrightnessMap = originBrightnessMap;
            this.modifiedBrightnessMap = modifiedBrightnessMap;
            this.duties = duties;
        }
    }

    Data ReadExcelData(string filePath);
    Task<Data> ReadExcelDataAsync(string filePath);
}

