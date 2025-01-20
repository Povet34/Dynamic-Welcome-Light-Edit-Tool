namespace UIEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(UIButton))]
    public class UIButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            UIButton targetButton = (UIButton)target;
            EditorGUILayout.LabelField("收收收  Custom Function  收收收");
            targetButton.MainText = (Text)EditorGUILayout.ObjectField("MainText", targetButton.MainText, typeof(Text), true);
            targetButton.ColorNormal = EditorGUILayout.ColorField("NormalColor", targetButton.ColorNormal);
            targetButton.ColorHighlighted = EditorGUILayout.ColorField("HighlightedColor", targetButton.ColorHighlighted);
            targetButton.ColorPressed = EditorGUILayout.ColorField("PressedColor", targetButton.ColorPressed);
            targetButton.ColorSelected = EditorGUILayout.ColorField("SelectedColor", targetButton.ColorSelected);
            targetButton.ColorDisabled = EditorGUILayout.ColorField("DisabledColor", targetButton.ColorDisabled);
            EditorGUILayout.LabelField(string.Empty);
            base.OnInspectorGUI();
        }
    }
}