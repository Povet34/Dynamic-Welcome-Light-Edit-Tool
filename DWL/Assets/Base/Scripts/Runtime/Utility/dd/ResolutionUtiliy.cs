using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace ResolutionUtiliy 
{
    public struct ResolutionInfo
    {
        public Vector2 centerPoint;
        public float width;
        public float height;
        public Vector2 offset;

        public ResolutionInfo(Vector2 centerPoint, float width, float height, Vector2 offset = default(Vector2))
        {
            this.centerPoint = centerPoint;
            this.width = width;
            this.height = height;
            this.offset = offset;
        }

        public ResolutionInfo(float width, float height)
        {
            this.width = width;
            this.height = height;

            centerPoint = Vector2.zero;
            offset = Vector2.zero;
        }

        public string GetInfoToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"|{centerPoint.x}|{centerPoint.y}|{width}|{height}|{offset.x}|{offset.y}");

            return sb.ToString();
        }
    }

    public class ResolutionUtiliy
    {
        //지금은 상수값이다. 나중에는 값을 정해주어야한다. 지금은 참고용으로만 보자
        //readonly Vector2 Old_CenterPos = new Vector2(-428, 540);
        //readonly Vector2 New_CneterPos = new Vector2(-448, 495);
        //readonly Vector2 Old_Rect = new Vector2(855, 1080);
        //readonly Vector2 New_Rect = new Vector2(897, 990);

        public static ResolutionInfo GetPictureResolution()
        {
            var resolution = new ResolutionInfo(new Vector2(-428, 540), 855, 1080);
            //Debug.Log(resolution.GetInfoToString());

            return resolution;
        }

        public static ResolutionInfo GetUIResolution()
        {
            return new ResolutionInfo(new Vector2(-448, 495), 897, 990);
        }

        public static ResolutionInfo GetResolutionInfo_RectTransform(RectTransform rectTr, Vector2 offset = default(Vector2))
        {
            if (rectTr)
            {
                ResolutionInfo info = new ResolutionInfo();
                info.centerPoint = rectTr.anchoredPosition;
                info.width = rectTr.rect.width;
                info.height = rectTr.rect.height;
                info.offset = offset;

                return info;
            }

            return default(ResolutionInfo);
        }

        public static ResolutionInfo GetResolutionInfo_Texture(Texture texture, Vector2 offset = default(Vector2))
        {
            if (texture)
            {
                ResolutionInfo info = new ResolutionInfo();
                info.centerPoint = Vector2.zero;
                info.width = texture.width;
                info.height = texture.height;
                info.offset = offset;

                return info;
            }

            return default(ResolutionInfo);
        }

        public static ResolutionInfo GetScreenResolution()
        {
            int width = Screen.width;
            int height = Screen.height;

            return new ResolutionInfo(width, height);
        }
    }

    public class ModifyPoint
    {
        public static float GetModifiedPointX(float centerX_A, float width_A, float centerX_B, float width_B, float oldPointX)
        {
            return (oldPointX - centerX_A) * (width_B / width_A) + centerX_B;
        }

        public static float GetMoidifiedPointY(float centerY_A, float height_A, float centerY_B, float height_B, float oldPointY)
        {
            return (oldPointY - centerY_A) * (height_B / height_A) + centerY_B;
        }


        public static Vector2 GetMoidifiedPoint(float centerX_A, float centerY_A, float centerX_B, float centerY_B, float width_A, float height_A, float width_B, float height_B, float oldPointX, float oldPointY, float offsetX = 0f, float offsetY = 0f)
        {
            return new Vector2(GetModifiedPointX(centerX_A, width_A, centerX_B, width_B, oldPointX) + offsetX, GetMoidifiedPointY(centerY_A, height_A, centerY_B, height_B, oldPointY) + offsetY);
        }

        public static Vector2 GetMoidifiedPoint_Default(float pointX, float pointY)
        {
            var pictrue = ResolutionUtiliy.GetPictureResolution();
            var ui = ResolutionUtiliy.GetUIResolution();
            return GetMoidifiedPoint(pictrue.centerPoint.x, pictrue.centerPoint.y, ui.centerPoint.x, ui.centerPoint.y, pictrue.width, pictrue.height, ui.width, ui.height, pointX, pointY);
        }

        public static Vector2 GetMoidifiedPoint(RectTransform rectTr ,float pointX, float pointY, float offestX = 0, float offsetY = 0)
        {
            var pictrue = ResolutionUtiliy.GetPictureResolution();
            var rect = ResolutionUtiliy.GetResolutionInfo_RectTransform(rectTr);

            return GetMoidifiedPoint(pictrue.centerPoint.x, pictrue.centerPoint.y, rect.centerPoint.x, rect.centerPoint.y, pictrue.width, pictrue.height, rect.width, rect.height, pointX, pointY, offestX, offsetY);
        }
    }
}

