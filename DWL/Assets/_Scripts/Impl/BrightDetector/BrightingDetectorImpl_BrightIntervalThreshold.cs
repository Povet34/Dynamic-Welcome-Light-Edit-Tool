using Cysharp.Threading.Tasks;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.VideoioModule;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BrightingDetectorImpl_BrightIntervalThreshold : IBrightingDetector
{
    private Mat minBrightness;
    private Mat maxBrightness;
    private Mat filtered;

    public async UniTask<DetectedInfo> ProcessVideoAsync(IBrightingDetector.DetectingParameter param)
    {
        VideoCapture capture = new VideoCapture(param.videoPath);

        if (!capture.isOpened())
        {
            param.errorCallback?.Invoke();
        }

        Mat frame = new Mat();
        int frameBatchSize = 5; // 한 번에 처리할 프레임 수
        while (true)
        {
            List<UniTask> batchTasks = new List<UniTask>();
            for (int i = 0; i < frameBatchSize; i++)
            {
                if (!capture.read(frame))
                {
                    break;
                }
                Mat frameCopy = frame.clone();
                batchTasks.Add(UpdateBrightnessAsync(frameCopy));
            }

            if (batchTasks.Count == 0)
            {
                break;
            }

            await UniTask.WhenAll(batchTasks);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        capture.release();

        //Contour의 상태를 확인하고 싶을 때, 사용함
        //GameObject contour = new GameObject("contour", typeof(RawImage));
        //contour.GetComponent<RawImage>().texture = FilterPixels(brightnessIntervalThreshold);
        //contour.transform.SetParent(GameObject.FindFirstObjectByType<Canvas>().transform);

        return new DetectedInfo(FilterPixels(param.brightInterval), GetPointChannelsInContours(filtered, param.pixelSpacing, param.shapeWallThreshold), GetContours());
    }

    private async UniTask UpdateBrightnessAsync(Mat frame)
    {
        await UniTask.SwitchToThreadPool();

        if (minBrightness == null || maxBrightness == null)
        {
            minBrightness = new Mat(frame.size(), CvType.CV_8UC1, new Scalar(255));
            maxBrightness = new Mat(frame.size(), CvType.CV_8UC1, new Scalar(0));
        }

        Mat grayFrame = new Mat();
        Imgproc.cvtColor(frame, grayFrame, Imgproc.COLOR_BGR2GRAY);

        Core.min(minBrightness, grayFrame, minBrightness);
        Core.max(maxBrightness, grayFrame, maxBrightness);
    }

    public Texture2D FilterPixels(int brightnessIntervalThreshold)
    {
        if (minBrightness == null || maxBrightness == null)
        {
            throw new Exception("밝기 데이터가 초기화되지 않았습니다.");
        }

        int width = minBrightness.cols();
        int height = minBrightness.rows();

        filtered = new Mat(height, width, CvType.CV_8UC1, new Scalar(0));

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double minVal = minBrightness.get(y, x)[0];
                double maxVal = maxBrightness.get(y, x)[0];
                if (maxVal - minVal >= brightnessIntervalThreshold)
                {
                    filtered.put(y, x, new double[] { 255 });
                }
            }
        }

        Texture2D texture = new Texture2D(filtered.cols(), filtered.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(filtered, texture);
        return texture;
    }

    #region Extract Infos 

    /// <summary>
    /// 컨투어의 종횡비를 보고, 선분(Start, End)인지 점(Center)인지 판단하여 점을 구하고 그 점의 리스트를 반환한다.
    /// </summary>
    /// <param name="filtered"></param>
    /// <returns></returns>
    private List<BrightChannelInfo> GetChannelInosAnalyzeContours(Mat filtered, bool isCreateDetectedPixcelContainLines)
    {
        List<BrightChannelInfo> infos = new List<BrightChannelInfo>();
        List<MatOfPoint> contours = new List<MatOfPoint>();

        Imgproc.findContours(filtered, contours, new Mat(), Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE);

        foreach (var contour in contours)
        {
            OpenCVForUnity.CoreModule.Rect boundingRect = Imgproc.boundingRect(new MatOfPoint2f(contour.toArray()));

            if (boundingRect.width < 5 || boundingRect.height < 5)
            {
                continue;
            }

            float aspectRatio = (float)boundingRect.width / boundingRect.height;

            if (aspectRatio < 0.5 || aspectRatio > 2)
            {
                if (!isCreateDetectedPixcelContainLines)
                    continue;

                float length = CalculateContourLength(contour);
                Vector2 direction = DetermineDiagonalDirection(contour);

                float magnitude = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
                Vector2 normalizedDirection = new Vector2(direction.x / magnitude, direction.y / magnitude);

                Vector2 center = new Vector2(boundingRect.x + boundingRect.width / 2, boundingRect.y + boundingRect.height / 2);

                Vector2Int point1 = new Vector2Int((int)(center.x - normalizedDirection.x * length / 2), (int)(center.y - normalizedDirection.y * length / 2));
                Vector2Int point2 = new Vector2Int((int)(center.x + normalizedDirection.x * length / 2), (int)(center.y + normalizedDirection.y * length / 2));

                var info = new BrightChannelInfo(ePixelChannelType.Segment, new List<Vector2>() { point1, point2 });

                infos.Add(info);
            }
            else
            {
                var info = new BrightChannelInfo(ePixelChannelType.Point, new Vector2Int(boundingRect.x + boundingRect.width / 2, boundingRect.y + boundingRect.height / 2));

                infos.Add(info);
            }
        }

        return infos;
    }

    private List<BrightChannelInfo> GetPointChannelsInContours(Mat filtered, int pixelSpacing, float shapeWallThreshold)
    {
        List<BrightChannelInfo> infos = new List<BrightChannelInfo>();
        List<MatOfPoint> contours = new List<MatOfPoint>();

        Imgproc.findContours(filtered, contours, new Mat(), Imgproc.RETR_TREE, Imgproc.CHAIN_APPROX_SIMPLE);

        foreach (var contour in contours)
        {
            Point[] points = contour.toArray();

            // Find bounding box of the shape
            OpenCVForUnity.CoreModule.Rect boundingRect = Imgproc.boundingRect(new MatOfPoint2f(points));

            // Sample points inside the bounding box using Bresenham's algorithm
            for (int x = boundingRect.x; x < boundingRect.x + boundingRect.width; x += pixelSpacing)
            {
                for (int y = boundingRect.y; y < boundingRect.y + boundingRect.height; y += pixelSpacing)
                {
                    if (IsPointInsideShape(x, y, points, shapeWallThreshold))
                    {
                        //Debug.Log("Sampled point inside shape: " + x + ", " + y);
                        infos.Add(new BrightChannelInfo(type: ePixelChannelType.Point, new Vector2Int(x, y)));
                    }
                }
            }
        }

        return infos;
    }

    #endregion

    private List<MatOfPoint> GetContours()
    {
        List<Vector2Int> points = new List<Vector2Int>();
        List<MatOfPoint> contours = new List<MatOfPoint>();

        Imgproc.findContours(filtered, contours, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

        return contours;
    }

    /// <summary>
    /// 컨투어의 방향을 구한다.
    /// </summary>
    /// <param name="contour"></param>
    /// <returns></returns>
    private Vector2 DetermineDiagonalDirection(MatOfPoint contour)
    {
        MatOfPoint2f contourPoints = new MatOfPoint2f(contour.toArray());

        Mat line = new Mat();
        Imgproc.fitLine(contourPoints, line, Imgproc.DIST_L2, 0, 0.01, 0.01);

        float vx = (float)line.get(0, 0)[0];
        float vy = (float)line.get(1, 0)[0];

        return new Vector2(vx, vy);
    }

    /// <summary>
    /// 컨투어의 길이를 구한다.
    /// </summary>
    /// <param name="contour"></param>
    /// <returns></returns>
    public float CalculateContourLength(MatOfPoint contour)
    {
        MatOfPoint2f contour2f = new MatOfPoint2f(contour.toArray());
        double length = Imgproc.arcLength(contour2f, true) * 0.5f;
        return (float)length;
    }

    private bool IsPointInsideShape(int x, int y, Point[] points, float shapeWallThreshold)
    {
        // Ray casting algorithm to check if a point is inside a polygon
        bool inside = false;
        for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
        {
            // Calculate the distance from the point to the current line segment
            double distance = PointToLineDistance(new Point(x, y), points[i], points[j]);

            // If the distance is within the threshold, consider the point as lying on the edge
            if (distance <= shapeWallThreshold)
                return false;

            if (((points[i].y > y) != (points[j].y > y)) &&
                (x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private double PointToLineDistance(Point pt, Point lineStart, Point lineEnd)
    {
        double dx = lineEnd.x - lineStart.x;
        double dy = lineEnd.y - lineStart.y;
        double lengthSquared = dx * dx + dy * dy;
        double u = ((pt.x - lineStart.x) * dx + (pt.y - lineStart.y) * dy) / lengthSquared;

        if (u > 1)
            u = 1;
        else if (u < 0)
            u = 0;

        double closestX = lineStart.x + u * dx;
        double closestY = lineStart.y + u * dy;
        double distanceX = pt.x - closestX;
        double distanceY = pt.y - closestY;
        return Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
    }
}