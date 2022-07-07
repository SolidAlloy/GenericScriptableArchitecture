namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.Editor;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(BaseVariableInstancer), true)]
    public class VariableInstancerEditor : Editor
    {
        private bool _initialized;
        private SerializedProperty _variableReferenceProperty;
        private BaseVariableInstancer _target;
        private Editor _referenceEditor;
        private SerializedProperty _referenceInitialValueProperty;
        private SerializedProperty _valueProperty;

        private void OnEnable()
        {
            // The targets length is 0 or the first target is null for a couple frames after the domains reload.
            // We need to avoid exceptions while the target is not set by Unity.
            if (targets.Length == 0 || target == null)
                return;

            _variableReferenceProperty = serializedObject.FindProperty(nameof(VariableInstancer<int>._variableReference));
            _valueProperty = serializedObject.FindProperty(nameof(VariableInstancer<int>._value));
            _target = (BaseVariableInstancer) target;
            _initialized = true;
        }

        protected override void OnHeaderGUI()
        {
            GenericHeaderUtility.OnHeaderGUI(this);
        }

        public override void OnInspectorGUI()
        {
            if (!_initialized)
            {
                OnEnable();
                return;
            }

            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            var fieldRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            (Rect labelRect, Rect valueRect) = fieldRect.CutVertically(EditorGUIUtility.labelWidth);

            DrawLabel(labelRect, _variableReferenceProperty, GUIContentHelper.Temp("Base"));
            DrawObjectReference(valueRect, _variableReferenceProperty, 0);

            if (ApplicationUtil.InPlayMode && _target.BaseVariableReference != null)
                EditorGUILayout.PropertyField(_valueProperty, GUIContentHelper.Temp("Current Value"));
        }

        private void DrawLabel(Rect rect, SerializedProperty property, GUIContent label)
        {
            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.LabelField(rect, label);
            }
        }

        private void DrawObjectReference(Rect valueRect, SerializedProperty variableProperty, int indentLevel)
        {
            EditorGUI.BeginChangeCheck();
            CreatableObjectDrawer.Instance.OnGUI(valueRect, variableProperty, GUIContent.none);

            // Find a property of the variable object if the variable object is assigned and if either the property was not found yet or the variable was changed.
            if (variableProperty.objectReferenceValue != null && (EditorGUI.EndChangeCheck() || _referenceInitialValueProperty == null))
            {
                _referenceEditor = CreateEditor(variableProperty.objectReferenceValue);
                _referenceInitialValueProperty = _referenceEditor.serializedObject.FindProperty(nameof(Variable<int>._initialValue));

                if (ApplicationUtil.InPlayMode && _target.BaseVariableReference != variableProperty.objectReferenceValue && _target.Initialized)
                {
                    // Reassigning the variable reference through property setter since there is logic that notifies listeners there.
                    _target.BaseVariableReference = (BaseVariable) variableProperty.objectReferenceValue;
                }
            }

            if ( ! variableProperty.isExpanded || variableProperty.objectReferenceValue == null)
                return;

            using (EditorGUIHelper.IndentLevelBlock(indentLevel + 1))
            {
                using (new EditorGUI.DisabledScope(!ApplicationUtil.InEditMode))
                {
                    _referenceEditor.serializedObject.UpdateIfRequiredOrScript();
                    EditorGUILayout.PropertyField(_referenceInitialValueProperty);
                    _referenceEditor.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}