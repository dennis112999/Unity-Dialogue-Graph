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
        private GameObject _dialogueControllerUI;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _contentText;

        [Header("Image")]
        [SerializeField] private Image _leftImage;
        [SerializeField] private Image _rightImage;

        [Header("Buttons")]
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
            _nameText.text = name;
        }

        public void SetImage(Sprite leftSprite, Sprite rightSprite)
        {
            if(leftSprite != null)  _leftImage.sprite = leftSprite;
            if(rightSprite != null) _rightImage.sprite = rightSprite;
        }

        public void SetContentText(string text)
        {
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

        public void SetupDialogueOptions(List<DialogueOption> dialogueOptions, UnityAction onCallBack)
        {
            DeleteChoiceManagerChildren();

            for (int i = 0; i < dialogueOptions.Count; i++)
            {
                var button = Instantiate(_choiceButtonPrefab, _choiceManagerTransform);

                if(button.TryGetComponent(out ChoiceButton choiceButton))
                {
                    choiceButton.SetChoiceButton(dialogueOptions[i], OnChoiceButtonCallback);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("Missing ChoiceButton component on the instantiated button.");
#endif
                }
            }
        }

        private void OnChoiceButtonCallback()
        {

        }

        #endregion Choice Buttons
    }

}