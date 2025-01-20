using System.Collections.Generic;
using UnityEngine;

public static class BresenhamLineAlgorithm
{
    //How to use...
    //Texture2D texture = ...; // �ȼ� ���� ���� �ؽ�ó
    //var linePoints = BresenhamLineAlgorithm.GetLinePoints(10, 10, 150, 150);
    //foreach (var point in linePoints)
    //{
    //    Color color = texture.GetPixel(point.x, point.y);
    //    // ���⼭ color ������ ����Ͽ� �ʿ��� �۾� ����
    //}

    public static List<Vector2Int> GetLinePoints(int x0, int y0, int x1, int y1, int gap)
    {
        var points = new List<Vector2Int>();

        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;
        int counter = 0; // ���� ī����

        while (true)
        {
            if (counter % gap == 0) // ���ݿ� �������� ���� �ȼ� �߰�
            {
                points.Add(new Vector2Int(x0, y0));
            }

            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }

            counter++; // �� �ݺ����� ī���� ����
        }

        return points;
    }

    public static List<Color> GetGradientTransition(Texture2D texture, Vector2Int v0, Vector2Int v1, float threshold)
    {
        var linePoints = GetLinePoints(v0.x, v0.y, v1.x, v1.y, 1);
        List<Color> colorTransitions = new List<Color>();

        Color previousColor = texture.GetPixel(v0.x, v0.y);
        foreach (var point in linePoints)
        {
            Color currentColor = texture.GetPixel(point.x, point.y);
            if (ColorDifference(previousColor, currentColor) > threshold)
            {
                colorTransitions.Add(currentColor);
            }
            previousColor = currentColor;
        }

        return colorTransitions;
    }

    private static float ColorDifference(Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b);
    }
}