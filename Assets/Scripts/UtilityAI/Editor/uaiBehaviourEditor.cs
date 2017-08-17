using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace JakePerry
{
    [CustomEditor(typeof(uaiBehaviour))]
    public class uaiBehaviourEditor : Editor
    {

        private ReorderableList considerations = null;

        private void OnEnable()
        {
            // Create a new reorderable list for considerations in the inspector
            considerations = new ReorderableList(serializedObject, 
                serializedObject.FindProperty("_considerations"),
                                true, true, true, true);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float singleLineHeightDoubled = singleLineHeight * 2;

            // Lambda function for drawing header. This simply uses a label field to write Considerations as the list header
            considerations.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Considerations"); };

            // Allow each element enough space for two lines, plus some padding
            considerations.elementHeight = singleLineHeightDoubled + 8;

            // Lambda function for laying out each element in the list
            considerations.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = considerations.serializedProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;

                    EditorGUI.PropertyField(new Rect(rect.x + 20, rect.y, rect.width - 70.0f, singleLineHeight),
                                            element.FindPropertyRelative("_propertyName"),
                                            GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, 20, singleLineHeight),
                                            element.FindPropertyRelative("_enabled"),
                                            GUIContent.none);
                    
                    // Width field + label
                    SerializedProperty weightProperty = element.FindPropertyRelative("_weight");
                    if (weightProperty != null)
                    {
                        // Three quarters the width, accounting for animationCurve space
                        float width3Q = (rect.width - 50.0f) * 0.75f;

                        // Draw float value field
                        float fieldStartX = rect.x + width3Q;
                        EditorGUI.PropertyField(new Rect(fieldStartX, rect.y + singleLineHeight, rect.width - width3Q - 50.0f, singleLineHeight),
                                                weightProperty,
                                                GUIContent.none);

                        // Draw label
                        EditorGUI.LabelField(new Rect(fieldStartX - 44.0f, rect.y + singleLineHeight, 
                                                44.0f, singleLineHeight),
                                            new GUIContent("Weight", "How highly is this consideration weighted?"));

                        // Clamp value
                        weightProperty.floatValue = Mathf.Clamp(weightProperty.floatValue, 0.01f, 10.0f);
                    }

                    EditorGUI.PropertyField(new Rect(rect.width, rect.y, singleLineHeightDoubled, singleLineHeightDoubled),
                                            element.FindPropertyRelative("_priority"),
                                            GUIContent.none);
                };

            // Add delegate for adding a new consideration element
            considerations.onAddCallback += AddNewConsideration;
        }

        private void AddNewConsideration(ReorderableList r)
        {
            SerializedProperty property = r.serializedProperty;

            if (property.isArray)
            {
                int arraySize = property.arraySize;

                // Insert a new element
                property.InsertArrayElementAtIndex(arraySize);

                SerializedProperty thisElement = property.GetArrayElementAtIndex(arraySize);

                thisElement.FindPropertyRelative("_priority").animationCurveValue =
                    new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));

                thisElement.FindPropertyRelative("_propertyName").stringValue = "Enter Property Name";

                thisElement.FindPropertyRelative("_enabled").boolValue = true;

                thisElement.FindPropertyRelative("_weight").floatValue = 1.0f;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();

            // Show help message to explain the purpose of considerations & action delegates lists
            EditorGUILayout.LabelField(new GUIContent("Hover for help", "Considerations list:\nA list of properties that will be considered by this behaviour.Use the animation curve to specify how each property will be weighted when the value is 0 - 1\n\nPre-Evaluation Delegates:\nA list of delegate functions which will be triggered immediately before the behaviour is evaluated by the agent.\n\nAction Delegates:\nA list of delegate functions which will be triggered by this behaviour when it is run."), 
                                        EditorStyles.boldLabel);

            // Draw name property field
            SerializedProperty behaviourNameProperty = serializedObject.FindProperty("_behaviourName");
            if (behaviourNameProperty != null)
                EditorGUILayout.TextField(new GUIContent("Behaviour Name"), behaviourNameProperty.stringValue);

            EditorGUILayout.Space();

            // Draw the consideration list
            if (considerations != null)
                considerations.DoLayoutList();

            // Draw current priority
            uaiBehaviour script = target as uaiBehaviour;
            float priority = script.Evaluate();
            ProgressBar(priority, "Current Priority");

            serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();
        }

        // ProgressBar code is taken from Unity Manual example on Editor page.
        private void ProgressBar(float value, string label)
        {
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
    }
}
