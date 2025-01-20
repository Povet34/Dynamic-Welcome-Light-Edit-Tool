using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(Camera2D))]
public class Camera2DEditor : Editor
{
    Camera2D _cam2D;
    Vector2 _camSize = Vector2.zero;

    private void OnEnable()
    {
        _cam2D = (Camera2D)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /// ī�޶��� �ּ� ���� ũ��
        _cam2D.minWidth = EditorGUILayout.IntField("min width", _cam2D.minWidth);
        /// ī�޶��� �ּ� ���� ũ��
        _cam2D.minHeight = EditorGUILayout.IntField("min height", _cam2D.minHeight);
        /// ���� ũ�� ������ ��� true, ���� ũ�� ������ ��� false
        _cam2D.matchWidth = EditorGUILayout.Toggle("match width", _cam2D.matchWidth);

        /// ���ø����̼� �÷��� ��, �ػ󵵿� ���� ������ ��ũ���� ũ�⸦ ������ 
        if (UnityEngine.Application.isPlaying)
        {
            EditorGUILayout.IntField("screen width", _cam2D.screenWidth);
            EditorGUILayout.IntField("screen height", _cam2D.screenHeight);
        }
    }
}