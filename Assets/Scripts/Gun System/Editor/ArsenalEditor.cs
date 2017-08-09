using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Arsenal))]
public class ArsenalEditor : Editor
{

    private ReorderableList guns = null;

    void OnEnable()
    {
        guns = new ReorderableList(serializedObject,
            serializedObject.FindProperty("_guns"),
                            true, true, true, true);

        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        guns.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Guns"); };

        guns.elementHeight = singleLineHeight + singleLineHeight + 8;

        guns.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = guns.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_gun"));

                EditorGUI.PropertyField(new Rect(rect.x, rect.y + singleLineHeight, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_holsterPoint"));
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        if (guns != null)
            guns.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
