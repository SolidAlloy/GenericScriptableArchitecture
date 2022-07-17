namespace GenericScriptableArchitecture.Editor
{
    using System;
    using GenericUnityObjects.Editor;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(BaseVariableInstancer), true)]
    public class VariableInstancerEditor : Editor
    {
        private bool _initialized;
        private GenericVariableInstancerEditor _editor;
        private InspectorGUIHelper _inspectorGUIHelper;
        private PlayModeUpdateHelper _updateHelper;

        private void OnEnable()
        {
            try { _inspectorGUIHelper = new InspectorGUIHelper(serializedObject); }
            catch { return; }

            // The targets length is 0 or the first target is null for a couple frames after the domains reload.
            // We need to avoid exceptions while the target is not set by Unity.
            if (targets.Length == 0 || target == null)
                return;

            _editor = target is IVariableWithHistory variableWithHistory
                ? new VariableInstancerWithHistoryEditor(_inspectorGUIHelper, variableWithHistory, serializedObject, Repaint, () => _initialized)
                : new GenericVariableInstancerEditor(_inspectorGUIHelper, (BaseVariableInstancer) target, serializedObject, Repaint, () => _initialized);

            _initialized = true;
            _updateHelper = new PlayModeUpdateHelper(this, () =>_editor.Update());
        }

        private void OnDisable()
        {
            _updateHelper?.Dispose();
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

            if (_inspectorGUIHelper == null)
            {
                base.OnInspectorGUI();
                return;
            }

            _editor.OnInspectorGUI();
        }
    }

    internal class GenericVariableInstancerEditor : IInlineDrawer
    {
        protected readonly BaseVariableInstancer _target;
        protected readonly VariableHelperDrawer _variableHelperDrawer;
        private readonly SerializedProperty _variableReferenceProperty;
        protected readonly InspectorGUIHelper _inspectorGUIHelper;

        private SerializedObject _referenceSerializedObject;
        private SerializedProperty _referenceInitialValueProperty;
        private SerializedProperty _valueProperty;

        public GenericVariableInstancerEditor(InspectorGUIHelper inspectorGUIHelper, BaseVariableInstancer target, SerializedObject serializedObject, Action repaint, Func<bool> canDrawListeners)
        {
            _variableReferenceProperty = serializedObject.FindProperty(nameof(VariableInstancer<int>._variableReference));
            _target = target;
            var variableHelper = ((IVariable) target).VariableHelper;
            var variableHelperProperty = serializedObject.FindProperty(nameof(VariableInstancer<int>._variableHelper));
            _variableHelperDrawer = new VariableHelperDrawer(variableHelper, variableHelperProperty, repaint, canDrawListeners);
            _inspectorGUIHelper = inspectorGUIHelper;
        }

        public virtual void Update() => _variableHelperDrawer.Update();

        public void OnInspectorGUI()
        {
            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            DrawGUI();
        }

        protected virtual void DrawGUI()
        {
            DrawBaseReference();

            if (_target.BaseVariableReference == null)
                return;

            _variableHelperDrawer.DrawCurrentValue();
            _variableHelperDrawer.DrawStackTrace();
            _variableHelperDrawer.DrawListeners();
        }

        public bool HasContent => true;

        public void OnInlineGUI()
        {
            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            DrawInlineGUI();

        }

        protected virtual void DrawInlineGUI()
        {
            if (ApplicationUtil.InEditMode)
            {
                DrawBaseReference();
            }
            else
            {
                _variableHelperDrawer.DrawCurrentValue();
            }
        }

        protected void DrawBaseReference()
        {
            var fieldRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            (Rect labelRect, Rect valueRect) = fieldRect.CutVertically(EditorGUIUtility.labelWidth);

            DrawLabel(labelRect, _variableReferenceProperty, GUIContentHelper.Temp("Base"));
            DrawObjectReference(valueRect, _variableReferenceProperty, 0);
        }

        private static void DrawLabel(Rect rect, SerializedProperty property, GUIContent label)
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
                _referenceSerializedObject = new SerializedObject(variableProperty.objectReferenceValue);
                _referenceInitialValueProperty = _referenceSerializedObject.FindProperty(nameof(Variable<int>._initialValue));

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
                    _referenceSerializedObject.UpdateIfRequiredOrScript();

                    if (_referenceInitialValueProperty == null)
                        throw new NullReferenceException("The variable value is a non-serialized type. Make it serializable to use in a variable.");

                    EditorGUILayout.PropertyField(_referenceInitialValueProperty);
                    _referenceSerializedObject.ApplyModifiedProperties();
                }
            }
        }
    }

    internal class VariableInstancerWithHistoryEditor : GenericVariableInstancerEditor
    {
        private readonly VariableWithHistoryHelperDrawer _drawerWithHistory;

        public VariableInstancerWithHistoryEditor(InspectorGUIHelper inspectorGUIHelper, IVariableWithHistory target, SerializedObject serializedObject, Action repaint, Func<bool> canDrawListeners)
            : base(inspectorGUIHelper, (BaseVariableInstancer) target, serializedObject, repaint, canDrawListeners)
        {
            var variableHelper = target.VariableHelperWithHistory;
            var variableHelperProperty = serializedObject.FindProperty(nameof(VariableWithHistory<int>._variableHelperWithHistory));
            _drawerWithHistory = new VariableWithHistoryHelperDrawer(variableHelper, variableHelperProperty, canDrawListeners);
        }

        public override void Update()
        {
            base.Update();
            _drawerWithHistory.Update();
        }

        protected override void DrawGUI()
        {
            DrawBaseReference();

            if (_target.BaseVariableReference == null)
                return;

            _drawerWithHistory.DrawCurrentValue(_variableHelperDrawer);

            if (EditorApplication.isPlaying)
                _drawerWithHistory.DrawPreviousValue();

            _variableHelperDrawer.DrawStackTrace();
            _variableHelperDrawer.DrawListeners();
            _drawerWithHistory.DrawListenersWithHistory();
        }

        protected override void DrawInlineGUI()
        {
            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            if (ApplicationUtil.InEditMode)
            {
                DrawBaseReference();
            }
            else
            {
                if (_target.BaseVariableReference == null)
                    return;

                _drawerWithHistory.DrawCurrentValue(_variableHelperDrawer);
                _drawerWithHistory.DrawPreviousValue();
            }
        }
    }
}