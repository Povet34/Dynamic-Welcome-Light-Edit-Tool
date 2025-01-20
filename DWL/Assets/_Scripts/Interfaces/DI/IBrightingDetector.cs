using Cysharp.Threading.Tasks;
using OpenCVForUnity.CoreModule;
using System;
using System.Collections.Generic;
using UnityEngine;
public enum ePixelChannelType
{
    Point,
    Segment,
}

public interface IBrightingDetector
{
    public struct DetectingParameter
    {
        public string videoPath;
        public bool isCreateDetectedPixcelContainLines;
        public int brightInterval;
        public int pixelSpacing;
        public float shapeWallThreshold;

        public Action errorCallback;
    }

    UniTask<DetectedInfo> ProcessVideoAsync(DetectingParameter param);
}

public class BrightChannelInfo
{
    public ePixelChannelType type;
    public List<Vector2> points;

    public BrightChannelInfo() { }

    public BrightChannelInfo(ePixelChannelType type, List<Vector2> points)
    {
        this.type = type;
        this.points = points;
    }

    public BrightChannelInfo(ePixelChannelType type, Vector2Int point)
    {
        this.type = type;
        this.points = new List<Vector2>() { point };
    }

    public void SetPoints(List<Vector2> points)
    {
        this.points = points;
    }

    public void SetPoint(Vector2 point)
    {
        points = new List<Vector2> { point };
    }
}

public class DetectedInfo
{
    public Texture2D texture;
    public List<BrightChannelInfo> centers;
    public List<MatOfPoint> contours;

    public DetectedInfo() { }

    public DetectedInfo(Texture2D texture, List<BrightChannelInfo> centers, List<MatOfPoint> contours)
    {
        this.texture = texture;
        this.centers = centers;
        this.contours = contours;
    }

    public void SortByPositionX()
    {
        if (null != centers)
        {
            centers.Sort((a, b) =>
            {
                if (a.points.Count == 0 || b.points.Count == 0)
                {
                    throw new InvalidOperationException("points 리스트는 비어있을 수 없습니다.");
                }

                int compareX = a.points[0].x.CompareTo(b.points[0].x);
                if (compareX != 0)
                {
                    return compareX;
                }
                else
                {
                    return a.points[0].y.CompareTo(b.points[0].y);
                }
            });
        }
    }
}