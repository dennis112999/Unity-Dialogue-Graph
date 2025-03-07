using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

using System;

using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueGraphWindow : EditorWindow
    {
        private DialogueView _dialogueView;
        private string _fileName = "New Narrative";

        private Blackboard _blackBoard;

        [MenuItem("Graph/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraphWindow>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
            GenerateBlackBoard();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_dialogueView);
        }

        /// <summary>
        /// Construct Graph View -> Only one time
        /// </summary>
        private void ConstructGraphView()
        {
            _dialogueView = new DialogueView(this)
            {
                name = "Dialogue Graph"
            };

            _dialogueView.StretchToParentSize();
            rootVisualElement.Add(_dialogueView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            // Create FileNameText
            var fileNameTextField = new TextField("File Name :");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            // Add Save Data Button and Load Data Button
            Button SaveDataButton = UIHelper.CreateButton("Save Data", () => RequestDataOperation(true));
            toolbar.Add(SaveDataButton);

            Button LoadDataButton = UIHelper.CreateButton("Load Data", () => RequestDataOperation(false));
            toolbar.Add(LoadDataButton);

            Button clearButton = UIHelper.CreateButton("Clear", () => ClearNode());
            toolbar.Add(clearButton);

            rootVisualElement.Add(toolbar);
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };

            Action updateMiniMapPosition = () =>
            {
                float windowWidth = _dialogueView.contentContainer.layout.width;

                if (float.IsNaN(windowWidth))
                {
                    Debug.LogError("Window width is NaN. Layout may not be updated.");
                }
                else
                {
                    var cards = _dialogueView.contentViewContainer.WorldToLocal(new Vector2(windowWidth - 210, 50));
                    miniMap.SetPosition(new Rect(cards.x, cards.y, 200, 140));
                }
            };

            // Schedule the initial position setup
            _dialogueView.schedule.Execute(updateMiniMapPosition).ExecuteLater(0);

            // Listen to the GeometryChangedEvent to update the position when the window size changes
            _dialogueView.RegisterCallback<GeometryChangedEvent>(evt => updateMiniMapPosition());

            _dialogueView.Add(miniMap);
        }

        #region Black Board

        private void GenerateBlackBoard()
        {
            _blackBoard = new Blackboard(_dialogueView);

            ResetBlackboard();

            _blackBoard.SetPosition(new Rect(10, 30, 200, 300));

            _blackBoard.addItemRequested = (_) => ShowSOSelectionWindow();

            _dialogueView.Add(_blackBoard);
        }

        private void ShowSOSelectionWindow()
        {
            EditorGUIUtility.ShowObjectPicker<VariableData>(null, false, "", 0);

            EditorApplication.update += CheckSOSelection;
        }

        private void CheckSOSelection()
        {
            VariableData selectedData = EditorGUIUtility.GetObjectPickerObject() as VariableData;

            if (selectedData != null)
            {
                AddVariableDataToBlackboard(selectedData);

                EditorApplication.update -= CheckSOSelection;
            }
        }

        private void AddVariableDataToBlackboard(VariableData variableData)
        {
            ResetBlackboard();

            foreach (var variable in variableData.Variables)
            {
                var field = new BlackboardField { text = $"{variable.Name} = {variable.Value}", typeText = "Float" };
                _blackBoard.Add(field);
            }
        }

        private void ResetBlackboard()
        {
            _blackBoard.Clear();

            _blackBoard.Add(new BlackboardSection { title = "Variable Datas" });

            BlackboardSection section = new BlackboardSection { title = "Variables" };
            _blackBoard.Add(section);
        }

        #endregion Black Board

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(_fileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name", "OK");
            }
            else
            {
                var saveUtility = GraphSaveUtility.GetInstance(_dialogueView);

                if(save)
                {
                    saveUtility.SaveGraph(_fileName);
                }
                else
                {
                    saveUtility.LoadGraph(_fileName);
                }
            }
        }

        private void ClearNode()
        {
            var saveUtility = GraphSaveUtility.GetInstance(_dialogueView);
            saveUtility.ClearGraph();
        }
    }

}