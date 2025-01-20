using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neofect.BodyChecker.Utility
{
    public class TextFormatUtility : MonoBehaviour
    {
        public static string GetAngleText(float angle, float normalAngle)
        {
            return $"<color=#ffc877>{angle:N0}°</color><color=#9cdbff>/{normalAngle:N0}°</color>";
        }

        public static string GetAngleTextUnrecorded(float normalAngle)
        {
            return $"<color=#ffc877>-</color><color=#9cdbff>/{normalAngle:N0}°</color>";
        }
    }
}