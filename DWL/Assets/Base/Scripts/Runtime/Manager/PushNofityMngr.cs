using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;

public class PushNofityMngr : AbstractSingleton<PushNofityMngr>
{
    private FirebaseApp app;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(
            task => 
            {
                if (task.Result == DependencyStatus.Available)
                {
                    app = FirebaseApp.DefaultInstance;
                    FirebaseMessaging.TokenReceived += OnTokenReceived;
                    FirebaseMessaging.MessageReceived += OnMessageReceived;
                }
                else
                {
                    Debug.LogError("[FIREBASE] Could not resolve all dependencis: " + task.Result);
                }
            });
    }

    private void OnTokenReceived(object sender, TokenReceivedEventArgs e)
    {
        if (null != e)
        {
            Debug.LogFormat("[FIREBASE] Token: {0}", e.Token);
        }
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e) 
    {
        if (null != e && null != e.Message && null != e.Message.Notification)
        {
            Debug.LogFormat("[FIREBASE] From: {0}, Title: {1}, Text: {2}",
                e.Message.From,
                e.Message.Notification.Title,
                e.Message.Notification.Body);
        }
    }
}