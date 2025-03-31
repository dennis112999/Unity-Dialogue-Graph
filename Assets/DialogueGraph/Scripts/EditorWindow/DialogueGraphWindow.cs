#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Callbacks;

using System;

using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph
{
    public class DialogueGraphWindow : EditorWindow
    {
        private DialogueContainer _currentDialogueContainer;

        private DialogueView _dialogueView;

        private Label _nameOfDialougeContainer;

        private Blackboard _blackBoard;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset(0)]
        public static bool OpenDialogueGraph(int instanceID, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceID);

            if (item is DialogueContainer dialogueContainer)
            {
                // Make a unity editor window of type DialogueEditorWindow
                DialogueGraphWindow window = (DialogueGraphWindow)GetWindow(typeof(DialogueGraphWindow));
                window.titleContent = new GUIContent("Dialogue Editor");
                window.minSize = new Vector2(500, 250);

                // Load in DialogueContainer data in to the editor window
                window.Load(dialogueContainer);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Will load in current selected dialogue container
        /// </summary>
        private void Load(DialogueContainer dialogueContainer)
        {
            _currentDialogueContainer = dialogueContainer;

            _nameOfDialougeContainer.text = "Name:  " + _currentDialogueContainer.name;

            var saveUtility = GraphSaveUtility.GetInstance(_dialogueView);
            saveUtility.LoadGraphByDialogueContainer(dialogueContainer);
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

            // Create a container for spacing adjustments
            var nameContainer = new VisualElement();
            nameContainer.style.flexDirection = FlexDirection.Row;
            nameContainer.style.alignItems = Align.Center;
            nameContainer.style.marginTop = 4;

            // Create Name Label
            _nameOfDialougeContainer = UIHelper.CreateLabel("Name:  ");
            nameContainer.Add(_nameOfDialougeContainer);

            toolbar.Add(nameContainer);

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
            miniMap.SetPosition(new Rect(10, 10, 200, 140));

            Action updateMiniMapPosition = () =>
            {
                float windowWidth = _dialogueView.contentContainer.layout.width;

                if (!float.IsNaN(windowWidth))
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
            var saveUtility = GraphSaveUtility.GetInstance(_dialogueView);

            if (save)
            {
                saveUtility.SaveGraph(_currentDialogueContainer);
            }

            saveUtility.LoadGraphByDialogueContainer(_currentDialogueContainer);
        }

        private void ClearNode()
        {
            var saveUtility = GraphSaveUtility.GetInstance(_dialogueView);
            saveUtility.ClearGraph();
        }
    }

}

#endif