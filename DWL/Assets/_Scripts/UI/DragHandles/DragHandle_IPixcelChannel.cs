using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandle_IPixcelChannel : DragHandle
{
    IPixelChannel owner;
    Action<IPixelChannel> beginDragCallback;
    Action<int, Vector2Int> changePositionByHandle;

    public void Setup(IPixelChannel owner, Action<IPixelChannel> beginDragCallback, Action<int, Vector2Int> changePositionByHandle)
    {
        this.owner = owner;
        this.beginDragCallback = beginDragCallback;
        this.changePositionByHandle = changePositionByHandle;

        changePositionByHandle?.Invoke(owner.Index, GetRoundedPos());
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        beginDragCallback?.Invoke(owner);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        beginDragCallback?.Invoke(owner);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        changePositionByHandle?.Invoke(owner.Index, GetRoundedPos());
    }
}
