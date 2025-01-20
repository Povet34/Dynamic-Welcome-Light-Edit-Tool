namespace UIEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(UIToggle))]
    public class UIToggleEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            UIToggle targetToggle = (UIToggle)target;

            EditorGUILayout.LabelField("收收收  Custom Function  收收收");
            EditorGUILayout.BeginHorizontal();
            targetToggle.GoOnState = (GameObject)EditorGUILayout.ObjectField("OnObject", targetToggle.GoOnState, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            targetToggle.GoOffState = (GameObject)EditorGUILayout.ObjectField("OffObject", targetToggle.GoOffState, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            targetToggle.MainText = (Text)EditorGUILayout.ObjectField("MainText", targetToggle.MainText, typeof(Text), true);
            targetToggle.ColorNormal = EditorGUILayout.ColorField("NormalColor", targetToggle.ColorNormal);
            targetToggle.ColorHighlighted = EditorGUILayout.ColorField("HighlightedColor", targetToggle.ColorHighlighted);
            targetToggle.ColorPressed = EditorGUILayout.ColorField("PressedColor",  targetToggle.ColorPressed);
            targetToggle.ColorSelected = EditorGUILayout.ColorField("SelectedColor", targetToggle.ColorSelected);
            targetToggle.ColorDisabled = EditorGUILayout.ColorField("DisabledColor", targetToggle.ColorDisabled);
            EditorGUILayout.LabelField(string.Empty);

            base.OnInspectorGUI();
        }
    }
}