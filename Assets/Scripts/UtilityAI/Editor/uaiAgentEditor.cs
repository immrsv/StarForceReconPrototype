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
            // TODO: Get height from propertydrawer somehow
            properties.elementHeight = singleLineHeightDoubled + singleLineHeight + 8;

            properties.drawElementCallback = 
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    // Call the propertyDrawer for the base property class
                    SerializedProperty element = properties.serializedProperty.GetArrayElementAtIndex(index);
                    //Debug.Log("array element type: " + properties.serializedProperty.arrayElementType);
                    //Debug.Log("Drawing property for: " + element);
                    EditorGUI.PropertyField(rect, element);
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
            var propertyList = agent.properties;

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
