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
        private static readonly GUIContent _currentValueLabel = new GUIContent("Current Value");

        private SerializedProperty _initialValue;
        private SerializedProperty _value;
        private SerializedProperty _previousValue;

        private FieldInfo _valueField;
        private FieldInfo _previousValueField;

        private bool _initialValueEnabled;

        protected static bool InPlayMode => EditorApplication.isPlayingOrWillChangePlaymode;

        protected static bool InEditMode => ! InPlayMode;

        protected virtual void OnEnable()
        {
            const string valueFieldName = "_value";
            const string previousValueFieldName = "_previousValue";

            _initialValue = serializedObject.FindProperty("_initialValue");
            _value = serializedObject.FindProperty(valueFieldName);
            _previousValue = serializedObject.FindProperty(previousValueFieldName);

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

        protected abstract void DrawFields();

        protected void DrawCurrentValue()
        {
            EditorGUI.BeginChangeCheck();

            using (new EditorGUI.DisabledScope(InEditMode))
                EditorDrawHelper.DelayedPropertyField(_value, _currentValueLabel);

            if ( ! EditorGUI.EndChangeCheck())
                return;

            object previousValue = _valueField.GetValue(target).DeepCopy();
            serializedObject.ApplyModifiedProperties();
            _previousValueField.SetValue(target, previousValue);
        }

        protected void DrawInitialValue()
        {
            if (InEditMode)
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
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(_previousValue);
        }
    }
}