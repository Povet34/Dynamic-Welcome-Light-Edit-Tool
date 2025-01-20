using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightArrowOnOff : MonoBehaviour
{
    public Button button;

    public GameObject objOn;
    public GameObject objOff;

    // Update is called once per frame
    void Update()
    {
        if(button.interactable)
        {
            objOn.SetActive(true);
            objOff.SetActive(false);
        }
        else
        {
            objOn.SetActive(false);
            objOff.SetActive(true);
        }
    }
}
