#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Dennis.Tools.DialogueGraph.Data;
using UnityEditor;

namespace Dennis.Tools.DialogueGraph.Editor
{
    [CustomEditor(typeof(VariableData))]
    public class VariableDataEditor : UnityEditor.Editor
    {
        private SerializedProperty _variables;
        private Dictionary<int, bool> _foldoutStates = new Dictionary<int, bool>();

        private void OnEnable()
        {
            // Get reference to the "Variables" property in VariableData
            _variables = serializedObject.FindProperty("Variables");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Title
            EditorGUILayout.LabelField("Variable Data", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Button to refresh variable dictionary
            if (GUILayout.Button("Refresh Variables", GUILayout.Height(25)))
            {
                RefreshVariableDictionary();
            }

            EditorGUILayout.Space(10);
            DrawVariablesList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawVariablesList()
        {
            HashSet<string> uniqueNames = new HashSet<string>();

            for (int i = 0; i < _variables.arraySize; i++)
            {
                SerializedProperty variable = _variables.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = variable.FindPropertyRelative("Name");
                SerializedProperty valueProp = variable.FindPropertyRelative("Value");

                // Store foldout state for each variable
                if (!_foldoutStates.ContainsKey(i))
                {
                    _foldoutStates[i] = false;
                }

                EditorGUILayout.BeginVertical("box");

                // Toggle foldout
                _foldoutStates[i] = EditorGUILayout.Foldout(_foldoutStates[i], nameProp.stringValue, true);

                if (_foldoutStates[i])
                {
                    EditorGUI.indentLevel++;

                    // Edit variable name and check for duplicates
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(nameProp, new GUIContent("Variable Name"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();

                        if (!uniqueNames.Add(nameProp.stringValue))
                        {
                            EditorUtility.DisplayDialog(
                                "Duplicate Variable Name",
                                $"The variable name '{nameProp.stringValue}' is already in use.\nPlease choose a unique name.",
                                "OK"
                            );
                        }
                    }

                    // Edit variable value
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(valueProp, new GUIContent("Value"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                    }

                    // Button to delete variable
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Delete", GUILayout.Width(80)))
                    {
                        _variables.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(5);

            // Button to add a new variable
            if (GUILayout.Button("+ Add Variable", GUILayout.Height(25)))
            {
                AddNewVariable();
            }
        }

        private void AddNewVariable()
        {
            string baseName = "NewVariable";
            int counter = 1;

            // Get all existing variable names
            HashSet<string> existingNames = new HashSet<string>(
                Enumerable.Range(0, _variables.arraySize)
                          .Select(i => _variables.GetArrayElementAtIndex(i)
                                                 .FindPropertyRelative("Name").stringValue)
            );

            // Find a unique name
            string newVariableName = baseName;
            while (existingNames.Contains(newVariableName))
            {
                newVariableName = $"{baseName}{counter++}";
            }

            // Add new variable with default values
            _variables.arraySize++;
            SerializedProperty newVariable = _variables.GetArrayElementAtIndex(_variables.arraySize - 1);
            newVariable.FindPropertyRelative("Name").stringValue = newVariableName;
            newVariable.FindPropertyRelative("Value").floatValue = 0;
        }

        private void RefreshVariableDictionary()
        {
            VariableData variableData = (VariableData)target;

            // Find and invoke the private method "InitializeDictionary"
            var initMethod = typeof(VariableData).GetMethod(
                "InitializeDictionary",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            initMethod?.Invoke(variableData, null);

            // Check for duplicate variable names
            HashSet<string> uniqueNames = new HashSet<string>();
            foreach (var entry in variableData.Variables)
            {
                if (!uniqueNames.Add(entry.Name))
                {
                    EditorUtility.DisplayDialog(
                        "Duplicate Variable Name",
                        $"The variable name '{entry.Name}' is already in use.\nPlease choose a unique name.",
                        "OK"
                    );
                    return;
                }
            }
        }
    }
}

#endif
