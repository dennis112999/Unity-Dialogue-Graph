using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Dennis.UI;
using UnityEngine.Events;

namespace Dennis.Tools.DialogueGraph.UI
{
    public class DialogueControllerUI : MonoBehaviour
    {
        [Header("Controller")]
        [SerializeField] private GameObject _dialogueControllerUI;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _contentText;

        [Header("Image")]
        [SerializeField] private Image _leftImage;
        [SerializeField] private Image _rightImage;

        [Header("Buttons")]
        [SerializeField] private GameObject _choicePanel;
        [SerializeField] private Transform _choiceManagerTransform;
        [SerializeField] private GameObject _choiceButtonPrefab;

        [Header("Continue")]
        [SerializeField] private Button _continueButton;

        public void ShowDialogueUI(bool show)
        {
            _dialogueControllerUI.SetActive(show);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
#if UNITY_EDITOR
                Debug.LogWarning("SetName: The name string is null or empty.");
#endif
                name = "Forget to write the name string";
            }

            _nameText.text = name;
        }

        public void SetImage(Sprite leftSprite, Sprite rightSprite)
        {
            // Handle left image
            _leftImage.gameObject.SetActive(leftSprite != null);
            if (leftSprite != null)
            {
                _leftImage.sprite = leftSprite;
            }

            // Handle right image
            _rightImage.gameObject.SetActive(rightSprite != null);
            if (rightSprite != null)
            {
                _rightImage.sprite = rightSprite;
            }
        }

        public void SetContentText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
#if UNITY_EDITOR
                Debug.LogWarning("SetContentText: The content text string is null or empty.");
#endif
                text = "Forget to write the content text string";
            }

            _contentText.text = text;
        }

        #region Choice Buttons

        private void DeleteChoiceManagerChildren()
        {
            foreach (Transform child in _choiceManagerTransform)
            {
                Destroy(child.gameObject);
            }
        }

        public void SetupDialogueOptions(List<DialogueOption> dialogueOptions)
        {
            _choicePanel.SetActive(true);

            DeleteChoiceManagerChildren();

            for (int i = 0; i < dialogueOptions.Count; i++)
            {
                var button = Instantiate(_choiceButtonPrefab, _choiceManagerTransform);

                if (button.TryGetComponent(out ChoiceButton choiceButton))
                {
                    choiceButton.SetChoiceButton(dialogueOptions[i], OnChoiceButtonClick);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Missing ChoiceButton component on the instantiated button.");
#endif
                }
            }
        }

        public void OnChoiceButtonClick()
        {
            _choicePanel.SetActive(false);
        }

        #endregion Choice Buttons

        /// <summary>
        /// Assigns an action to the continue button and makes it visible
        /// </summary>
        /// <param name="action">Action to execute when clicked</param>
        public void ConfigureContinueButton(UnityAction action)
        {
            if (_continueButton == null || action == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[ConfigureContinueButton] Invalid parameters: either button or action is null.");
#endif
                return;
            }

            _continueButton.onClick.RemoveAllListeners();
            _continueButton.onClick.AddListener(action);
            _continueButton.gameObject.SetActive(true);
        }

    }

}