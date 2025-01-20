using UnityEngine;

public class TimeUtil
{
    public static string GetTimeString_MMSS(float seconds)
    {
        int totalSeconds = Mathf.RoundToInt(seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    public static string GetTimeString_SSMSMS(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);
        int remainingSeconds = totalSeconds % 60;
        int milliseconds = Mathf.FloorToInt((seconds - totalSeconds) * 1000);

        return string.Format("{0:00}:{1:000}", remainingSeconds, milliseconds);
    }
}
