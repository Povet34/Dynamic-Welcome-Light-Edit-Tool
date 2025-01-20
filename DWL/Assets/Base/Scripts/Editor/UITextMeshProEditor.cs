namespace UIEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro.EditorUtilities;
    using UnityEditor;
    using UnityEditor.UI;
    using UnityEngine;

    [CustomEditor(typeof(UITextMeshPro))]
    public class UITextMeshProEditor : TMP_EditorPanelUI
    {
        public override void OnInspectorGUI()
        {
            UITextMeshPro targetText = (UITextMeshPro)target;
            EditorGUILayout.LabelField("━━━  Custom Function  ━━━");
            targetText.IsApplyLocalText = EditorGUILayout.Toggle("Apply Local Text", targetText.IsApplyLocalText || !string.IsNullOrEmpty(targetText.LocalTextKey));
            targetText.LocalTextKey = EditorGUILayout.TextField("Local Text Code", targetText.LocalTextKey);

            Color originalColor = EditorStyles.boldLabel.normal.textColor;
            EditorStyles.boldLabel.normal.textColor = Color.yellow;
            EditorGUILayout.LabelField("ContentSizeFitter가 있는 경우 Check 시 작동", EditorStyles.boldLabel);
            targetText.IsAutoFitable = EditorGUILayout.Toggle("Auto Fitable", targetText.IsAutoFitable);

            EditorStyles.boldLabel.normal.textColor = originalColor;
            EditorGUILayout.LabelField(string.Empty);

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(targetText.gameObject);

            base.OnInspectorGUI();
        }
    }
}