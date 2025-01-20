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
                // �񵿱� �۾� ����
                Debug.Log("�񵿱� �۾� ���� ��...");
                await UniTask.Delay(1000, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("�񵿱� �۾��� ��ҵǾ����ϴ�.");
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