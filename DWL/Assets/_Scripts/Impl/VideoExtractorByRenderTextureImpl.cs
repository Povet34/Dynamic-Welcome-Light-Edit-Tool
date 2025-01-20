using UnityEngine;
using UnityEngine.Video;

public class VideoExtractorByRenderTextureImpl : IVideoExtractor
{
    public Texture2D ExtractFrame(VideoPlayer videoPlayer)
    {
        RenderTexture renderTexture = videoPlayer.texture as RenderTexture;
        if (!renderTexture) return null;
         
        Texture2D videoFrameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        if(!videoFrameTexture) return null;

        RenderTexture.active = renderTexture;
        videoFrameTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        videoFrameTexture.Apply();

        RenderTexture.active = null;

        return videoFrameTexture; 
    }
}
