using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FocusControll : MonoBehaviour 
{
    [SerializeField] private Selectable[] selectables;
    [SerializeField] private UIButton nextButton;

    private int selectedIdx = 0;
    private Coroutine focusUpdate = null;

    private void OnEnable()
    {
        StartCoroutine(SetDefFocus());
        focusUpdate = StartCoroutine(FocusUpdate());
    }

    private void OnDisable()
    {
        StopCoroutine(focusUpdate);
    }

    private IEnumerator SetDefFocus()
    {
        yield return new WaitForEndOfFrame();
        for (int index = 0; index < selectables.Length; index++)
        {
            if (selectables[index].gameObject.activeSelf && selectables[index].IsInteractable())
            {
                selectables[index].Select();
                selectedIdx = index;
                break;
            }
        }
    }

    private IEnumerator FocusUpdate()
    {
        while (true)
        {
            KeyDownBind();
            yield return new WaitForSeconds(0);
        }
    }

    private void KeyDownBind()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
        {
            int idx = 0;
            if (selectedIdx <= 0)
                idx = selectables.Length - 1;
            else
                idx = selectedIdx - 1;

            for (int index = idx; index < selectables.Length; index--)
            {
                if (selectables[index].gameObject.activeSelf && selectables[index].IsInteractable())
                {
                    selectables[index].Select();
                    selectedIdx = index;
                    break;
                }
                if (index == 0)
                    index = selectables.Length - 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            int idx = 0;
            if (selectedIdx >= selectables.Length - 1)
                idx = 0;
            else
                idx = selectedIdx + 1;

            for (int index = idx; index < selectables.Length; index++)
            {
                if (selectables[index].gameObject.activeSelf && selectables[index].IsInteractable())
                {
                    selectables[index].Select();
                    selectedIdx = index;
                    break;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            //if (null != nextButton)
            //{
            //    nextButton.ClickCallback?.Invoke(nextButton.ButtonData);
            //}
        }

        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var current = EventSystem.current;
            var selection = current?.currentSelectedGameObject;
            if (selection)
            {
                var selectable = selection.GetComponent<Selectable>();
                if(selectable)
                {
                    FindSelectionIndex(selectable);
                }
            }
            else
            {
                //Toggle/Toggle Group은 EventSystem에 잡히지 않기 때문에, pointer에서 Raycast를 해야 찾을 수 있다.
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if (raycastResults.Count > 0)
                {
                    foreach (var go in raycastResults)
                    {
                        var toggle = go.gameObject.GetComponentInParent<Toggle>();
                        if (toggle)
                        {
                            FindSelectionIndex(toggle);
                            break;
                        }
                    }
                }
            }

            void FindSelectionIndex(Selectable selectable)
            {
                for (int idx = 0; idx < selectables.Length; idx++)
                {
                    if (selectable == selectables[idx])
                    {
                        selectable.Select();
                        selectedIdx = idx;
                        break;
                    }
                }
            }
        }
    }
}