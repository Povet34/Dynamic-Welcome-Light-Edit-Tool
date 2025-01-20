using UnityEngine;
using UnityEngine.Video;

public class VideoExtractorImpl_ByCachedRenderTexture : IVideoExtractor
{
    private Texture2D reusableTexture;

    public Texture2D ExtractFrame(VideoPlayer videoPlayer)
    {
        RenderTexture renderTexture = videoPlayer.texture as RenderTexture;
        if (renderTexture == null)
        {
            Debug.LogError("VideoPlayer does not have a valid RenderTexture.");
            Debug.Log($"videoPlayer : {videoPlayer}..... {videoPlayer.url}.... {videoPlayer.texture}");
            return null;
        }

        if (reusableTexture == null || reusableTexture.width != renderTexture.width || reusableTexture.height != renderTexture.height)
        {
            if (reusableTexture != null)
            {
                Object.Destroy(reusableTexture);
            }

            reusableTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        }

        RenderTexture.active = renderTexture;
        reusableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        reusableTexture.Apply();
        RenderTexture.active = null;

        return reusableTexture;
    }
}
