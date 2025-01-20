using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPixelChannel
{
    int Index { get; set; }
    Texture2D SourceTexture { get; }
    ePixelChannelType PixelChannelType { get;}

    void Init(int index, Color color, Vector2 initPos, int scale, bool isNotifierShow, Action<IPixelChannel> selectCallback, Action<int, Vector2Int> changePositionByHandle);

    /// <summary>
    /// Position, index만 갱신한다. (텍스쳐, frame x)
    /// </summary>
    void UpdateChannelInfo(int index, Vector2Int position);

    /// <summary>
    /// 텍스쳐와 Frame, position을 갱신한다. 실질적인 Update며, data로 저장하기 위함
    /// </summary>
    void SetTextrue(Texture2D texture, int recordTime, List<Vector2> positions);
    List<PixelData> GetPixelDatas();
    List<GameObject> GetDragHandles();
    void Destroy();
    void ShowNotifier(bool isShow);
    void SetHandlePos(List<Vector2> points);
    void SetParent(Transform parent);
    void UpdateSelection(bool isSelected);
    void ChangeSize(int multi);
    Vector2Int GetFirstHandlePos();
}
