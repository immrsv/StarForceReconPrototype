using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Gun))]
public class GunEditor : Editor
{

    private void DrawInspector()
    {
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);

        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty originProperty = serializedObject.FindProperty("_gunOrigin");
            if (originProperty != null)
                EditorGUILayout.PropertyField(originProperty);

            SerializedProperty damageProperty = serializedObject.FindProperty("_damage");
            if (damageProperty != null)
                EditorGUILayout.PropertyField(damageProperty);

            SerializedProperty spreadProperty = serializedObject.FindProperty("_spread");
            if (spreadProperty != null)
                EditorGUILayout.PropertyField(spreadProperty);

            SerializedProperty layerMaskProperty = serializedObject.FindProperty("_layerMask");
            if (layerMaskProperty != null)
                EditorGUILayout.PropertyField(layerMaskProperty);

            EditorGUI.indentLevel = indent;
        }

        EditorGUILayout.LabelField("Firing", EditorStyles.boldLabel);

        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty semiAutoProperty = serializedObject.FindProperty("_semiAuto");
            if (semiAutoProperty != null)
                EditorGUILayout.PropertyField(semiAutoProperty);

            SerializedProperty fireRateRPMProperty = serializedObject.FindProperty("_fireRateRPM");
            if (fireRateRPMProperty != null)
                EditorGUILayout.PropertyField(fireRateRPMProperty);

            EditorGUI.indentLevel = indent;
        }

        EditorGUILayout.LabelField("Ammo", EditorStyles.boldLabel);

        {
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;

            SerializedProperty bottomlessClipProperty = serializedObject.FindProperty("_bottomlessClip");
            if (bottomlessClipProperty != null)
            {
                EditorGUILayout.PropertyField(bottomlessClipProperty);

                if (!bottomlessClipProperty.boolValue)
                {
                    SerializedProperty clipSizeProperty = serializedObject.FindProperty("_clipSize");
                    if (clipSizeProperty != null)
                        EditorGUILayout.PropertyField(clipSizeProperty);
                }
            }

            SerializedProperty infiniteAmmoProperty = serializedObject.FindProperty("_infiniteAmmo");
            if (infiniteAmmoProperty != null)
            {
                EditorGUILayout.PropertyField(infiniteAmmoProperty);

                if (!infiniteAmmoProperty.boolValue)
                {
                    SerializedProperty startAmmoProperty = serializedObject.FindProperty("_startAmmo");
                    if (startAmmoProperty != null)
                        EditorGUILayout.PropertyField(startAmmoProperty);
                }
            }

            SerializedProperty ammoPerShotProperty = serializedObject.FindProperty("_ammoPerShot");
            if (ammoPerShotProperty != null)
                EditorGUILayout.PropertyField(ammoPerShotProperty);

            SerializedProperty reloadTimeProperty = serializedObject.FindProperty("_reloadTime");
            if (reloadTimeProperty != null)
                EditorGUILayout.PropertyField(reloadTimeProperty);

            EditorGUI.indentLevel = indent;
        }

        SerializedProperty useHeatProperty = serializedObject.FindProperty("_useHeat");
        if (useHeatProperty != null)
        {
            Rect r = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useHeatProperty, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUI.LabelField(new Rect(r.x + 13, r.y, r.width - 13, EditorGUIUtility.singleLineHeight), 
                                "Heat", EditorStyles.boldLabel);

            if (useHeatProperty.boolValue)
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;

                SerializedProperty heatLocksReloadProperty = serializedObject.FindProperty("_heatLocksReload");
                if (heatLocksReloadProperty != null)
                    EditorGUILayout.PropertyField(heatLocksReloadProperty);

                SerializedProperty overheatThresholdProperty = serializedObject.FindProperty("_overheatThreshold");
                if (overheatThresholdProperty != null)
                    EditorGUILayout.PropertyField(overheatThresholdProperty);

                SerializedProperty coolThresholdProperty = serializedObject.FindProperty("_coolThreshold");
                if (coolThresholdProperty != null)
                    EditorGUILayout.PropertyField(coolThresholdProperty);

                SerializedProperty heatPerShotProperty = serializedObject.FindProperty("_heatPerShot");
                if (heatPerShotProperty != null)
                    EditorGUILayout.PropertyField(heatPerShotProperty);

                SerializedProperty heatLossPerSecondProperty = serializedObject.FindProperty("_heatLossPerSecond");
                if (heatLossPerSecondProperty != null)
                    EditorGUILayout.PropertyField(heatLossPerSecondProperty);

                SerializedProperty coolingPauseTimeProperty = serializedObject.FindProperty("_coolingPauseTime");
                if (coolingPauseTimeProperty != null)
                    EditorGUILayout.PropertyField(coolingPauseTimeProperty);

                // Draw heat bar
                SerializedProperty heatProperty = serializedObject.FindProperty("_currentHeat");
                if (heatProperty != null)
                {
                    GUIContent guic = new GUIContent("heat: " + heatProperty.floatValue.ToString());
                    Rect rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(guic, "TextField"));
                    ProgressBar(rect, heatProperty.floatValue, guic.text);
                }

                EditorGUI.indentLevel = indent;
            }
        }
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void ProgressBar(Rect rect, float value, string label)
    {
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
