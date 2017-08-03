using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    private void OnEnable()
    {

    }

    private void DrawHealthInspector()
    {
        SerializedProperty maxHealth = serializedObject.FindProperty("_maxHealth");
        if (maxHealth != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);

            // Draw max health slider
            EditorGUILayout.PropertyField(maxHealth);

            SerializedProperty startRandom = serializedObject.FindProperty("_startRandom");
            if (startRandom != null)
            {
                // Draw random start toggle
                EditorGUILayout.PropertyField(startRandom);

                if (startRandom.boolValue)
                {
                    SerializedProperty min = serializedObject.FindProperty("_startMinimum");
                    SerializedProperty max = serializedObject.FindProperty("_startMaximum");

                    if (min != null && max != null)
                    {
                        // Track previous values
                        int minValue = min.intValue;
                        int maxValue = max.intValue;

                        // Indent rect
                        EditorGUI.indentLevel += 1;

                        // Display min & max starting values
                        EditorGUILayout.PropertyField(min);
                        EditorGUILayout.PropertyField(max);

                        // Reset indent
                        EditorGUI.indentLevel -= 1;

                        // Clamp min & max values
                        int newMinValue = min.intValue;
                        int newMaxValue = max.intValue;

                        if (newMaxValue > maxHealth.intValue)   // Maximum cannot be more than maxHealth
                            max.intValue = maxHealth.intValue;

                        if (newMinValue > maxHealth.intValue)   // Minimum cannot be more than maxHealth
                            min.intValue = maxHealth.intValue;

                        if (newMinValue > maxValue) // Minimum cannot be more than maximum
                            min.intValue = maxValue;

                        if (newMaxValue < minValue) // Maximum cannot be less than minimum
                            max.intValue = minValue;
                    }
                }
                else
                {
                    SerializedProperty startingHealth = serializedObject.FindProperty("_startingHealth");
                    if (startingHealth != null)
                    {
                        // Draw starting health slider
                        EditorGUILayout.PropertyField(startingHealth);

                        // Clamp value
                        if (startingHealth.intValue > maxHealth.intValue)
                            startingHealth.intValue = maxHealth.intValue;
                    }
                }
            }

            SerializedProperty rechargeWhenLow = serializedObject.FindProperty("_rechargeWhenLow");
            if (rechargeWhenLow != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Recharging", EditorStyles.boldLabel);

                // Draw recharge toggle
                EditorGUILayout.PropertyField(rechargeWhenLow);

                if (rechargeWhenLow.boolValue)
                {
                    SerializedProperty lowThreshold = serializedObject.FindProperty("_lowThresholdRecharge");
                    SerializedProperty delay = serializedObject.FindProperty("_delayAfterDamage");
                    SerializedProperty gain = serializedObject.FindProperty("_gainPerSecond");

                    if (lowThreshold != null && delay != null && gain != null)
                    {
                        // Indent rect
                        EditorGUI.indentLevel += 1;

                        // Draw properties
                        EditorGUILayout.PropertyField(lowThreshold);
                        EditorGUILayout.PropertyField(delay);
                        EditorGUILayout.PropertyField(gain);
                        
                        // Reset indent
                        EditorGUI.indentLevel -= 1;
                    }
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawHealthInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
