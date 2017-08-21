using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{

    private ReorderableList clips = null;

    private void OnEnable()
    {
        // Create a new reorderable list for audio clips in the inspector
        clips = new ReorderableList(serializedObject,
            serializedObject.FindProperty("_clips"),
                            true, true, true, true);

        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float singleLineHeightDoubled = singleLineHeight * 2;

        clips.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Audio Clips"); };

        clips.elementHeight = singleLineHeightDoubled + 4;

        clips.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = clips.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_name"));

                EditorGUI.PropertyField(new Rect(rect.x, rect.y + singleLineHeight, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_clip"));
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        if (clips != null)
            clips.DoLayoutList();
        
        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }
}
