using System;
using UnityEngine.UIElements;

namespace Dennis.Tools
{
    public static class UIHelper
    {
        /// <summary>
        /// Creates Box UI element
        /// </summary>
        /// <param name="className">class USS name to add to the Box</param>
        /// <returns>Box UI</returns>
        public static Box CreateBox(string className = null)
        {
            Box box = new Box();
            AddClassIfNotEmpty(box, className);
            return box;
        }

        /// <summary>
        /// Creates Label UI element
        /// </summary>
        /// <param name="labelName">Label Name</param>
        /// <param name="className">class USS name to add to the Label</param>
        /// <returns>Label UI element</returns>
        public static Label CreateLabel(string labelName, string className = null)
        {
            Label label = new Label(labelName);
            AddClassIfNotEmpty(label, className);
            return label;
        }

        /// <summary>
        /// Create Button
        /// </summary>
        /// <param name="buttonText">text displayed</param>
        /// <param name="onClickAction">On Button click action</param>
        /// <param name="className">class USS name to add</param>
        /// <returns>Button</returns>
        public static Button CreateButton(string buttonText, Action onClickAction, string className = null)
        {
            Button button = new Button(onClickAction) { text = buttonText };
            AddClassIfNotEmpty(button, className);
            return button;
        }

        /// <summary>
        /// Create TextField
        /// </summary>
        /// <param name="initialValue">initial text value of the field</param>
        /// <param name="onValueChanged">action - the text value changes</param>
        /// <param name="className">class USS name to add</param>
        /// <returns>TextField with properties</returns>
        public static TextField CreateTextField(string initialValue, Action<string> onValueChanged, string className = null)
        {
            TextField textField = new TextField(string.Empty);
            textField.SetValueWithoutNotify(initialValue);
            textField.RegisterValueChangedCallback(evt => onValueChanged(evt.newValue));
            AddClassIfNotEmpty(textField, className);
            return textField;
        }

        /// <summary>
        /// Create Image
        /// </summary>
        /// <param name="className">class USS name to add</param>
        /// <returns>Image</returns>
        public static Image CreateImage(string className = null)
        {
            Image image = new Image();
            AddClassIfNotEmpty(image, className);
            return image;
        }

        /// <summary>
        /// Adds a USS class to a VisualElement
        /// </summary>
        /// <param name="element">The UI element to add the class to</param>
        /// <param name="className">The class name to add</param>
        private static void AddClassIfNotEmpty(VisualElement element, string className)
        {
            if (string.IsNullOrEmpty(className)) return;
            element.AddToClassList(className);
        }
    }
}
