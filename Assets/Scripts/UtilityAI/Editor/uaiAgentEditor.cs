using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace JakePerry
{
    [CustomEditor(typeof(uaiAgent)), CanEditMultipleObjects]
    public class uaiAgentEditor : Editor
    {

        private ReorderableList properties = null;

        private void OnEnable()
        {
            // Create a new reorderable list for properties in the inspector
            properties = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_properties"),
                                true, true, true, true);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float singleLineHeightDoubled = singleLineHeight * 2;

            // Lambda function for drawing header. This simply uses a label field to write Considerations as the list header
            properties.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Properties"); };

            // Allow each element enough space for three lines, plus some padding
            properties.elementHeight = singleLineHeightDoubled + singleLineHeight + 8;

            // Add delegate for the drop-down 'add element' button
            properties.onAddDropdownCallback += DropDownAdd;
        }

        private void DropDownAdd(Rect buttonRect, ReorderableList list)
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            // Draw the properties list
            //if (properties != null)
            //    properties.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();
        }
    }
}
