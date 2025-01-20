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
    /// Position, index�� �����Ѵ�. (�ؽ���, frame x)
    /// </summary>
    void UpdateChannelInfo(int index, Vector2Int position);

    /// <summary>
    /// �ؽ��Ŀ� Frame, position�� �����Ѵ�. �������� Update��, data�� �����ϱ� ����
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
