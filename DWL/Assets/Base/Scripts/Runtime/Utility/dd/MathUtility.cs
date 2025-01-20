using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neofect.BodyChecker.Utility
{
    public class MathUtility
    {
        public static float GetScale(float value, float min, float max, float minScale, float maxScale)
        {
            float scaled = minScale + (value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}
