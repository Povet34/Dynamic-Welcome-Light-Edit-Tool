using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestContour : MonoBehaviour
{
    public int pixelSpacing = 1; // Interval for sampling points
    public float cubeSize = 1f;
    public float shapeWallThreshold = 1f;
    public bool isOnlyTest = false;
    public int count;

    List<MatOfPoint> contours;
    List<Point> insidePoints;

    public void SetContours(List<MatOfPoint> contours)
    {
#if UNITY_EDITOR

        this.contours = contours;
        insidePoints = new List<Point>();

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
                    if (IsPointInsideShape(x, y, points))
                    {
                        //Debug.Log("Sampled point inside shape: " + x + ", " + y);
                        insidePoints.Add(new Point(x, y));
                    }
                }
            }
        }
#endif
    }

    private bool IsPointInsideShape(int x, int y, Point[] points)
    {
        // Ray casting algorithm to check if a point is inside a polygon
        bool inside = false;
        for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
        {
            // Check if the point is on the line segment
            if (Math.Abs(points[i].y - points[j].y) < 0.00001 && Math.Abs(points[i].y - y) < 0.00001 &&
                ((points[i].x <= x && x <= points[j].x) || (points[j].x <= x && x <= points[i].x)))
            {
                return false; // Point is on the boundary
            }

            if (((points[i].y > y) != (points[j].y > y)) &&
                (x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    private void OnDrawGizmos()
    {
        if (null == contours) return;

        foreach (var contour in contours)
        {
            Point[] points = contour.toArray();
            for (int i = 0; i < points.Length - 1; i++)
            {
                Gizmos.DrawLine(new Vector3((float)points[i].x, (float)points[i].y, 0),
                                 new Vector3((float)points[i + 1].x, (float)points[i + 1].y, 0));
            }

            // Connect the last and first points to complete the loop
            Gizmos.DrawLine(new Vector3((float)points[points.Length - 1].x, (float)points[points.Length - 1].y, 0),
                             new Vector3((float)points[0].x, (float)points[0].y, 0));
        }

        count = insidePoints.Count;

        foreach (var point in insidePoints)
        {
            Gizmos.DrawCube(new Vector3((float)point.x, (float)point.y, 0f), Vector3.one * cubeSize);
        }
    }
}

//public class TestContour : MonoBehaviour
//{
//    public int pixelSpacing = 1; // Interval for sampling points
//    public float cubeSize = 1f;
//    public float shapeWallThreshold = 1f;
//    public bool isOnlyTest = false;
//    public int count;

//    List<MatOfPoint> contours;
//    List<Point> insidePoints;

//    public void SetContours(List<MatOfPoint> contours)
//    {
//#if UNITY_EDITOR

//        this.contours = contours;
//        insidePoints = new List<Point>();

//        foreach (var contour in contours)
//        {
//            Point[] points = contour.toArray();

//            // Find bounding box of the shape
//            OpenCVForUnity.CoreModule.Rect boundingRect = Imgproc.boundingRect(new MatOfPoint2f(points));

//            // Sample points inside the bounding box using Bresenham's algorithm
//            for (int x = boundingRect.x; x < boundingRect.x + boundingRect.width; x += pixelSpacing)
//            {
//                for (int y = boundingRect.y; y < boundingRect.y + boundingRect.height; y += pixelSpacing)
//                {
//                    if (IsPointInsideShape(x, y, points))
//                    {
//                        //Debug.Log("Sampled point inside shape: " + x + ", " + y);
//                        insidePoints.Add(new Point(x, y));
//                    }
//                }
//            }
//        }
//#endif
//    }

//    private bool IsPointInsideShape(int x, int y, Point[] points)
//    {
//        // Ray casting algorithm to check if a point is inside a polygon
//        bool inside = false;
//        for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
//        {
//            // Calculate the distance from the point to the current line segment
//            double distance = PointToLineDistance(new Point(x, y), points[i], points[j]);

//            // If the distance is within the threshold, consider the point as lying on the edge
//            if (distance <= shapeWallThreshold)
//                return false;

//            if (((points[i].y > y) != (points[j].y > y)) &&
//                (x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
//            {
//                inside = !inside;
//            }
//        }
//        return inside;
//    }

//    private double PointToLineDistance(Point pt, Point lineStart, Point lineEnd)
//    {
//        double dx = lineEnd.x - lineStart.x;
//        double dy = lineEnd.y - lineStart.y;
//        double lengthSquared = dx * dx + dy * dy;
//        double u = ((pt.x - lineStart.x) * dx + (pt.y - lineStart.y) * dy) / lengthSquared;

//        if (u > 1)
//            u = 1;
//        else if (u < 0)
//            u = 0;

//        double closestX = lineStart.x + u * dx;
//        double closestY = lineStart.y + u * dy;
//        double distanceX = pt.x - closestX;
//        double distanceY = pt.y - closestY;
//        return Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
//    }

//    private void OnDrawGizmos()
//    {
//        if (null == contours) return;

//        foreach (var contour in contours)
//        {
//            Point[] points = contour.toArray();
//            for (int i = 0; i < points.Length - 1; i++)
//            {
//                Gizmos.DrawLine(new Vector3((float)points[i].x, (float)points[i].y, 0),
//                                 new Vector3((float)points[i + 1].x, (float)points[i + 1].y, 0));
//            }

//            // Connect the last and first points to complete the loop
//            Gizmos.DrawLine(new Vector3((float)points[points.Length - 1].x, (float)points[points.Length - 1].y, 0),
//                             new Vector3((float)points[0].x, (float)points[0].y, 0));
//        }

//        count = insidePoints.Count;

//        foreach (var point in insidePoints)
//        {
//            Gizmos.DrawCube(new Vector3((float)point.x, (float)point.y, 0f), Vector3.one * cubeSize);
//        }
//    }
//}