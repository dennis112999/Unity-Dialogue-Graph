using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Dennis.Tools.DialogueGraph.Data;
using UnityEngine.UIElements;
using Dennis.Tools.DialogueGraph.Utilities;

namespace Dennis.Tools.DialogueGraph
{
    /// <summary>
    /// Base class for nodes that contain conditional data
    /// </summary>
    public abstract class BaseConditionNode<T> : BaseNode where T : BaseData
    {
        /// <summary>
        /// The current node data instance associated with this node
        /// </summary>
        protected T _currentNodeData;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseConditionNode() { }

        /// <summary>
        /// Adds a new condition to the node.
        /// This updates the UI and ensures that the condition is stored correctly
        /// </summary>
        protected void AddCondition()
        {
            // Ensure that the node data implements IConditionHolder
            if (!(_currentNodeData is IConditionHolder conditionHolder)) return;

            // Initialize or clone existing conditions
            if (conditionHolder.ConditionDatas == null)
            {
                conditionHolder.ConditionDatas = new List<ConditionData>();
            }
            else
            {
                conditionHolder.ConditionDatas = conditionHolder.ConditionDatas
                    .Select(DialogueCloneUtility.CloneCondition)
                    .ToList();
            }

            // Create a new condition
            ConditionData conditionData = new ConditionData();
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Initialize UI for the condition
            OnConditionTypeChanged(conditionData, boxContainer, ConditionType.IsTrue);

            // Add UI to the main container
            mainContainer.Add(boxContainer);
            conditionHolder.ConditionDatas.Add(conditionData);

            // Adjust UI visibility if needed
            ShowHideChoiceEnum();
        }

        /// <summary>
        /// Restores an existing condition into the UI
        /// </summary>
        /// <param name="conditionData">The condition data to restore</param>
        protected void RestoreCondition(ConditionData conditionData)
        {
            Box boxContainer = UIHelper.CreateBox("ConditionContainer");

            // Generate UI components based on existing condition data
            OnConditionTypeChanged(conditionData, boxContainer, conditionData.ConditionType);

            // Add to UI
            mainContainer.Add(boxContainer);
        }

        /// <summary>
        /// Handles changes to the condition type and updates the UI accordingly
        /// </summary>
        /// <param name="conditionData">The condition data being modified</param>
        /// <param name="boxContainer">The UI container for the condition</param>
        /// <param name="newValue">The new condition type value</param>
        protected void OnConditionTypeChanged(ConditionData conditionData, Box boxContainer, ConditionType newValue)
        {
            conditionData.ConditionType = newValue;
            boxContainer.Clear();

            // Left side: Condition text input field
            TextField conditionTextField = UIHelper.CreateTextField(conditionData.ConditionText, newValue =>
            {
                conditionData.ConditionText = newValue;
            }, "TextField");

            // Middle: Dropdown for selecting condition type
            EnumField conditionTypeEnumField = UIHelper.CreateEnumField(conditionData.ConditionType, updatedValue =>
            {
                OnConditionTypeChanged(conditionData, boxContainer, (ConditionType)updatedValue);
            }, "Enum");

            // Right side: Remove button
            Button removeButton = UIHelper.CreateButton("Remove", () =>
            {
                OnRemoveButtonClick(boxContainer, conditionData);
            }, "RemoveButton");

            // Add UI components to the condition box
            boxContainer.Add(conditionTextField);
            boxContainer.Add(conditionTypeEnumField);

            // Add comparison value field if applicable
            AddComparisonValueField(conditionData, boxContainer, newValue);

            boxContainer.Add(removeButton);
        }

        /// <summary>
        /// Handles the removal of a condition from the node
        /// </summary>
        /// <param name="boxContainer">The UI container associated with the condition</param>
        /// <param name="conditionData">The condition data to remove</param>
        protected void OnRemoveButtonClick(Box boxContainer, ConditionData conditionData)
        {
            if (!(_currentNodeData is IConditionHolder conditionHolder) ||
                conditionHolder.ConditionDatas == null ||
                conditionHolder.ConditionDatas.Count == 0) return;

            // Find the condition to remove
            ConditionData itemToRemove = conditionHolder.ConditionDatas
                .Find(c => c.ConditionText == conditionData.ConditionText &&
                           c.ConditionType == conditionData.ConditionType &&
                           c.ComparisonValue == conditionData.ComparisonValue);

            if (itemToRemove != null)
            {
                // Remove the condition from the list
                conditionHolder.ConditionDatas.Remove(itemToRemove);
            }
            else
            {
                Debug.LogError($"ConditionData does not match, unable to remove!");
            }

            // Remove from UI
            mainContainer.Remove(boxContainer);
            ShowHideChoiceEnum();
        }

        protected virtual void ShowHideChoiceEnum() { }
    }
}
