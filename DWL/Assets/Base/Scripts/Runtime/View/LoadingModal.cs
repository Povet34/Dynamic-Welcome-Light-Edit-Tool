using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingModal : MonoBehaviour
{
    [SerializeField] private Text descriptionText;

    public void Show(string descriptionText)
    {
        gameObject.SetActive(true);
        this.descriptionText.text = descriptionText;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}