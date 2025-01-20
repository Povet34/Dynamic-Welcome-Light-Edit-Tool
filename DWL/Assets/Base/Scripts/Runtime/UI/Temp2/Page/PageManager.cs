using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public const int DEFAULT_PAGE_INDEX = 0;
    private const int UI_CELL_COUNT = 9;
    private const int UI_PAGE_GROUP_COUNT = 5;

    [SerializeField] private GameObject[] buttons;
    [SerializeField] Text[] buttonTexts;

    private Action<int> refresh;
    private int pageIndex;
    private int totalCellCount;

    public void Initialize(Action<int> refresh, int pageIndex, int totalCellCount)
    {
        this.refresh = refresh;
        this.pageIndex = pageIndex;
        this.totalCellCount = totalCellCount;
        RefreshData();
    }

    public int GetCellGapCount()
    {
        return GetLastCellIndex() - GetFirstCellIndex() + 1;
    }

    private void RefreshData()
    {
        foreach (var b in buttons)
            b.SetActive(false);
        for(int index = 0; index < GetPageGapCount(); index++)
        {
            buttons[index].SetActive(true);
            buttonTexts[index].text = $"{GetFirstPageIndex() + 1 + index}";
            
            if(GetFirstPageIndex() + index == pageIndex)
                buttonTexts[index].color = GetSelectedColor();
            else
                buttonTexts[index].color = GetDefaultColor();
        }
    }

    private int GetFirstPageIndex()
    {
        return (pageIndex / UI_PAGE_GROUP_COUNT) * UI_PAGE_GROUP_COUNT;
    }

    private int GetLastPageIndex()
    {
        if (IsLastPageGroup())
            return GetTotalPageCount() - 1;
        else
            return GetFirstPageIndex() + (UI_PAGE_GROUP_COUNT - 1);
    }

    private int GetPageGapCount()
    {
        return GetLastPageIndex() - GetFirstPageIndex() + 1;
    }

    private Color GetDefaultColor()
    {
        ColorUtility.TryParseHtmlString("#52575CFF", out Color c);
        return c;
    }

    private Color GetSelectedColor()
    {
        ColorUtility.TryParseHtmlString("#4AA5BAFF", out Color c);
        return c;
    }

    private int GetFirstCellIndex()
    {
        return pageIndex * UI_CELL_COUNT;
    }

    private int GetLastCellIndex()
    {
        if (IsLastPage())
            return totalCellCount - 1;
        else
            return GetFirstCellIndex() + (UI_CELL_COUNT - 1);
    }

    private int GetTotalPageCount()
    {
        return ((totalCellCount - 1) / UI_CELL_COUNT) + 1;
    }

    private bool IsLastPage()
    {
        return pageIndex == GetTotalPageCount() - 1;
    }

    private bool IsLastPageGroup()
    {
        return (pageIndex / UI_PAGE_GROUP_COUNT) == (GetTotalPageCount() / UI_PAGE_GROUP_COUNT);
    }


    #region UI Event : --------------------------------------------------------
    public void OnClickPrev()
    {
        if (pageIndex > DEFAULT_PAGE_INDEX)
        {
            pageIndex--;
            RefreshData();
            refresh?.Invoke(pageIndex);
        }
    }

    public void OnClickNext()
    {
        if (IsLastPage() == false)
        {
            pageIndex++;
            RefreshData();
            refresh?.Invoke(pageIndex);
        }
    }

    public void OnClickNumber(int uiIndex)
    {
        var targetIndex = GetFirstPageIndex() + uiIndex;
        if (pageIndex != targetIndex)
        {
            pageIndex = targetIndex;
            RefreshData();
            refresh?.Invoke(pageIndex);
        }
    }
    #endregion
}