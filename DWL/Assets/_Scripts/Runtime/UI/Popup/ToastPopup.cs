using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastPopup : Popup
{
    [SerializeField] private UITextMeshPro descText;

    [SerializeField] private float fadeOutDuration = 0.5f;      // Fade Out 지속 시간
    [SerializeField] private float delayBeforeFadeOut = 3.0f;   // Fade Out 시작 전 지연 시간

    private CanvasGroup canvasGroup;
    private Action callbackClose;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (null == canvasGroup )
        {
            canvasGroup.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0.0f;
    }

    public override void ShowPopup(PopupSetting settings, Action callbackClose)
    {
        this.gameObject.SetActive(true);

        if (!string.IsNullOrEmpty(settings.Desc))
            descText.text = settings.Desc;

        StartCoroutine(ShowToastCoroutine());
        this.callbackClose = callbackClose;
    }

    public override void ClosePopup()
    {
        this.gameObject.SetActive(false);
    }

    private IEnumerator ShowToastCoroutine()
    {
        canvasGroup.alpha = 1.0f;

        // 지연
        yield return new WaitForSeconds(delayBeforeFadeOut);

        // Fade Out
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, fadeOutDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / duration);
            yield return null;
        }

        callbackClose?.Invoke();
    }
}