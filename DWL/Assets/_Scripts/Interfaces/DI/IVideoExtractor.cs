using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public interface IVideoExtractor 
{
    Texture2D ExtractFrame(VideoPlayer videoPlayer);
}
