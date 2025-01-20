using UnityEngine;
using UnityEngine.UI;
using RenderHeads.Media.AVProMovieCapture;
using Common.UI;

public class VideoRecordingManager : Singleton<VideoRecordingManager>
{
    RawImage rawImage;
    CaptureFromScreen screenCapture;
    CaptureFromTexture textureCaptrue;

    private void Awake()
    {
        screenCapture = GetComponentInChildren<CaptureFromScreen>();
        textureCaptrue = GetComponentInChildren<CaptureFromTexture>();
    }

    public void StartScreenRecording()
    {
        screenCapture.StartCapture();
    }

    public void StopScreenRecording()
    {
        screenCapture.StopCapture();
    }

    public void StartTextureRecording()
    {
        if(!rawImage)
        {
            rawImage = FindObjectOfType<VideoViewer>().GetComponentInChildren<RawImage>();
        }

        if (rawImage)
        {
            Texture textureToCapture = rawImage.texture;

            if (textureToCapture != null)
            {
                textureCaptrue.SetSourceTexture(textureToCapture);
                textureCaptrue.StartCapture();
            }
        }
    }

    public void StopTextrueRecording()
    {
        if (textureCaptrue.IsCapturing())
        {
            textureCaptrue.StopCapture();
        }

        rawImage = null;
    }
}
