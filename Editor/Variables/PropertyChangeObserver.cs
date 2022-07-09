namespace GenericScriptableArchitecture.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class PropertyChangeObserver
    {
        private readonly SerializedProperty _serializedProperty;
        private bool _changeCheckPassedPreviously;
        private int _previousHotControl;
        private int _previousKeyboardControl;

        public PropertyChangeObserver(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty;
        }

        public bool DrawProperty(Action<SerializedProperty> drawProperty, Action onChange = null)
        {
            var currentEvent = Event.current;

            if (IsEnterPressed(currentEvent))
                ExitGUI(currentEvent);

            bool previousExpandValue = _serializedProperty.isExpanded;

            using var changeCheck = new EditorGUI.ChangeCheckScope();

            drawProperty(_serializedProperty);

            // Must be called after drawProperty and before changeCheck.changed
            bool changeAppliedToProperty = IsChangeAppliedToProperty();

            // Do not count toggling foldout as a change.
            if (changeCheck.changed && previousExpandValue == _serializedProperty.isExpanded)
            {
                if ( ! _changeCheckPassedPreviously)
                {
                    onChange?.Invoke();
                }

                _changeCheckPassedPreviously = true;
            }

            return changeAppliedToProperty;
        }

        private bool IsChangeAppliedToProperty()
        {
            bool focusChanged = GUIUtility.hotControl != _previousHotControl || GUIUtility.keyboardControl != _previousKeyboardControl;
            _previousHotControl = GUIUtility.hotControl;
            _previousKeyboardControl = GUIUtility.keyboardControl;

            if (focusChanged && _changeCheckPassedPreviously)
            {
                _changeCheckPassedPreviously = false;
                return true;
            }

            return false;
        }

        private static bool IsEnterPressed(Event currentEvent)
        {
            return currentEvent.type == EventType.KeyDown && (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);
        }

        private static void ExitGUI(Event currentEvent)
        {
            GUIUtility.keyboardControl = 0;
            currentEvent.Use();
        }
    }
}