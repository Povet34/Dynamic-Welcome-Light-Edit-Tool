using System.Collections.Generic;
using UnityEngine;

public class TextDB
{
    [System.Serializable]
    public class DataListWrapper
    {
        public List<Data> datas;
    }

    [System.Serializable]
    public class Data
    {
        public string key;
        public string value;

        public Data(string key, string value)
        {
            this.key = key ;
            this.value = value;
        }
    }

    Dictionary<string, string> textDictionary;

    void InitializeTextDB()
    {
        TextAsset jsonTextAsset = Resources.Load<TextAsset>("TextDB");

        if (jsonTextAsset != null)
        {
            DataListWrapper wrapper = JsonUtility.FromJson<DataListWrapper>(jsonTextAsset.text);
            textDictionary = new Dictionary<string, string>();

            for (int i = 0; i < wrapper.datas.Count; i++)
            {
                textDictionary.Add(wrapper.datas[i].key, wrapper.datas[i].value);
            }
        }
        else
        {
            Debug.LogError("Failed to load JSON file from Resources.");
        }
    }

    public string GetText(string key)
    {
        if (null == textDictionary)
            InitializeTextDB();

        if (textDictionary.ContainsKey(key))
            return textDictionary[key];

        return null;
    }
}
