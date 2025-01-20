using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoReplayingManager : AbstractSingleton<VideoReplayingManager>
{
    private VideoPlayer videoPlayer;

    private Coroutine coroutineAsyncPrepareVideo;
    private Coroutine coroutineAsyncPlay;

    private Action finished;

    // Start is called before the first frame update
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += LoopPointReached;
    }

    void LoopPointReached(VideoPlayer source)
    {
        finished?.Invoke();
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= LoopPointReached;
    }

    public void SetFinished(Action finished)
    {
        this.finished = finished;
    }

    public void Prepare(RawImage rawImage, string filePath)
    {
        StopAsyncPrepare();
        coroutineAsyncPrepareVideo = StartCoroutine(AsyncPrepare(rawImage, filePath));
    }

    IEnumerator AsyncPrepare(RawImage rawImage, string filePath)
    {
        videoPlayer.url = filePath;
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        rawImage.texture = videoPlayer.texture;
        Pause();
    }

    public void Play()
    {
        if (videoPlayer.isPrepared)
            videoPlayer.Play();
        else
            Debug.LogWarning("video is not prepared");
    }

    public void Stop()
    {
        videoPlayer.Stop();
        StopAsyncPrepare();
        StopAsyncPlay();
    }

    public void Pause()
    {
        videoPlayer.Pause();
    }

    public bool IsPlaying()
    {
        return videoPlayer.isPlaying;
    }

    public bool IsPaused()
    {
        return videoPlayer.isPaused;
    }

    public bool IsPrepared()
    {
        return videoPlayer.isPrepared;
    }

    public void SetSeek(float seek)
    {
        var targetFrame = (long)(GetTotalFrame() * seek); 
        SetFrame(targetFrame);
    }

    public double GetTime()
    {
        if (videoPlayer.isPrepared)
            return videoPlayer.time;
        else
            return 0;
    }

    public double GetTotalTime()
    {
        if (videoPlayer.isPrepared)
            return videoPlayer.length;
        else
            return 0;
    }

    public void SetTime(float time)
    {
        if (videoPlayer.canSetTime)
            videoPlayer.time = time;
        else
            Debug.LogWarning($"not available set time");
    }

    public long GetFrame()
    {
        if (videoPlayer.isPrepared)
            return videoPlayer.frame;
        else
            return 0;
    }

    public ulong GetTotalFrame()
    {
        if (videoPlayer.isPrepared)
            return videoPlayer.frameCount;
        else
            return 0;
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
        StopAsyncPlay();
        StartCoroutine(AsyncPlay());
    }

    IEnumerator AsyncPlay()
    {
        yield return new WaitForSeconds(0.1f);
        Play();
    }

    private void StopAsyncPlay()
    {
        if (coroutineAsyncPlay != null)
            StopCoroutine(coroutineAsyncPlay);
    }

    private void StopAsyncPrepare()
    {
        if (coroutineAsyncPrepareVideo != null)
            StopCoroutine(coroutineAsyncPrepareVideo);
    }
}
