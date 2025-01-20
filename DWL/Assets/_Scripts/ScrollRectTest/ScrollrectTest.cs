using ChannelAnalyzers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollrectTest : MonoBehaviour
{
    public Button button;
    public RectTransform contentPanel; // Content의 RectTransform
    public GameObject itemPrefab; // 추가할 아이템의 프리팹

    private float minScrollPos = 0f; // 최소 스크롤 지점
    private float maxScrollPos; // 최대 스크롤 지점

    private List<GameObject> itemList = new List<GameObject>();

    private void Awake()
    {
        button.onClick.AddListener(AddItem);
    }

    public void AddItem()
    {
        Instantiate(itemPrefab, contentPanel);
        StartCoroutine(AdjustScrollRange());

        PushRigtAddButton();
    }

    private void Update()
    {
        SetScrollPosition(contentPanel.anchoredPosition.x);
    }

    private IEnumerator AdjustScrollRange()
    {
        yield return new WaitForEndOfFrame();

        maxScrollPos = Mathf.Max(contentPanel.rect.width - ((RectTransform)contentPanel.parent).rect.width, minScrollPos);
    }

    public void SetScrollPosition(float position)
    {
        float clampedPosition = Mathf.Clamp(position, minScrollPos, maxScrollPos);
        contentPanel.anchoredPosition = new Vector2(-clampedPosition, contentPanel.anchoredPosition.y);
    }

    void PushRigtAddButton()
    {
        if (null != itemList)
            button.GetComponent<RectTransform>().SetAsLastSibling();
    }
}
