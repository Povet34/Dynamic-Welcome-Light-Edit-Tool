using UnityEngine;

public static class Definitions
{
    public const float LINEAR_INTERPOLATION_BETWEEN_ORIGIN_DATA_OFFSET = 0.5f;
    public const float EPSILON = 0.0001f;

    //Graph Compare
    public static Color32 COMPARE_GRAPH_LINE_COLOR = new Color32(38, 119, 128, 100);

    //Frame Rate Offset
    public static int GetProperRateOffset(int videoWidth)
    {
        if(videoWidth == RESOLUTION_WIDTH_1920)
            return RESOLUTION_WIDTH_1920;
        if (videoWidth == RESOLUTION_WIDTH_2560)
            return RESOLUTION_WIDTH_2560;

        return RESOLUTION_WIDTH_1920;
    }

    public static int RESOLUTION_WIDTH_1920 = 1;
    public static int RESOLUTION_WIDTH_2560 = 10;

    //Duty Value
    public static int FADE_VALUE = 255;

    public static int ADD_FADE_TIME_OFFSET(int value) => IS_FADE_VALUE(value) ? 1 : 0;
    public static bool IS_FADE_VALUE(int value) => value == FADE_VALUE;
    public static bool IS_FLICKERING(int pre, int cur) => pre != FADE_VALUE && cur != FADE_VALUE;
    public static int LAST_DUTY_INDEX = 3;
    public static int FIRST_DUTY = 0;

    //Excel
    public static string EXCEL_ORIGIN_PREFIX = "Origin";
    public static string EXCEL_MODIFIED_PREFIX = "Modified";
    public static string EXCEL_INTERNAL_COMPUTATION_PREFIX = "InternalComputation";

    public static string CHANNEL_DATA_EXCEL_SHEET_NAME_POSITIONS = "Positions";
    public static string CHANNEL_DATA_EXCEL_SHEET_NAME_ORIGIN_SIMPLIFIED = $"{EXCEL_ORIGIN_PREFIX} Simplified Data";
    public static string CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_SIMPLIFIED = $"{EXCEL_MODIFIED_PREFIX} Simplified Data";
    public static string CHANNEL_DATA_EXCEL_SHEET_NAME_MODIFIED_BRIGHTNESS = $"{EXCEL_MODIFIED_PREFIX} Brightness Data";
    public static string CHANNEL_DATA_EXCEL_SHEET_NAME_INTERNAL_COMPUTATION_ORIGIN_DEATIL_BRIGHTNESS = $"{EXCEL_INTERNAL_COMPUTATION_PREFIX} Detail Data";
    public static string CHANNEL_DATA_EXCEL_SHEET_CHANNELTIME = "Channel/Time(ms)";
    public static string CHANNEL_DATA_EXCEL_SHEET_VIDEO_DATA = "Video Data";
    public static string CHANNEL_DATA_EXCEL_SHEET_DUTY = "Duty";

    public static string CHANNEL_DATA_EXCEL_BRIGHTNESS = "BRIGHTNESS";
    public static string CHANNEL_DATA_EXCEL_COLOR = "RGB";

    public static string CHANNEL_DATA_EXCEL_SHEET_ORIGINAL_EXTRACTED = $"{EXCEL_ORIGIN_PREFIX} {CHANNEL_DATA_EXCEL_BRIGHTNESS}";
    public static string CHANNEL_DATA_EXCEL_SHEET_MODIFIED_EXTRACTED = $"{EXCEL_MODIFIED_PREFIX} {CHANNEL_DATA_EXCEL_BRIGHTNESS}";

    public static string CHANNEL_DATA_EXCEL_SHEET_COLOR = $"{EXCEL_ORIGIN_PREFIX} {CHANNEL_DATA_EXCEL_COLOR}";

    public static string CHANNEL_DATA_EXCEL_TEMP_SHEET_NAME = "TempSheet";

    public static string EXCEL_VIDEO_DATA_HEADER_FRAMECOUNT = "FrameCount";
    public static string EXCEL_VIDEO_DATA_HEADER_VIDEOLENGTH = "VideoLength";
    public static string EXCEL_VIDEO_DATA_HEADER_RESOLUTION = "Resolution";


    //Excel Table
    public static string ORIGIN_EXCEL_TIME_TABLE = $"{EXCEL_ORIGIN_PREFIX} Time Table";
    public static string ORIGIN_EXCEL_DUTY_TABLE = $"{EXCEL_ORIGIN_PREFIX} Duty Table";
    public static string ORIGIN_EXCEL_MODE_TABLE = $"{EXCEL_ORIGIN_PREFIX} Mode Table";

