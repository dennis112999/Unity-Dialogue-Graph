using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Dennis.Tools.DialogueGraph;

namespace Dennis.UI
{
    public class DialogueOption
    {
        public string Text { get; set; }
        public bool ConditionCheck { get; set; }
        public ChoiceNodeFailAction FailAction { get; set; }
    }

    public class ChoiceButton : MonoBehaviour
    {
        [Header("UI element")]
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Button _choiceButton;

        /// <summary>
        /// Sets the text and click event for the choice button
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="choiceNodeFailAction">Action to set button state</param>
        /// <param name="unityActionCallback">Callback for button click event</param>
        public void SetChoiceButton(DialogueOption dialogueOption, UnityAction unityActionCallback = null)
        {
            _buttonText.text = dialogueOption.Text;

            if(!dialogueOption.ConditionCheck) SetButtonState(dialogueOption.FailAction);

            _choiceButton.onClick.RemoveAllListeners();

            if (unityActionCallback != null)
            {
                _choiceButton.onClick.AddListener(unityActionCallback);
            }
        }

        private void SetButtonState(ChoiceNodeFailAction choiceNodeFailAction)
        {
            switch (choiceNodeFailAction)
            {
                case ChoiceNodeFailAction.Hide:
                    gameObject.SetActive(false);
                    break;

                case ChoiceNodeFailAction.Disable:
                    _choiceButton.interactable = false;
                    break;
            }
        }
    }
}
