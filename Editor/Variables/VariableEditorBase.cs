namespace GenericScriptableArchitecture.Editor
{
    using System.Reflection;
    using GenericUnityObjects.UnityEditorInternals;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.UnityEngineInternals;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    internal abstract class VariableEditorBase : GenericHeaderEditor
    {
        protected bool WithHistory;

        private static readonly GUIContent _currentValueLabel = new GUIContent("Current Value");

        private VariableBase _variableBase;

        private SerializedProperty _initialValue;
        private SerializedProperty _value;
        private SerializedProperty _previousValue;

        private FieldInfo _valueField;
        private FieldInfo _previousValueField;

        private bool _initialValueEnabled;

        protected virtual void OnEnable()
        {
            _variableBase = target as VariableBase;
            WithHistory = target.GetType().BaseType?.GetGenericTypeDefinition() == typeof(VariableWithHistory<>);

            _initialValue = serializedObject.FindProperty(nameof(Variable<int>._initialValue));
            GetValueField();
            GetPreviousValueField();
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
            EditorGUI.BeginChangeCheck();

            using (new EditorGUI.DisabledScope(ApplicationUtil.InEditMode))
                EditorDrawHelper.DelayedPropertyField(_value, _currentValueLabel);

            if ( ! EditorGUI.EndChangeCheck())
                return;

            ChangePreviousValue();

            // Invoke events. Both current and previous value are updated at this stage.
            _variableBase.InvokeValueChangedEvents();
        }

        private void ChangePreviousValue()
        {
            if ( ! WithHistory)
                return;

            // Get previous value before applying the change
            object previousValue = _valueField.GetValue(target).DeepCopy();

            // Apply the changed value so that it is not lost.
            serializedObject.ApplyModifiedProperties();

            // Set the previousValue
            _previousValueField.SetValue(target, previousValue);

            // Load the previous value to serialized object so that it is updated in the inspector
            serializedObject.Update();
        }

        protected void DrawInitialValue()
        {
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