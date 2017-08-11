using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnZone)), CanEditMultipleObjects]
public class SpawnZoneEditor : Editor
{
    private void DrawSpawnZoneInspector()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_startDisabled"));

        EditorGUILayout.LabelField("Quantities", EditorStyles.boldLabel);

        SerializedProperty minAliveEnemies = serializedObject.FindProperty("_minAliveEnemies");
        SerializedProperty maxAliveEnemies = serializedObject.FindProperty("_maxAliveEnemies");

        if (minAliveEnemies != null && maxAliveEnemies != null)
        {
            EditorGUILayout.PropertyField(minAliveEnemies);
            EditorGUILayout.PropertyField(maxAliveEnemies);
        }

        EditorGUILayout.LabelField("Waves", EditorStyles.boldLabel);

        SerializedProperty poolSize = serializedObject.FindProperty("_enemyPoolSize");
        SerializedProperty restTime = serializedObject.FindProperty("_restTime");
        SerializedProperty spawnToMax = serializedObject.FindProperty("_spawnToMax");
        SerializedProperty waveSize = serializedObject.FindProperty("_waveSize");

        if (poolSize != null)
            EditorGUILayout.PropertyField(poolSize);

        if (restTime != null)
            EditorGUILayout.PropertyField(restTime);

        if (spawnToMax != null)
        {
            EditorGUILayout.PropertyField(spawnToMax);

            if (!spawnToMax.boolValue && waveSize != null)
                EditorGUILayout.PropertyField(waveSize);
        }
        else if (waveSize != null)
            EditorGUILayout.PropertyField(waveSize);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSpawnZoneInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
