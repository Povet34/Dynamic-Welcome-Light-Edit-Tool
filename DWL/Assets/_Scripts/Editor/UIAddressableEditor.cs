using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// UI �����͸� Inspectorâ���� �����ϱ� ���� Editor
/// </summary>
[CustomEditor(typeof(UIAddressable))]
public class UIAddressableEditor : Editor
{
    UIAddressable uiAddressable;

    private void OnEnable()
    {
        uiAddressable = (UIAddressable)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /// UI_TYPE�� ���� �� ��� ���� UI �����Ϳ��� ������ UI�� ������
        if (GUILayout.Button("Check"))
        {
            bool isRefresh = false;
            bool isValid = false;
            UIAddressableData uiAddressableData;
            for (int i = uiAddressable.uiList.Count - 1; i >= 0; --i)
            {
                isValid = false;
                uiAddressableData = uiAddressable.uiList[i];
                foreach (eUIType type in Enum.GetValues(typeof(eUIType)))
                {
                    if (uiAddressableData.uiType.Equals(type))
                    {
                        isValid = true;
                        break;
                    }
                }

                if (isValid == false)
                {
                    uiAddressable.uiList.RemoveAt(i);
                    isRefresh = true;
                }
            }

            if (isRefresh)
            {
                EditorUtility.SetDirty(uiAddressable);
            }
        }
    }
}