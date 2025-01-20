using System;
using UnityEngine;

public class VideoPixelChannelPositionCell : MonoBehaviour
{
    [SerializeField] GameObject bg;
    [SerializeField] UITextMeshPro channelNumText;
    [SerializeField] UIInputField inputFieldX;
    [SerializeField] UIInputField inputFieldY;

    int channlIndex;
    public int ChannlIndex => channlIndex;
    Action<int, Vector2Int> updatePosCallback;

    private void Awake()
    {
        inputFieldX.onEndEdit.AddListener(UpdatePosByInputField);
        inputFieldY.onEndEdit.AddListener(UpdatePosByInputField);
    }

    public void Init(int channlIndex, Action<int, Vector2Int> updatePosCallback)
    {
        this.channlIndex = channlIndex;
        this.updatePosCallback = updatePosCallback;

        channelNumText.text = channlIndex.ToString();
    }

    private void UpdatePosByInputField(string inputData)
    {
        if (!int.TryParse(inputFieldX.text, out int x))
            x = -1;

        if (!int.TryParse(inputFieldY.text, out int y))
            y = -1;

        if (x < 0 || y < 0)
            return;

        Vector2Int newPos = new Vector2Int(x, y);
        updatePosCallback?.Invoke(channlIndex, newPos);
    }

    public void UpdateByPixelChannelPos(Vector2Int newPos)
    {
        inputFieldX.text = newPos.x.ToString();
        inputFieldY.text = newPos.y.ToString();
    }

    public void UpdateIndex(int index)
    {
        channlIndex = index;
    }

    public void ShowChildren(bool isShow)
    {
        bg.gameObject.SetActive(isShow);
        channelNumText.gameObject.SetActive(isShow);
        inputFieldX.gameObject.SetActive(isShow);
        inputFieldY.gameObject.SetActive(isShow);
    }
}
