namespace GenericScriptableArchitecture.Editor
{
    using System;
    using JetBrains.Annotations;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using UnityEditor;
    using UnityEngine;

    public readonly struct EventHolderDrawerUtil
    {
        private readonly SerializedProperty _mainProperty;
        private readonly SerializedProperty _event;
        private readonly SerializedProperty _variable;
        private readonly SerializedProperty _type;

        public EventHolderDrawerUtil(SerializedProperty property)
        {
            _mainProperty = property;
            _event = _mainProperty.FindPropertyRelative(nameof(_event));
            _variable = _mainProperty.FindPropertyRelative(nameof(_variable));
            _type = _mainProperty.FindPropertyRelative(nameof(_type));
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
            using (new EditorDrawHelper.PropertyWrapper(fieldRect, label, _mainProperty))
            {
                (Rect labelRect, Rect buttonRect, Rect valueRect) = GetLabelButtonValueRects(fieldRect);

                EditorGUI.HandlePrefixLabel(fieldRect, labelRect, label);

                // The indent level must be made 0 for the button and value to be displayed normally, without any
                // additional indent. Otherwise, the button will not be clickable, and the value will look shifted
                // compared to other fields.
                int previousIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                EventType = ChoiceButton.DrawAndCheckType(buttonRect, EventType);
                EditorGUI.PropertyField(valueRect, ExposedProperty, GUIContent.none);

                EditorGUI.indentLevel = previousIndent;
            }
        }

        public void DrawEvent(Rect fieldRect, GUIContent label)
        {
            EventType = EventTypes.ScriptableEvent;
            EditorGUI.PropertyField(fieldRect, _event, label);
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

            private static readonly string[] _popupOptions = { "Scriptable Event", "Variable" };

            public static float Width => _buttonStyle.fixedWidth;

            [Pure]
            public static EventTypes DrawAndCheckType(Rect buttonRect, EventTypes currentType)
            {
                int result = EditorGUI.Popup(buttonRect, (int) currentType, _popupOptions, _buttonStyle);
                return (EventTypes) result;
            }
        }
    }
}