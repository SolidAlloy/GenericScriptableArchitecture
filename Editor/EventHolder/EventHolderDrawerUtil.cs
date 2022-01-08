namespace GenericScriptableArchitecture.Editor
{
    using System;
    using JetBrains.Annotations;
    using SolidUtilities.Editor;
    using SolidUtilities;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using UnityEngine.Assertions;

    public readonly struct EventHolderDrawerUtil
    {
        private readonly SerializedProperty _mainProperty;
        private readonly SerializedProperty _event;
        private readonly SerializedProperty _variable;
        private readonly SerializedProperty _type;
        private readonly SerializedProperty _notifyCurrentValue;
        private readonly int _argsCount;
        private readonly bool _drawObjectField;

        public EventHolderDrawerUtil(SerializedProperty property, int argsCount)
        {
            _mainProperty = property;
            _argsCount = argsCount;
            _event = _mainProperty.FindPropertyRelative(nameof(_event));
            _variable = _mainProperty.FindPropertyRelative(nameof(_variable));
            _type = _mainProperty.FindPropertyRelative(nameof(_type));
            _notifyCurrentValue = _mainProperty.FindPropertyRelative(nameof(_notifyCurrentValue));
            _drawObjectField = _mainProperty.FindPropertyRelative(nameof(EventHolder<int>.DrawObjectField)).boolValue;
        }

        private EventTypes EventType
        {
            get => (EventTypes) _type.enumValueIndex;
            set => _type.enumValueIndex = (int) value;
        }

        private SerializedProperty ExposedProperty
        {
            get
            {
                return EventType switch
                {
                    EventTypes.ScriptableEvent => _event,
                    EventTypes.Variable => _variable,
                    _ => throw new ArgumentOutOfRangeException(nameof(EventType),
                        "Unknown enum value passed to event type")
                };
            }
        }

        public void DrawButtonAndValue(Rect fieldRect, GUIContent label)
        {
            using var _ = new EditorDrawHelper.PropertyWrapper(fieldRect, label, _mainProperty);

            fieldRect.height = EditorGUIUtility.singleLineHeight;

            int previousIndent = EditorGUI.indentLevel;

            if (_drawObjectField)
            {
                (Rect labelRect, Rect buttonRect, Rect valueRect) = GetLabelButtonValueRects(fieldRect);

                EditorGUI.HandlePrefixLabel(fieldRect, labelRect, label);

                // The indent level must be made 0 for the button and value to be displayed normally, without any
                // additional indent. Otherwise, the button will not be clickable, and the value will look shifted
                // compared to other fields.
                EditorGUI.indentLevel = 0;

                EventType = ChoiceButton.DrawAndCheckType(buttonRect, EventType, _argsCount);
                EditorGUI.PropertyField(valueRect, ExposedProperty, GUIContent.none);
            }

            if (EventType == EventTypes.Variable)
            {
                if (_drawObjectField)
                    fieldRect.y += EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(fieldRect, _notifyCurrentValue, GUIContentHelper.Temp("Fire current value on enable"));
            }

            EditorGUI.indentLevel = previousIndent;
        }

        public void DrawEvent(Rect fieldRect, GUIContent label)
        {
            if (!_drawObjectField)
                return;

            EventType = EventTypes.ScriptableEvent;
            EditorGUI.PropertyField(fieldRect, _event, label);
        }

        public float GetPropertyHeight()
        {
            int initialLinesCount = _drawObjectField ? 1 : 0;
            return EditorGUIUtility.singleLineHeight * (initialLinesCount + (EventType == EventTypes.Variable ? 1 : 0));
        }

        private (Rect label, Rect button, Rect value) GetLabelButtonValueRects(Rect totalRect)
        {
            const float indentWidth = 15f;
            const float valueLeftIndent = 2f;

            totalRect.height = EditorGUIUtility.singleLineHeight;

            (Rect labelAndButtonRect, Rect valueRect) = totalRect.CutVertically(EditorGUIUtility.labelWidth);

            labelAndButtonRect.xMin += EditorGUI.indentLevel * indentWidth;

            float buttonWidth = ChoiceButton.Width;

            (Rect labelRect, Rect buttonRect) = labelAndButtonRect.CutVertically(buttonWidth, fromRightSide: true);

            valueRect.xMin += valueLeftIndent;
            return (labelRect, buttonRect, valueRect);
        }

        private static class ChoiceButton
        {
            private static readonly GUIStyle _buttonStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };

            private static readonly string[] _popupOptionsOne = { "Scriptable Event", "Variable" };
            private static readonly string[] _popupOptionsTwo = { "Scriptable Event", "Variable With History" };

            public static float Width => _buttonStyle.fixedWidth;

            [Pure]
            public static EventTypes DrawAndCheckType(Rect buttonRect, EventTypes currentType, int argsCount)
            {
                Assert.IsTrue(argsCount == 1 || argsCount == 2, "argsCount equals 1 or 2");
                int result = EditorGUI.Popup(buttonRect, (int) currentType, argsCount == 1 ? _popupOptionsOne : _popupOptionsTwo, _buttonStyle);
                return (EventTypes) result;
            }
        }
    }
}