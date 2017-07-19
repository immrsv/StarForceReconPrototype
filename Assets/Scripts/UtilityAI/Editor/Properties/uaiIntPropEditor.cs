using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JakePerry
{
    [CustomPropertyDrawer(typeof(uaiIntProp))]
    public class uaiIntPropEditor : PropertyDrawer
    {
        // Override height for each property
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float linesPerProperty = 5.5f + ((property.FindPropertyRelative("_startRandom").boolValue) ? 1 : 0);
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * (linesPerProperty - 1);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            // Space the property from the top by two pixels
            rect.y += 2;
            rect.x += 10;

            float lineOneY = rect.y + singleLineHeight;
            float lineTwoY = rect.y + singleLineHeight + singleLineHeight;
            float lineThreeY = lineTwoY + singleLineHeight;

            // Draw line at top
            EditorGUI.DrawRect(new Rect(rect.x - 10, rect.y, rect.width, 1), Color.black);

            // Draw title
            EditorGUI.DropShadowLabel(new Rect(rect.x - 10, rect.y - 2, 100, singleLineHeight), "Int Property");

            // Name field
            EditorGUI.LabelField(new Rect(rect.x, lineOneY, 100, singleLineHeight), "Name:");
            EditorGUI.PropertyField(new Rect(rect.x + 50, lineOneY, rect.width - 78.0f, singleLineHeight),
                                    property.FindPropertyRelative("_name"),
                                    GUIContent.none);

            // Randomize variables
            EditorGUI.LabelField(new Rect(rect.x, lineTwoY, rect.width, singleLineHeight), "Start Random:");
            EditorGUI.PropertyField(new Rect(rect.x + 90.0f, lineTwoY, 20, singleLineHeight),
                                    property.FindPropertyRelative("_startRandom"),
                                    GUIContent.none);

            bool showRandomValues = property.FindPropertyRelative("_startRandom").boolValue;
            if (showRandomValues)
            {
                float startPosX = rect.x + 62;
                float fieldWidth = rect.width / 2 - startPosX - 10;

                EditorGUI.LabelField(new Rect(rect.x, lineThreeY, rect.width, singleLineHeight),
                                        new GUIContent("Min Start:", "The minimum (normalized) value this property can randomly start with."));
                EditorGUI.PropertyField(new Rect(startPosX, lineThreeY, rect.width / 2 - startPosX - 10, singleLineHeight),
                                        property.FindPropertyRelative("_minStartValue"),
                                        GUIContent.none);

                startPosX = rect.x + 62 + rect.width / 2;
                fieldWidth = rect.width - (startPosX) - 4;

                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, lineThreeY, rect.width, singleLineHeight),
                                        new GUIContent("Max Start:", "The maximum (normalized) value this property can randomly start with."));
                EditorGUI.PropertyField(new Rect(startPosX, lineThreeY, fieldWidth, singleLineHeight),
                                        property.FindPropertyRelative("_maxStartValue"),
                                        GUIContent.none);
            }

            float lineFourY = ((showRandomValues) ? lineThreeY : lineTwoY) + singleLineHeight;

            // Show property value
            EditorGUI.LabelField(new Rect(rect.x, lineFourY, 100, singleLineHeight),
                                    new GUIContent("Value:", "The current value of the property."));
            EditorGUI.PropertyField(new Rect(rect.x + 62, lineFourY, rect.width - 90, singleLineHeight),
                                   property.FindPropertyRelative("_value"),
                                    GUIContent.none);

            float lineFiveY = lineFourY + singleLineHeight;

            // Show value min & max values
            {
                float startPosX = rect.x + 70;
                float fieldWidth = rect.width / 2 - startPosX - 10;

                EditorGUI.LabelField(new Rect(rect.x, lineFiveY, 100, singleLineHeight),
                                        new GUIContent("Min Value:", "The minimum value allowed for this property."));
                EditorGUI.PropertyField(new Rect(startPosX, lineFiveY, fieldWidth, singleLineHeight),
                                        property.FindPropertyRelative("_minValue"),
                                        GUIContent.none);

                startPosX = rect.x + 70 + rect.width / 2;
                fieldWidth = rect.width - (startPosX) - 4;

                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, lineFiveY, rect.width, singleLineHeight),
                                        new GUIContent("Max Value:", "The maximum value allowed for this property."));
                EditorGUI.PropertyField(new Rect(startPosX, lineFiveY, fieldWidth, singleLineHeight),
                                        property.FindPropertyRelative("_maxValue"),
                                        GUIContent.none);
            }

            // Draw line at bottom
            EditorGUI.DrawRect(new Rect(rect.x - 10, rect.y + rect.height - 5, rect.width, 1), Color.black);

            EditorGUI.EndProperty();
        }
    }
}
