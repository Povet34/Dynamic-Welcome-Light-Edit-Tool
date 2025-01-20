using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface UIResetCallback
{
    void ResetCallback();
}

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
[AddComponentMenu("UI/Custom/UIButton")]
public class UIButton : Button, UIResetCallback
{
    #region UI Button Method : ------------------------------------------------
    /// <summary>
    /// Sound 기능 (Hover, Click)
    /// Animation 기능 (Hover, Click)
    /// </summary>
    #endregion
    [SerializeField] public Text MainText;

    [SerializeField] public Color ColorNormal = Color.white;
    [SerializeField] public Color ColorHighlighted = Color.white;
    [SerializeField] public Color ColorPressed = Color.white;
    [SerializeField] public Color ColorSelected = Color.white;
    [SerializeField] public Color ColorDisabled = Color.white;

    protected string buttonData;
    public string ButtonData { get { return buttonData; } set { buttonData = value; } }

    protected UnityAction<string, PointerEventData> downCallback;
    protected UnityAction<string, PointerEventData> upCallback;

    [Header("Sound Effect")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    [Header("Animation")]
    public new Animator animator;
    public string hoverAnimation = "Hover";
    public string clickAnimation = "Click";

    private float clickDelay = 0.5f;        // 클릭 딜레이 시간 (초)
    private float clickTimer = 0.0f;
    private bool isClickDisabled = false;

    protected UnityAction<string> callbackClick;

    protected override void Awake()
    {
        audioSource = this.GetOrAddComponent<AudioSource>();
    }

    private void Update()
    {
        if (isClickDisabled) 
        {
            clickTimer += Time.deltaTime;
            if (clickTimer >= clickDelay) 
            {
                isClickDisabled = false;
                clickTimer = 0.0f;
            }
        }
    }

    #region Pointer Event : ---------------------------------------------------
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (null != hoverSound)
            audioSource.PlayOneShot(hoverSound);

        if (null != animator && base.interactable)
            animator.SetTrigger(hoverAnimation);

        // 툴팁 표시 로직 추가
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 툴팁 숨김 로직 추가
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable)
            return;

        if (isClickDisabled) 
            return;

        if (null != clickSound)
            audioSource.PlayOneShot(clickSound);

        if (null != animator && base.interactable)
            animator.SetTrigger(clickAnimation);

        base.OnPointerClick(eventData);
        callbackClick?.Invoke(buttonData);
        isClickDisabled = true;
    }
    #endregion

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

    public void SetCallback(string buttonData, UnityAction<string> callbackClick, UnityAction<string, PointerEventData> downCallback = null, UnityAction<string, PointerEventData> upCallback = null)
    {
        this.buttonData = buttonData;
        this.callbackClick = callbackClick;
        this.downCallback = downCallback;
        this.upCallback = upCallback;
    }

    public virtual void ResetCallback()
    {
        this.buttonData = string.Empty;
        this.callbackClick = null;
        this.downCallback = null;
        this.upCallback = null;
    }

    #region Event Data : ------------------------------------------------------
    

    //public override void OnPointerDown(PointerEventData eventData)
    //{
    //    if (interactable)
    //    {
    //        base.OnPointerDown(eventData);
    //        downCallback?.Invoke(buttonData, eventData);
    //    }
    //}

    //public override void OnPointerUp(PointerEventData eventData)
    //{
    //    if (interactable)
    //    {
    //        base.OnPointerUp(eventData);
    //        upCallback?.Invoke(buttonData, eventData);
    //    }
    //}
    #endregion
}