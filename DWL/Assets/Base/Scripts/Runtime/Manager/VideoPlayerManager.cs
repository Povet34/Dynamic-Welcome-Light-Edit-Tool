using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : AbstractSingleton<VideoPlayerManager>
{
    // 영상이 표현될 대상 영역.
    public RawImage rawImage;

    private VideoPlayer videoPlayer;

    private Coroutine coroutineAsyncPrepareVideo;
    //private Coroutine coroutineAsyncPlay;

    private Action actionAfterLoopPointReached;

    // Start is called before the first frame update
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += LoopPointReached;
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= LoopPointReached;
    }

    public void Prepare(RawImage targetRawImage, VideoClip videoClip)
    {
        StartCoroutine(AsyncPrepareVideo(targetRawImage, videoClip, ()=> { Debug.Log($"===== frame count : {videoPlayer.frameCount} ====="); }));
    }

    public void Play(RawImage targetRawImage, VideoClip videoClip, long repeatFrame = -1, Action action = null)
    {
        if(repeatFrame == - 1)
            SetActionAfterLoopPointerReached(null);
        else
            SetActionAfterLoopPointerReached(() => SetFrameAndAsyncPlay(repeatFrame));
        StopAsyncPrepareVideo();
        coroutineAsyncPrepareVideo = StartCoroutine(AsyncPrepareVideo(targetRawImage, videoClip, ()=>
        {
            action?.Invoke();
            videoPlayer.isLooping = false;
            videoPlayer.Play();
        }));
    }


    private IEnumerator AsyncPrepareVideo(RawImage targetRawImage, VideoClip videoClip, Action action = null)
    {
        videoPlayer.clip = videoClip;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        targetRawImage.texture = videoPlayer.texture;
        action?.Invoke();
    }

    public void Play()
    {
        videoPlayer.Play();
    }

    public void Stop()
    {
        videoPlayer.Stop();
        StopAsyncPrepareVideo();
    }

    public void Pause()
    {
        videoPlayer.Pause();
    }

    public bool IsPrepared()
    {
        return videoPlayer.isPrepared;
    }

    public bool IsPlaying()
    {
        return videoPlayer.isPlaying;
    }

    public void SetTime(float time)
    {
        if (videoPlayer.canSetTime)
            videoPlayer.time = time;
        else
            Debug.LogWarning($"not available set time");
    }

    public void SetFrame(long frame)
    {
        if (videoPlayer.canSetTime)
            videoPlayer.frame = frame;
        else
            Debug.LogWarning($"not available set time");
    }

    public void SetFrameAndAsyncPlay(long frame)
    {
        SetFrame(frame);
        //StopAsyncPlay();
        StartCoroutine(AsyncPlay());
    }

    IEnumerator AsyncPlay()
    {
        yield return new WaitForSeconds(0.1f);
        Play();
    }

    public void SetActionAfterLoopPointerReached(Action actionAfterLoopPointReached)
    {
        this.actionAfterLoopPointReached = actionAfterLoopPointReached;
    }

    void LoopPointReached(VideoPlayer source)
    {
        actionAfterLoopPointReached?.Invoke();
    }

    private void StopAsyncPlay()
    {
        //if (coroutineAsyncPlay != null)
        //    StopCoroutine(coroutineAsyncPlay);
    }

    private void StopAsyncPrepareVideo()
    {
        if (coroutineAsyncPrepareVideo != null)
            StopCoroutine(coroutineAsyncPrepareVideo);
    }
}
