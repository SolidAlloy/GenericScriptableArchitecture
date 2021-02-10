namespace ExtendedScriptableObjects.Editor
{
    using System.Reflection;
    using GenericUnityObjects.UnityEditorInternals;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.UnityEngineInternals;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    [CustomEditor(typeof(VariableBase), true)]
    internal class VariableEditor : GenericHeaderEditor
    {
        private static readonly GUIContent _currentValueLabel = new GUIContent("Current Value");

        private SerializedProperty _initialValue;
        private SerializedProperty _value;
        private SerializedProperty _previousValue;

        private SerializedProperty _description;
        private SerializedProperty _changed;
        private SerializedProperty _changedWithHistory;

        private bool _initialValueEnabled;

        private FieldInfo _valueField;
        private FieldInfo _previousValueField;

        private void OnEnable()
        {
            const string valueFieldName = "_value";
            const string previousValueFieldName = "_previousValue";

            _initialValue = serializedObject.FindProperty("_initialValue");
            _value = serializedObject.FindProperty(valueFieldName);
            _previousValue = serializedObject.FindProperty(previousValueFieldName);

            _description = serializedObject.FindProperty("_description");
            _changed = serializedObject.FindProperty("_changed");
            _changedWithHistory = serializedObject.FindProperty("_changedWithHistory");

            const BindingFlags privateInstanceField = BindingFlags.Instance | BindingFlags.NonPublic;
            _valueField = target.GetType().BaseType?.GetField(valueFieldName, privateInstanceField);
            _previousValueField = target.GetType().BaseType?.GetField(previousValueFieldName, privateInstanceField);

            Assert.IsNotNull(_valueField);
            Assert.IsNotNull(_previousValueField);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            DrawFields();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawFields()
        {
            EditorGUILayout.PropertyField(_description);

            DrawInitialValue();
            DrawCurrentValue();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(_previousValue);

            EditorGUILayout.PropertyField(_changed);
            EditorGUILayout.PropertyField(_changedWithHistory);
        }

        private void DrawInitialValue()
        {
            if ( ! EditorApplication.isPlaying)
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

        private void DrawCurrentValue()
        {
            EditorGUI.BeginChangeCheck();

            using (new EditorGUI.DisabledScope(!Application.isPlaying))
                EditorDrawHelper.DelayedPropertyField(_value, _currentValueLabel);

            if ( ! EditorGUI.EndChangeCheck())
                return;

            object previousValue = _valueField.GetValue(target).DeepCopy();
            serializedObject.ApplyModifiedProperties();
            _previousValueField.SetValue(target, previousValue);
        }
    }
}