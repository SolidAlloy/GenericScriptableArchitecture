namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Reflection;
    using GenericUnityObjects.UnityEditorInternals;
    using SolidUtilities.UnityEngineInternals;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    internal abstract class VariableEditorBase : GenericHeaderEditor
    {
        protected bool WithHistory;
        protected VariableBase VariableBase;

        private static readonly GUIContent _currentValueLabel = new GUIContent("Current Value");

        private SerializedProperty _initialValue;
        private SerializedProperty _value;
        private SerializedProperty _previousValue;

        private FieldInfo _valueField;
        private FieldInfo _previousValueField;

        private bool _initialValueEnabled;

        private bool _changeCheckPassedPreviously;
        private int _previousHotControl;
        private int _previousKeyboardControl;
        private object _previousValueObject;

        protected virtual void OnEnable()
        {
            // Can be null because we also use the editor for drawing Constant<T>
            VariableBase = target as VariableBase;

            WithHistory = IsTargetWithHistory();

            try
            {
                _initialValue = serializedObject.FindProperty(nameof(Variable<int>._initialValue));
            }
            catch // SerializedObjectNotCreatableException can be thrown but it is internal, so we can't catch it directly.
            {
                return;
            }

            GetValueField();
            GetPreviousValueField();
        }

        private bool IsTargetWithHistory()
        {
            var baseType = target.GetType().BaseType;

            if (baseType is {IsGenericType: true})
                return baseType.GetGenericTypeDefinition() == typeof(VariableWithHistory<>);

            return false;
        }

        private void GetValueField()
        {
            const string valueFieldName = nameof(Variable<int>._value);
            _value = serializedObject.FindProperty(valueFieldName);
            _valueField = target.GetType().BaseType?.GetField(valueFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(_valueField);
        }

        private void GetPreviousValueField()
        {
            if ( ! WithHistory)
                return;

            const string previousValueFieldName = nameof(VariableWithHistory<int>._previousValue);
            _previousValue = serializedObject.FindProperty(previousValueFieldName);
            _previousValueField = target.GetType().BaseType?.GetField(previousValueFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(_previousValueField);
        }

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            DrawFields();
        }

        protected abstract void DrawFields();

        protected void DrawCurrentValue()
        {
            ExitGUIOnEnterPress();

            bool changeAppliedToProperty;

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                using (new EditorGUI.DisabledScope(ApplicationUtil.InEditMode))
                {
                    EditorGUILayout.PropertyField(_value, _currentValueLabel);
                }

                // Must be called after PropertyField and before changeCheck.changed
                changeAppliedToProperty = IsChangeAppliedToProperty();

                if (changeCheck.changed)
                {
                    if ( ! _changeCheckPassedPreviously && WithHistory)
                    {
                        _previousValueObject = _valueField.GetValue(target).DeepCopy();
                    }

                    _changeCheckPassedPreviously = true;
                }
            }

            if (changeAppliedToProperty)
                ApplyChangeToVariable();
        }

        private void ApplyChangeToVariable()
        {
            if (WithHistory)
            {
                ChangePreviousValue();
            }
            else
            {
                ApplyCurrentValue();
            }

            // ReSharper disable once Unity.NoNullPropagation
            VariableBase?.InvokeValueChangedEvents();
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

        private void ExitGUIOnEnterPress()
        {
            var e = Event.current;

            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
            {
                GUIUtility.keyboardControl = 0;
                e.Use();
            }
        }

        private void ChangePreviousValue()
        {
            // Get previous value before applying the change
            // object previousValue = _valueField.GetValue(target).DeepCopy();

            // Apply the changed value so that it is not lost.
            serializedObject.ApplyModifiedProperties();

            // Set the previousValue
            _previousValueField.SetValue(target, _previousValueObject);

            // Load the previous value to serialized object so that it is updated in the inspector
            serializedObject.Update();
        }

        private void ApplyCurrentValue()
        {
            // Apply the change value, otherwise the event will be invoked with the old value.
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawInitialValue()
        {
            if (_initialValue == null)
                throw new NullReferenceException("The variable value is a non-serialized type. Make it serializable to use in a variable.");

            if (ApplicationUtil.InEditMode)
            {
                EditorGUILayout.PropertyField(_initialValue);
                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope( ! _initialValueEnabled))
                    EditorGUILayout.PropertyField(_initialValue);

                const float toggleWidth = 14f;

                _initialValueEnabled = EditorGUILayout.Toggle(_initialValueEnabled, GUILayout.MaxWidth(toggleWidth));
            }
        }

        protected void DrawPreviousValue()
        {
            if ( ! WithHistory)
                return;

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(_previousValue);
        }
    }
}