    public static string MODIFIED_EXCEL_TIME_TABLE = $"{EXCEL_MODIFIED_PREFIX} Time Table";
    public static string MODIFIED_EXCEL_DUTY_TABLE = $"{EXCEL_MODIFIED_PREFIX} Duty Table";
    public static string MODIFIED_EXCEL_MODE_TABLE = $"{EXCEL_MODIFIED_PREFIX} Mode Table";

    public static int EXCEL_TABLE_BUFFER = 100;

    //Coming soon
    public static string COMMING_SOOM_COMENTS = "Coming Soon";

    //Channel Selection
    public static string CHANNEL_SELECTED_SPRITE_NAME = "_img_contents_point_s";
    public static string CHANNEL_BASIC_SPRITE_NAME = "NonTagetEllipse";

    //Color to Brightness
    public static float RED_WEIGHT = 0.299f;
    public static float GREEN_WEIGHT = 0.587f;
    public static float BLUE_WEIGHT = 0.114f;

    public static float CONVERT_8BTI_TO_8BIT = 1;
    public static float CONVERT_8BTI_TO_10BIT = (float)((1 << 10) / 255);
    public static float CONVERT_8BTI_TO_12BIT = (float)((1 << 12) / 255);
    public static float CONVERT_8BTI_TO_16BIT = (float)((1 << 16) / 255);

    public static float GetBitRatio(int bit)
    {
        switch (bit)
        {
            case 10:
                return CONVERT_8BTI_TO_10BIT;
            case 12:
                return CONVERT_8BTI_TO_12BIT;
            case 16:
                return CONVERT_8BTI_TO_16BIT;
            default:
                return 1;
        }
    }

    public static float GetBrightness255(UnityEngine.Color32 color)
    {
        return RED_WEIGHT * color.r + GREEN_WEIGHT * color.g + BLUE_WEIGHT * color.b;
    }


    //Error Coments

    public static string WARNING = "[Warning]";
    public static string SAVE_FAILED = "[Save Failed]";

    public static string ERROR_EXIST_CHANNEL_STARTED_FADE_ON_ZERO_FRAME = $"{SAVE_FAILED} 0번 Frame에서부터 Fade가 시작되는 채널이 존재합니다.";
    public static string ERROR_EXIST_CHANNEL_FAIL_SHOT_OF_MINIMUM_INTERVALS_DUTY_ON_START = $"{WARNING} 시작 구간에서 최소한의 변화/유지 구간을 충족시키지 못한 채널이 존재합니다.";
    public static string LAST_DUTY_MAINTAIN_REGION_ISNT_SO_SMALL = $"{WARNING} 마지막 유지 구간이 너무 작습니다.";

    //Channel Size

    public static int CHANNEL_SIZE_ZOOM_X5 = 2;
    public static int CHANNEL_SIZE_ZOOM_X4 = 3;
    public static int CHANNEL_SIZE_ZOOM_X3 = 5;
    public static int CHANNEL_SIZE_ZOOM_X2 = 8;
    public static int CHANNEL_SIZE_ZOOM_X1 = 10;

    public static float SEGMENT_SIZE_ZOOM_X1 = 2f;
    public static float SEGMENT_SIZE_ZOOM_X2 = 1f;
    public static float SEGMENT_SIZE_ZOOM_X3 = 0.7f;
    public static float SEGMENT_SIZE_ZOOM_X4 = 0.5f;
    public static float SEGMENT_SIZE_ZOOM_X5 = 0.5f;

    public static int GetChannelSize_ByRawImg(int sizeDelta)
    {
        switch (sizeDelta)
        {
            case 1:
                return CHANNEL_SIZE_ZOOM_X1;
            case 2:
                return CHANNEL_SIZE_ZOOM_X2;
            case 3:
                return CHANNEL_SIZE_ZOOM_X3;
            case 4:
                return CHANNEL_SIZE_ZOOM_X4;
            case 5:
                return CHANNEL_SIZE_ZOOM_X5;
            default:
                return CHANNEL_SIZE_ZOOM_X1;
        }
    }

    public static float GetSegmentSize_ByRawImg(int sizeDelta)
    {
        switch (sizeDelta)
        {
            case 1:
                return SEGMENT_SIZE_ZOOM_X1;
            case 2:
                return SEGMENT_SIZE_ZOOM_X2;
            case 3:
                return SEGMENT_SIZE_ZOOM_X3;
            case 4:
            case 5:
                return SEGMENT_SIZE_ZOOM_X3;
            default:
                return CHANNEL_SIZE_ZOOM_X1;
        }
    }
}