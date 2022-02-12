namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Reflection;
    using SolidUtilities.UnityEngineInternals;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    internal abstract class VariableEditorBase : PlayModeUpdatableEditor
    {
        protected bool _withHistory;
        protected BaseVariable _baseVariable;

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

        protected override void OnEnable()
        {
            base.OnEnable();

            // Can be null because we also use the editor for drawing Constant<T>
            _baseVariable = target as BaseVariable;
            _withHistory = IsTargetWithHistory();

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

            if (baseType is { IsGenericType: true })
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
            if ( ! _withHistory)
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
            bool previousExpandValue = _value.isExpanded;

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(_value, _currentValueLabel);

                // Must be called after PropertyField and before changeCheck.changed
                changeAppliedToProperty = IsChangeAppliedToProperty();

                // Do not count toggling foldout as a change.
                if (changeCheck.changed && previousExpandValue == _value.isExpanded)
                {
                    if ( ! _changeCheckPassedPreviously && _withHistory)
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
            if (_withHistory)
            {
                ChangePreviousValue();
            }
            else
            {
                ApplyCurrentValue();
            }

            // ReSharper disable once Unity.NoNullPropagation
            _baseVariable?.InvokeValueChangedEvents();
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
            // Apply the changed value so that it is not lost.
            serializedObject.ApplyModifiedProperties();

            // Set the previousValue
            _previousValueField.SetValue(target, _previousValueObject);

            // Set "HasPreviousValue" to true because it might still be false and is not set automatically.
            _baseVariable.HasPreviousValueInternal = true;

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
            if ( ! _withHistory)
                return;

            if (_baseVariable.HasPreviousValueInternal)
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(_previousValue);
            }
            else
            {
                EditorGUILayout.LabelField("Previous Value", "Not set yet");
            }
        }
    }
}