using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SpawnNode))]
public class SpawnNodeEditor : Editor
{

    private ReorderableList spawnables = null;

    private void OnEnable()
    {
        // Create a new reorderable list for spawnable enemies in the inspector
        spawnables = new ReorderableList(serializedObject,
            serializedObject.FindProperty("_spawnables"),
                            true, true, true, true);

        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        float singleLineHeightDoubled = singleLineHeight * 2;

        // Lambda function for drawing header
        spawnables.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Spawnable Enemies"); };

        spawnables.elementHeight = singleLineHeightDoubled + 6;

        spawnables.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = spawnables.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, singleLineHeight),
                                        element.FindPropertyRelative("_enemy"),
                                        new GUIContent("Enemy"));

                SerializedProperty chanceProperty = element.FindPropertyRelative("_chance");
                if (chanceProperty != null)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + singleLineHeight + 3, rect.width, singleLineHeight),
                                            chanceProperty,
                                            new GUIContent("Chance To Spawn"));

                    if (chanceProperty.floatValue < 0)
                        chanceProperty.floatValue = 0;
                }
            };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // Draw spawnables list
        if (spawnables != null)
            spawnables.DoLayoutList();

        // Show toggle for getClosestZone boolean
        EditorGUILayout.Space();
        SerializedProperty getClosestZoneProperty = serializedObject.FindProperty("_getClosestZone");
        EditorGUILayout.PropertyField(getClosestZoneProperty);

        // Show field for zone if not being found automatically
        if (!getClosestZoneProperty.boolValue)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_zone"));

        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }
}
