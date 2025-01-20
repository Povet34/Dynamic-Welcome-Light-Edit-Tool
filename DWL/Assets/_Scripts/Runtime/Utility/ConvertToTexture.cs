using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConvertToTexture : MonoBehaviour
{
    public static Texture2D ConvertRawImageToTexture2D(RawImage rawImage)
    {
        Texture texture = rawImage.texture;
        RenderTexture renderTexture = texture as RenderTexture;

        if (renderTexture == null)
        {
            Debug.LogError("Assigned texture is not a RenderTexture.");
            return null;
        }

        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;

        return texture2D;
    }

    public static Texture2D ConvertTextureToTexture2D(Texture texture)
    {
        RenderTexture renderTexture = texture as RenderTexture;
        if (renderTexture == null)
        {
            Debug.LogError("Assigned texture is not a RenderTexture.");
            return null;
        }

        Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;

        return texture2D;
    }
}
