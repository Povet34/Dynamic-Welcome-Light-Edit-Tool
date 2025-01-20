using System.Collections.Generic;
using UnityEngine;

public interface IPixelDataRecorder
{
    void ClearMap();
    void AddData(RecordKey key, RecordValue value);
    void CreateRecordFile(RecordFileInfo info);
}

public struct RecordFileInfo
{
    public string name;
    public float extendRatio;
    public float gradualThreshold;
    public float radicalThreshold;
    public int minimumFadeCount;
    public int bit;
    public float videoLength;
    public Vector2Int resolution;
    public bool isExtractColor;
    public bool isExtractBrightness;
    public bool isSaveBrightnessLinear;
}

/// <summary>
/// key로 저장될 정보.
/// Dic에서 index와 pos등, 한번의 영상 추출에서 바뀌지 않는 정보들이다. 
/// </summary>
public struct RecordKey
{
    public int index;
    public Vector2Int pos;

    public RecordKey(int index, Vector2Int pos)
    {
        this.index = index;
        this.pos = pos;
    }

    public bool IsSame(int index) => this.index == index; 
}

public struct RecordValue
{
    public enum RecordType
    {
        Brightness,
        Color,
        Details,
    }

    float brightness_Detail;
    public float Brightness_Decimal => brightness_Detail;
    public int Brightness_Int { get => (int)brightness_Detail; set => brightness_Detail = value; }
    public Color32 color;

    public RecordValue(float brightnessDetail)
    {
        brightness_Detail = brightnessDetail;
        color = Color.black;
    }

    public RecordValue(float brightnessDetail, Color32 color)
    {
        brightness_Detail = brightnessDetail;
        this.color = color;
    }

    public RecordValue(int brightness)
    {
        brightness_Detail = brightness;
        color = Color.black;
    }

    public RecordValue(Color32 color)
    {
        brightness_Detail = Definitions.GetBrightness255(color);
        this.color = color;
    }

    public RecordValue(int brightness, Color32 color)
    {
        brightness_Detail = brightness;
        this.color = color;
    }

    public static RecordValue Copy(RecordValue value)
    {
        var copied = new RecordValue(value.Brightness_Int, value.color);

        return copied;
    }

    public static List<float> GetFloatList(List<RecordValue> records)
    {
        List<float> result = new List<float>();
        foreach (var recordValue in records)
            result.Add(recordValue.brightness_Detail);

        return result;
    }

    public static List<int> GetIntList(List<RecordValue> records)
    {
        List<int> result = new List<int>();
        foreach (var recordValue in records)
            result.Add(recordValue.Brightness_Int);

        return result;
    }

    public static List<RecordValue> GetIntToRecords(List<int> brightnesses)
    {
        List<RecordValue> result = new List<RecordValue>();
        foreach (var brightness in brightnesses)
            result.Add(new RecordValue(brightness));

        return result;
    }

    public static List<Color32> GetColor32List(List<RecordValue> records)
    {
        List<Color32> result = new List<Color32>();
        foreach (var value in records)
            result.Add(value.color);

        return result;
    }


    public static List<RecordValue> GetColor32ToRecords(List<Color32> colors)
    {
        List<RecordValue> result = new List<RecordValue>();
        foreach (var color in colors)
            result.Add(new RecordValue(color));

        return result;
    }
}