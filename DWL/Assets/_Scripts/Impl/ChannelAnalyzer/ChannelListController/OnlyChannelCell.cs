using ChannelAnalyzers;
using System;
using UnityEngine;
using UnityEngine.UI;

public class OnlyChannelCell : MonoBehaviour
{
    [SerializeField] Toggle showToggle;
    [SerializeField] Text channelName;

    AChannelInfo info;
    Action<AChannelInfo, bool> toggleCallback;

    private void Awake()
    {
        showToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void Init(AChannelInfo info, Action<AChannelInfo, bool> toggleCallback)
    {
        showToggle.isOn = true;
        this.info = info;
        this.toggleCallback = toggleCallback;
        OnToggleChanged(true);

        channelName.text = $"Ch{info.channelIndex.ToString("D2")}";
    }

    public void OnToggleChanged(bool isOn)
    {
        toggleCallback?.Invoke(info, isOn);
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}