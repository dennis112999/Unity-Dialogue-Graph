using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Dennis.UI;
using UnityEngine.Events;

using Dennis.Tools.Dialogue;
using Dennis.Tools.DialogueGraph.Event;
using System;

using DG.Tweening;
using Dennis.Tools.DialogueGraph.Data;

namespace Dennis.Tools.DialogueGraph.UI
{
    public class DialogueControllerUI : MonoBehaviour
    {
        [Header("Controller")]
        [SerializeField] private GameObject _dialogueControllerUI;

        [Header("Text")]
        [SerializeField] RectTransform _dialoguePanel;
        [SerializeField] private float animationDuration = 0.5f;
        private float _hiddenPosY;
        private float _visiblePosY;

        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _contentText;
        [SerializeField] private TextAnimation _contentTextAnimation;

        [Header("Image")]
        [SerializeField] private Image _leftImage;
        [SerializeField] private Image _rightImage;

        [Header("Buttons")]
        [SerializeField] private GameObject _choicePanel;
        [SerializeField] private Transform _choiceManagerTransform;
        [SerializeField] private GameObject _choiceButtonPrefab;

        [Header("Continue")]
        [SerializeField] private Button _continueButton;

        public event Action OnAnimationComplete;

        private void Awake()
        {
            InitUI();
        }

        private void InitUI()
        {
            _visiblePosY = _dialoguePanel.anchoredPosition.y;
            _hiddenPosY = _visiblePosY - 650f;

            _dialoguePanel.anchoredPosition = new Vector2(_dialoguePanel.anchoredPosition.x, _hiddenPosY);
        }

        public void ShowDialogueUI(bool show)
        {
            if (show)
            {
                _dialogueControllerUI.SetActive(true);
            }
            else
            {
                SetImage(null, null);
            }

            float targetY = show ? _visiblePosY : _hiddenPosY;
            _dialoguePanel.DOAnchorPosY(targetY, animationDuration)
                .SetEase(show ? Ease.OutCubic : Ease.InCubic)
                .OnComplete(() =>
                {
                    if (!show)
                    {
                        _dialogueControllerUI.SetActive(false);

                        // OnDialogueCompleted Event
                        Events.OnDialogueCompleted.Publish();
                    }
            });
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

        public void SetImage(Sprite leftSprite, Sprite rightSprite, SpeakerType leftType = SpeakerType.None, SpeakerType rightType = SpeakerType.None)
        {
            // Handle left image
            _leftImage.gameObject.SetActive(leftSprite != null);
            if (leftSprite != null)
            {
                _leftImage.sprite = leftSprite;
                _leftImage.rectTransform.localEulerAngles = new Vector3(0, 180, 0);
                _leftImage.color = GetImageColorBySpeakerType(leftType);
            }

            // Handle right image
            _rightImage.gameObject.SetActive(rightSprite != null);
            if (rightSprite != null)
            {
                _rightImage.sprite = rightSprite;
                _rightImage.color = GetImageColorBySpeakerType(rightType);
            }
        }

        /// <summary>
        /// Returns color based on SpeakerType (white if speaking, dark gray if not)
        /// </summary>
        private Color GetImageColorBySpeakerType(SpeakerType type)
        {
            return type == SpeakerType.Speaking
                ? Color.white
                : new Color(0.5f, 0.5f, 0.5f, 1f);
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
            _contentTextAnimation.StartAnimation(OnAnimationCompleteEvent);

            _continueButton.gameObject.SetActive(false);
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

            _choicePanel.SetActive(true);
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
            _continueButton.onClick.AddListener(() => _continueButton.gameObject.SetActive(false));
            _continueButton.gameObject.SetActive(true);
        }

        private void OnAnimationCompleteEvent()
        {
            OnAnimationComplete?.Invoke();
        }
    }

}