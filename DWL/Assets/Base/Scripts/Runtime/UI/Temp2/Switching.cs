using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switching : MonoBehaviour 
{
    [SerializeField]
    private GameObject[] switchingObjArr;

    private void Awake()
    {
        foreach (var obj in switchingObjArr)
            obj.SetActive(false);
    }

    public void SetSwitchingActive(int index)
    {
        if (index >= 0 && index < switchingObjArr.Length)
        {
            for (int idx = 0; idx < switchingObjArr.Length; idx++)
            {
                bool isActive = index == idx;
                switchingObjArr[idx].SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// to do oink8407s : 분리 필요
    /// </summary>
    /// <param name="multiIndex"></param>
    public void SetSwitchingActive(int[] multiIndex)
    {
        foreach (var obj in switchingObjArr)
            obj.SetActive(false);

        foreach (var index in multiIndex)
        {
            if (index >= 0 && index < switchingObjArr.Length)
                switchingObjArr[index].SetActive(true);
        }
    }

    public int GetActiveIndex()
    {
        for (int index = 0; index < switchingObjArr.Length; index++)
        {
            if (switchingObjArr[index].activeSelf)
                return index;
        }

        return 0;
    }
}