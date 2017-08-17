using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor
{

    private void DrawInspector()
    {
        // TODO

        // TEMP: Draw heat bar
        SerializedProperty heatProperty = serializedObject.FindProperty("_currentHeat");
        if (heatProperty != null)
        {
            ProgressBar(heatProperty.floatValue, "heat: " + heatProperty.floatValue.ToString());
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInspector();

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }

    private void ProgressBar(float value, string label)
    {
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
