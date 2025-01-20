using UnityEngine;
using UnityEngine.Video;

public class VideoRatioCalculator
{
    float videoWidth;
    float videoHeight;

    float panelWidth;
    float panelHeight;

    float panelPosX;
    float panelPosY;

    Vector2 offset;

    float widthRatioVP => videoWidth / panelWidth;
    float heightRatioVP => videoHeight / panelHeight;

    public VideoRatioCalculator() { }

    public VideoRatioCalculator(VideoPlayer source, RectTransform panelRect, Vector2 offset)
    {
        SetVideoSize(source.width, source.height);
        SetPanelSize(panelRect.rect.width, panelRect.rect.height);
        SetPanelPos(panelRect.anchoredPosition.x, panelRect.anchoredPosition.y);
        SetOffset(offset);
    }

    public VideoRatioCalculator(float textrueWidth, float textureHeight, RectTransform panelRect, Vector2 offset)
    {
        SetVideoSize(textrueWidth, textureHeight);
        SetPanelSize(panelRect.rect.width, panelRect.rect.height);
        SetPanelPos(panelRect.anchoredPosition.x, panelRect.anchoredPosition.y);
        SetOffset(offset);
    }

    public VideoRatioCalculator(float videoWidth, float videoHeight, float panelWidth, float panelHeight, float panelPosX, float panelPosY, Vector2 offset)
    {
        SetVideoSize(videoWidth, videoHeight);
        SetPanelSize(panelWidth, panelHeight);
        SetPanelPos(panelPosX, panelPosY);
        SetOffset(offset);
    }

    public bool IsSame()
    {
        return videoWidth == panelWidth && videoHeight == panelHeight;
    }

    private void SetVideoSize(float videoWidth, float videoHeight)
    {
        this.videoWidth = videoWidth;
        this.videoHeight = videoHeight;
    }

    private void SetPanelSize(float panelWidth, float panelHeight)
    {
        this.panelWidth = panelWidth;
        this.panelHeight = panelHeight;
    }

    private void SetPanelPos(float posX, float posY)
    {
        panelPosX = posX;
        panelPosY = posY;
    }

    private void SetOffset(Vector2 offset)
    {
        this.offset = offset;
    }

    /// <summary>
    /// Calculates the relocated pixel position in video coordinates.
    /// </summary>
    /// <param name="pixel">GameObject to calculate position for.</param>
    /// <returns>New position in video coordinates.</returns>
    public Vector2 GetPositionInVideoCoordinates(GameObject pixel)
    {
        var rt = pixel.GetComponent<RectTransform>();
        if (rt)
        {
            float videoPosX = (rt.anchoredPosition.x + panelWidth * 0.5f - panelPosX - offset.x) * widthRatioVP;
            float videoPosY = (rt.anchoredPosition.y + panelHeight * 0.5f - panelPosY - offset.y) * heightRatioVP;

            return new Vector2(videoPosX, videoPosY);
        }

        return Vector2.zero;
    }

    public Vector2Int GetPositionInVideoCoordinatesByPosition(Vector2 pos)
    {
        int videoPosX = (int)((pos.x + panelWidth * 0.5f - panelPosX - offset.x) * widthRatioVP);
        int videoPosY = (int)((pos.y + panelHeight * 0.5f - panelPosY - offset.y) * heightRatioVP);

        return new Vector2Int(videoPosX, videoPosY);
    }

    public Vector2 GetPositionFromVideoCoordinates(Vector2 videoCoordinates)
    {
        float pixelPosX = (videoCoordinates.x / widthRatioVP) - panelWidth * 0.5f + panelPosX + offset.x;
        float pixelPosY = (videoCoordinates.y / heightRatioVP) - panelHeight * 0.5f + panelPosY + offset.y;

        return new Vector2(pixelPosX, pixelPosY);
    }
}
