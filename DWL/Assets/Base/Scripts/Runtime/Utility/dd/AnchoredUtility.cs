using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neofect.BodyChecker.Utility
{
    public class AnchoredUtility : MonoBehaviour
    {
        private const float WIDTH = 1920f;
        private const float HEIGHT = 1080f;
        private const float HALF_WIDTH = 960;
        private const float HALF_HEIGHT = 540;

        // kinect zure camera에서 제공하는 position 값.( 화면을 보는 기준 )
        // 너비( 좌 에서 우 ) : + ~ 0 ~ -
        // 높이( 아래 에서 위 ) : + ~ 0 ~ -

        // kinect azure camera에서 제공하는 position color 값의 범위.( 화면을 보는 기준 )
        // 너비( 좌 에서 우 ) : 1920 ~ 960 ~ 0
        // 높이( 아래 에서 위 ) : 1080 ~ 540 ~ 0

        // ui anchored position 값의 범위.( 화면을 보는 기준 )
        // 너비( 좌 에서 우 ) : -960 ~ 0 ~ 960
        // 높이( 아래 에서 위 ) : -540 ~ 0 ~ 540

        // 측정시 좌,우, 상,하 반전을 하고 있어서, 아래와 같은 공식을 사용한다.
        // 너비 : position color 값이 0 일때 1920 - 0 - 960 = 960;
        // 높이 : position color 값이 0 일때 1080 - 0 - 540 = 540;
        // 너비 : position color 값이 1920 일때 1920 - 1920 - 960 = -960;
        // 높이 : position color 값이 1080 일때 1080 - 1080 - 540 = -540;
        
        public static float GetAnchoredPositionX(float positionColor)
        {
            return WIDTH - positionColor - HALF_WIDTH;
        }

        public static float GetAnchoredPositionY(float positionColor)
        {
            return HEIGHT - positionColor - HALF_HEIGHT;
        }

        public static float GetPositionColorX(float anchoredPositionX)
        {
            // anchored -> positionColor
            // -960 -> 1920
            // 0 -> 960
            // 960 -> 0

            return HALF_WIDTH - anchoredPositionX;
        }

        public static float GetPositionColorY(float anchoredPositionY)
        {
            // anchored -> positionColor
            // -540 -> 1080
            // 0 -> 540
            // 540 -> 0

            return HALF_HEIGHT - anchoredPositionY;
        }

        
        public static float GetPositionX(float positionX, float positionColorX, float targetPositionColorX, float offsetX= 0f)
        {
            var x = Mathf.Abs(positionX) * Mathf.Abs(targetPositionColorX - HALF_WIDTH) / Mathf.Abs(positionColorX - HALF_WIDTH);
            if (targetPositionColorX > HALF_WIDTH)
                return x + offsetX;
            else
                return -x - offsetX;
        }

        public static float GetPositionY(float positionY, float positionColorY, float targetPositionColorY, float offsetY = 0f)
        {
            var y = Mathf.Abs(positionY) * Mathf.Abs(targetPositionColorY - HALF_HEIGHT) / Mathf.Abs(positionColorY - HALF_HEIGHT);
            if (targetPositionColorY > HALF_HEIGHT)
                return y + offsetY;
            else
                return -y - offsetY;
        }

        public static Vector2 GetPolarToCartesianCroodinate(float radius, float angle)
        {
            float angleInRadians = ConvertToRadians(angle);

            float x = radius * Mathf.Cos(angleInRadians);
            float y = radius * Mathf.Sin(angleInRadians);

            return new Vector2(x, y);
        }

        static float ConvertToRadians(float angle)
        {
            //Mathf.Deg2Rad * angle
            return (Mathf.PI / 180) * angle;
        }
    }
}
