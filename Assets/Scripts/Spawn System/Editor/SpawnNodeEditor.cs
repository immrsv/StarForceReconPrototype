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

        spawnables.onAddCallback =
            (ReorderableList r) =>
            {
                // Add a new element
                r.serializedProperty.arraySize++;

                // Get a reference to the new element
                int arraySize = r.serializedProperty.arraySize;
                SerializedProperty element = r.serializedProperty.GetArrayElementAtIndex(arraySize - 1);

                if (element != null)
                {
                    SerializedProperty chance = element.FindPropertyRelative("_chance");
                    SerializedProperty prevChance = element.FindPropertyRelative("_prevChance");

                    if (chance != null && prevChance != null)
                    {
                        if (arraySize == 1)
                        {
                            // This is the first element
                            chance.floatValue = 100.0f;
                            prevChance.floatValue = 100.0f;
                        }
                        else
                        {
                            chance.floatValue = 0.0f;
                            prevChance.floatValue = 0.0f;
                        }
                    }
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
