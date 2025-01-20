using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[AddComponentMenu("UI/Custom/UIToggle")]
public class UIToggle : Toggle, UIResetCallback
{
    [SerializeField] public GameObject GoOnState;
    [SerializeField] public GameObject GoOffState;

    [SerializeField] public Text MainText;

    [SerializeField] public Color ColorNormal = Color.white;
    [SerializeField] public Color ColorHighlighted = Color.white;
    [SerializeField] public Color ColorPressed = Color.white;
    [SerializeField] public Color ColorSelected = Color.white;
    [SerializeField] public Color ColorDisabled = Color.white;

    protected string toggleData;
    public string ToggleData { get { return toggleData; } set { toggleData = value; } }

    protected UnityAction<bool, string> clickCallback;
    public UnityAction<bool, string> ClickCallback { get { return clickCallback; } }

    protected override void Awake()
    {
        base.onValueChanged.RemoveListener(OnCallbackState);
        base.onValueChanged.AddListener(OnCallbackState);
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if (null != MainText)
        {
            switch (state)
            {
                case SelectionState.Normal:
                    MainText.color = ColorNormal;
                    break;
                case SelectionState.Highlighted:
                    MainText.color = ColorHighlighted;
                    break;
                case SelectionState.Pressed:
                    MainText.color = ColorPressed;
                    break;
                case SelectionState.Selected:
                    MainText.color = ColorSelected;
                    break;
                case SelectionState.Disabled:
                    MainText.color = ColorDisabled;
                    break;
            }
        }
    }

    public void SetCallback(string toggleData, UnityAction<bool, string> clickCallback)
    {
        this.toggleData = toggleData;
        this.clickCallback = clickCallback;
    }

    public virtual void ResetCallback()
    {
        this.toggleData = string.Empty;
        this.clickCallback = null;
    }

    public new void SetIsOnWithoutNotify(bool isOn)
    {
        GoOnState?.SetActive(isOn);
        GoOffState?.SetActive(!isOn);
        base.SetIsOnWithoutNotify(isOn);
    }

    private void OnCallbackState(bool isOn)
    {
        GoOnState?.SetActive(isOn);
        GoOffState?.SetActive(!isOn);
        clickCallback?.Invoke(isOn, toggleData);
    }
}