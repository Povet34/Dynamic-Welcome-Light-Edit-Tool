using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[AddComponentMenu("UI/Custom/UISlider")]
public class UISlider : Slider, UIResetCallback
{
    private UnityAction<float> onSelect;
    private UnityAction<float> onDeselect;

    public virtual void ResetCallback()
    {
        base.onValueChanged = null; 
        this.onSelect = null;
        this.onDeselect = null;
    }

    public void SetCallback(SliderEvent onValueChanged, UnityAction<float> onSelect = null, UnityAction<float> onDeselect = null)
    {
        base.onValueChanged = onValueChanged;
        this.onSelect = onSelect;
        this.onDeselect = onDeselect;
    }

    #region Event Data : ------------------------------------------------------
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelect?.Invoke(this.value);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        onDeselect?.Invoke(this.value);
    }
    #endregion
}