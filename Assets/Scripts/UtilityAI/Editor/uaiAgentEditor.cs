using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace JakePerry
{
    [CustomEditor(typeof(uaiAgent))]
    public class uaiAgentEditor : Editor
    {
        #region Variables

        private ReorderableList properties = null;
        private float singleLineHeight, singleLineHeightDoubled, lineOneY, lineTwoY, lineThreeY, lineFourY;

        #endregion

        private void DrawBoolProperty(Rect rect, SerializedProperty property)
        {
            // Title
            EditorGUI.LabelField(rect, "Boolean Property", EditorStyles.boldLabel);

            // Name field
            SerializedProperty nameProperty = property.FindPropertyRelative("_name");
            if (nameProperty != null)
            {
                nameProperty.stringValue = EditorGUI.TextField(new Rect(rect.x, lineOneY, rect.width, singleLineHeight), 
                                            new GUIContent("Name"), nameProperty.stringValue);
            }

            // Start Random
            SerializedProperty startRandomProperty = property.FindPropertyRelative("_startRandom");
            if (startRandomProperty != null)
            {
                startRandomProperty.boolValue = EditorGUI.ToggleLeft(new Rect(rect.x, lineTwoY, 97, singleLineHeight),
                                            new GUIContent("Start Random"), startRandomProperty.boolValue);
            }

            // Show value
            SerializedProperty valueProperty = property.FindPropertyRelative("_boolValue");
            if (valueProperty != null)
            {
                // TODO: If playing, show current value non editable. Else be editable
                valueProperty.boolValue = EditorGUI.ToggleLeft(new Rect(rect.x, lineThreeY, 97, singleLineHeight),
                                            new GUIContent("Current Value"), valueProperty.boolValue);
            }
        }
        private void DrawFloatProperty(Rect rect, SerializedProperty property)
        {
            // Title
            EditorGUI.LabelField(rect, "Float Property", EditorStyles.boldLabel);

            float rectWidthHalf = rect.width / 2;

            // Name field
            SerializedProperty nameProperty = property.FindPropertyRelative("_name");
            if (nameProperty != null)
            {
                nameProperty.stringValue = EditorGUI.TextField(new Rect(rect.x, lineOneY, rect.width, singleLineHeight),
                                            new GUIContent("Name"), nameProperty.stringValue);
            }

            // Start Random
            SerializedProperty startRandomProperty = property.FindPropertyRelative("_startRandom");
            SerializedProperty minRandomStart = property.FindPropertyRelative("_fminStartValue");
            SerializedProperty maxRandomStart = property.FindPropertyRelative("_fmaxStartValue");
            if (startRandomProperty != null)
            {
                startRandomProperty.boolValue = EditorGUI.ToggleLeft(new Rect(rect.x, lineTwoY, 97, singleLineHeight),
                                            new GUIContent("Start Random"), startRandomProperty.boolValue);

                // Show min & max if startRandom is true
                if (startRandomProperty.boolValue)
                {
                    if (minRandomStart != null && maxRandomStart != null)
                    {
                        // Get a vector2 struct storing these values
                        Vector2 startValues = new Vector2(minRandomStart.floatValue, maxRandomStart.floatValue);

                        startValues = EditorGUI.Vector2Field(new Rect(rect.x, lineThreeY, rect.width, singleLineHeight),
                                                new GUIContent("Min/Max Start", "The minimum and maximum allowed starting values.\nUse X for minimum & Y for maximum.\nThese will automatically be clamped to the overall min & max values if they exceed the limits."), 
                                                startValues);

                        minRandomStart.floatValue = startValues.x;
                        maxRandomStart.floatValue = startValues.y;

                        // Drop next lines down to make room
                        lineThreeY += singleLineHeight + 3;
                        lineFourY += singleLineHeight + 3;
                    }
                }
            }

            // Show values
            SerializedProperty valueProperty = property.FindPropertyRelative("_floatValue");
            SerializedProperty minValueProperty = property.FindPropertyRelative("_fminValue");
            SerializedProperty maxValueProperty = property.FindPropertyRelative("_fmaxValue");
            if (valueProperty != null)
            {
                if (minValueProperty != null && maxValueProperty != null)
                {
                    // Draw min & max value fields
                    Vector2 limitValues = new Vector2(minValueProperty.floatValue, maxValueProperty.floatValue);
                    limitValues = EditorGUI.Vector2Field(new Rect(rect.x, lineFourY, rect.width, singleLineHeight),
                                                new GUIContent("Min/Max Values", "The minimum and maximum allowed values.\nUse X for minimum and Y for maximum."), 
                                                limitValues);
                    
                    minValueProperty.floatValue = limitValues.x;
                    maxValueProperty.floatValue = limitValues.y;

                    // Draw value field as a slider if min & max values are not too different
                    float minVal = minValueProperty.floatValue;
                    float maxVal = maxValueProperty.floatValue;
                    float difference = Mathf.Abs(minVal - maxVal);
                    if (difference < 200)
                    {
                        EditorGUI.Slider(new Rect(rect.x, lineThreeY, rect.width, singleLineHeight),
                                valueProperty, minVal, maxVal, new GUIContent("Value"));
                    }
                    else
                    {
                        // If the value property was not already drawn as a slider, draw as a float input field
                        valueProperty.floatValue = EditorGUI.FloatField(new Rect(rect.x, lineThreeY, rectWidthHalf, singleLineHeight),
                                                        "Value", valueProperty.floatValue);

                        valueProperty.floatValue = Mathf.Clamp(valueProperty.floatValue, minVal, maxVal);
                    }
                }
            }
        }
        private void DrawIntProperty(Rect rect, SerializedProperty property)
        {
            // Title
            EditorGUI.LabelField(rect, "Integer Property", EditorStyles.boldLabel);

            float rectWidthHalf = rect.width / 2;
            float rectWidth045 = rect.width * 0.45f;
            float rectWidth183 = rect.width * 0.183f;

            // Name field
            SerializedProperty nameProperty = property.FindPropertyRelative("_name");
            if (nameProperty != null)
            {
                nameProperty.stringValue = EditorGUI.TextField(new Rect(rect.x, lineOneY, rect.width, singleLineHeight),
                                            new GUIContent("Name"), nameProperty.stringValue);
            }

            // Start Random
            SerializedProperty startRandomProperty = property.FindPropertyRelative("_startRandom");
            SerializedProperty minRandomStart = property.FindPropertyRelative("_iminStartValue");
            SerializedProperty maxRandomStart = property.FindPropertyRelative("_imaxStartValue");
            if (startRandomProperty != null)
            {
                startRandomProperty.boolValue = EditorGUI.ToggleLeft(new Rect(rect.x, lineTwoY, 97, singleLineHeight),
                                            new GUIContent("Start Random"), startRandomProperty.boolValue);

                // Show min & max if startRandom is true
                if (startRandomProperty.boolValue)
                {
                    if (minRandomStart != null && maxRandomStart != null)
                    {
                        // Draw min & max starting-value fields
                        EditorGUI.LabelField(new Rect(rect.x, lineThreeY, 90, singleLineHeight), 
                                                new GUIContent("Min/Max Start", "The minimum and maximum allowed starting values.\nUse X for minimum & Y for maximum.\nThese will automatically be clamped to the overall min & max values if they exceed the limits."));
                        EditorGUI.LabelField(new Rect(rect.x + rectWidth045 - 20, lineThreeY, 16, singleLineHeight),
                                            new GUIContent("X"));
                        EditorGUI.LabelField(new Rect(rect.x + rectWidth045 + rectWidth183 - 14, lineThreeY, 16, singleLineHeight),
                                                new GUIContent("Y"));

                        minRandomStart.intValue = EditorGUI.IntField(new Rect(rect.x + rectWidth045 - 8, lineThreeY,
                                                    rectWidth183 - 8, singleLineHeight),
                                                    GUIContent.none, minRandomStart.intValue);

                        maxRandomStart.intValue = EditorGUI.IntField(new Rect(rect.x + rectWidth045 + rectWidth183, lineThreeY,
                                                    rectWidth183 - 8, singleLineHeight),
                                                    GUIContent.none, maxRandomStart.intValue);

                        // Drop next lines down to make room
                        lineThreeY += singleLineHeight + 3;
                        lineFourY += singleLineHeight + 3;
                    }
                }
            }

            // Show values
            SerializedProperty valueProperty = property.FindPropertyRelative("_intValue");
            SerializedProperty minValueProperty = property.FindPropertyRelative("_iminValue");
            SerializedProperty maxValueProperty = property.FindPropertyRelative("_imaxValue");
            if (valueProperty != null)
            {
                if (minValueProperty != null && maxValueProperty != null)
                {
                    // Draw min & max value fields
                    EditorGUI.LabelField(new Rect(rect.x, lineFourY, 100, singleLineHeight), 
                                            new GUIContent("Min/Max Values", "The minimum and maximum allowed values.\nUse X for minimum and Y for maximum."));
                    EditorGUI.LabelField(new Rect(rect.x + rectWidth045 - 20, lineFourY, 16, singleLineHeight),
                                            new GUIContent("X"));
                    EditorGUI.LabelField(new Rect(rect.x + rectWidth045 + rectWidth183 - 14, lineFourY, 16, singleLineHeight),
                                            new GUIContent("Y"));

                    minValueProperty.intValue = EditorGUI.IntField(new Rect(rect.x + rectWidth045 - 8, lineFourY,
                                                    rectWidth183 - 8, singleLineHeight),
                                                    GUIContent.none, minValueProperty.intValue);

                    maxValueProperty.intValue = EditorGUI.IntField(new Rect(rect.x + rectWidth045 + rectWidth183, lineFourY,
                                                rectWidth183 - 8, singleLineHeight),
                                                GUIContent.none, maxValueProperty.intValue);

                    // Draw value field as a slider if min & max values are not too different
                    int minVal = minValueProperty.intValue;
                    int maxVal = maxValueProperty.intValue;
                    int difference = Mathf.Abs(minVal - maxVal);
                    if (difference < 200)
                    {
                        EditorGUI.IntSlider(new Rect(rect.x, lineThreeY, rect.width, singleLineHeight),
                                valueProperty, minVal, maxVal, new GUIContent("Value"));
                    }
                    else
                    {
                        // If the value property was not already drawn as a slider, draw as a float input field
                        valueProperty.intValue = EditorGUI.IntField(new Rect(rect.x, lineThreeY, rectWidthHalf, singleLineHeight),
                                                        "Value", valueProperty.intValue);

                        valueProperty.intValue = Mathf.Clamp(valueProperty.intValue, minVal, maxVal);
                    }
                }
            }
        }

        private void OnEnable()
        {
            // Create a new reorderable list for properties in the inspector
            properties = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_properties"),
                                true, true, true, true);

            singleLineHeight = EditorGUIUtility.singleLineHeight;
            singleLineHeightDoubled = singleLineHeight + singleLineHeight;

            // Lambda function for drawing header. This simply uses a label field to write Considerations as the list header
            properties.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Properties"); };

            // Allow each element enough space for five lines, plus some padding
            properties.elementHeight = singleLineHeightDoubled + singleLineHeightDoubled + singleLineHeightDoubled + 18;

            properties.drawElementCallback = 
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty element = properties.serializedProperty.GetArrayElementAtIndex(index);
                    uaiAgent agent = target as uaiAgent;

                    lineOneY = rect.y + singleLineHeight + 3;
                    lineTwoY = lineOneY + singleLineHeight + 3;
                    lineThreeY = lineTwoY + singleLineHeight + 3;
                    lineFourY = lineThreeY + singleLineHeight + 3;

                    if (agent != null)
                    {
                        uaiProperty currentProperty = agent.properties[index];

                        if (currentProperty.isBool)         DrawBoolProperty(rect, element);
                        else if (currentProperty.isFloat)   DrawFloatProperty(rect, element);
                        else if (currentProperty.isInt)     DrawIntProperty(rect, element);
                    }
                };

            // Add delegate for the drop-down 'add element' button
            properties.onAddDropdownCallback += AddNewProperty;

            properties.onRemoveCallback = (ReorderableList list) =>
            {
                if (EditorUtility.DisplayDialog("Warning!",
                    "Are you sure you want to delete this property from the agent?", "Yes", "No"))
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
            };
        }

        private void AddNewProperty(Rect rect, ReorderableList r)
        {
            // Get the property list being drawn
            uaiAgent agent = target as uaiAgent;
            List<uaiProperty> propertyList = agent.properties;

            if (propertyList != null)
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Bool"), false,
                    () =>
                    {
                        propertyList.Add(new uaiProperty(true));
                    }
                    );

                menu.AddItem(new GUIContent("Int"), false,
                    () =>
                    {
                        propertyList.Add(new uaiProperty(1));
                    }
                    );

                menu.AddItem(new GUIContent("Float"), false,
                    () =>
                    {
                        propertyList.Add(new uaiProperty(0.0f));
                    }
                    );

                menu.ShowAsContext();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            
            // Draw the properties list
            if (properties != null)
                properties.DoLayoutList();
            
            serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();
        }
    }
}
