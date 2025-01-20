using Cysharp.Threading.Tasks;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading;

public class PlayModeAsyncOperation : MonoBehaviour
{
    private CancellationTokenSource cancellationTokenSource;

    void OnEnable()
    {
        cancellationTokenSource = new CancellationTokenSource();
        RunAsyncOperation(cancellationTokenSource.Token).Forget();

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    private async UniTaskVoid RunAsyncOperation(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                // 비동기 작업 로직
                Debug.Log("비동기 작업 수행 중...");
                await UniTask.Delay(1000, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("비동기 작업이 취소되었습니다.");
        }
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            cancellationTokenSource.Cancel();
        }
    }
#endif

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
    }
}