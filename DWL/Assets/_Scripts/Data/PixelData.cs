using System;
using UnityEngine;

[Serializable]
public struct PixelData
{
    public const float RED_WEIGHT = 0.299f;
    public const float GREEN_WEIGHT = 0.587f;
    public const float BLUE_WEIGHT = 0.114f;

    public int index;
    public Color32 color;
    public Vector2Int pos;

    public int brightness255 => (int)(RED_WEIGHT * color.r + GREEN_WEIGHT * color.g + BLUE_WEIGHT * color.b);
    public string GetIndex() => index.ToString("D2");

    public int recordTime;

    public PixelData(int index, Color color, Vector2Int pos, int recordTime)
    {
        this.index = index;
        this.color = color;
        this.pos = pos;
        this.recordTime = recordTime;
    }

    public void UpdateData(int index, Color color, Vector2Int pos, int recordTime)
    {
        this.index = index;
        this.color = color;
        this.pos = pos;
        this.recordTime = recordTime;
    }

    public string GetData()
    {
        string posInt = $"Position : ({pos.x},{pos.y})\n";
        string color255 = $"Color : ({color.r},{color.g},{color.b})\n";
        string brightness = $"Brightness : {brightness255}\n";

        return posInt + color255 + brightness;
    }

    public Color32 GetColor()
    {
        return color;
    }

    public string GetBrightness()
    {
        return $"{brightness255}\n";
    }

    public string GetPos()
    {
        return $"({pos.x},{pos.y})";
    }
}