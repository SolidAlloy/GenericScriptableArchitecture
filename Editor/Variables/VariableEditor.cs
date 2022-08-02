namespace GenericScriptableArchitecture.Editor
{
    using System;
    using GenericUnityObjects.Editor;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(BaseVariable), true)] // both variable and variable with history
    internal class VariableEditor : Editor
    {
        private GenericVariableEditor _variableEditor;
        private InspectorGUIHelper _inspectorGUIHelper;
        private PlayModeUpdateHelper _updateHelper;

        private void OnEnable()
        {
            try { _inspectorGUIHelper = new InspectorGUIHelper(serializedObject); }
            catch { return; }

            if (target == null)
                return;

            _variableEditor = target is IVariableWithHistory variableWithHistory
                ? new VariableWithHistoryEditor(_inspectorGUIHelper, serializedObject, variableWithHistory, Repaint)
                : new GenericVariableEditor(_inspectorGUIHelper, serializedObject, (IVariable) target, Repaint);

            _updateHelper = new PlayModeUpdateHelper(this, () => _variableEditor.Update());
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
            if (_inspectorGUIHelper == null)
            {
                base.OnInspectorGUI();
                return;
            }

            _variableEditor.OnInspectorGUI();
        }
    }

    internal class GenericVariableEditor : IInlineDrawer
    {
        private readonly SerializedProperty _initialValue;
        private bool _initialValueEnabled;
        protected readonly VariableHelperDrawer _variableHelperDrawer;
        protected readonly SerializedProperty _description;
        private readonly SerializedObject _serializedObject;
        private readonly InspectorGUIHelper _inspectorGUIHelper;

        public GenericVariableEditor(InspectorGUIHelper inspectorGUIHelper, SerializedObject serializedObject, IVariable target, Action repaint)
        {
            _inspectorGUIHelper = inspectorGUIHelper;
            _initialValue = serializedObject.FindProperty(nameof(Variable<int>._initialValue));
            var variableHelper = target.VariableHelper;
            var variableHelperProperty = serializedObject.FindProperty(nameof(Variable<int>._variableHelper));
            _variableHelperDrawer = new VariableHelperDrawer(variableHelper, variableHelperProperty, repaint);
            _description = serializedObject.FindProperty(nameof(BaseValue._description));
        }

        public virtual void Update()
        {
            _variableHelperDrawer.Update();
        }

        public void OnInspectorGUI()
        {
            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            DrawGUI();
        }

        protected virtual void DrawGUI()
        {
            EditorGUILayout.PropertyField(_description);
            DrawInitialValue();
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
                DrawInitialValue();
            }
            else
            {
                _variableHelperDrawer.DrawCurrentValue();
            }
        }

        protected void DrawInitialValue()
        {
            _initialValueEnabled = DrawInitialValue(_initialValue, _initialValueEnabled);
        }

        public static bool DrawInitialValue(SerializedProperty initialValueProperty, bool initialValueEnabled)
        {
            if (initialValueProperty == null)
                throw new NullReferenceException("The variable value is a non-serialized type. Make it serializable to use in a variable.");

            if (ApplicationUtil.InEditMode)
            {
                EditorGUILayout.PropertyField(initialValueProperty);
                return initialValueEnabled;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope( ! initialValueEnabled))
                    EditorGUILayout.PropertyField(initialValueProperty);

                const float toggleWidth = 14f;

                return EditorGUILayout.Toggle(initialValueEnabled, GUILayout.MaxWidth(toggleWidth));
            }
        }
    }

    internal class VariableWithHistoryEditor : GenericVariableEditor
    {
        private readonly VariableWithHistoryHelperDrawer _drawerWithHistory;

        public VariableWithHistoryEditor(InspectorGUIHelper inspectorGUIHelper, SerializedObject serializedObject, IVariableWithHistory target, Action repaint)
            : base(inspectorGUIHelper, serializedObject, target, repaint)
        {
            var variableHelper = target.VariableHelperWithHistory;
            var variableHelperProperty = serializedObject.FindProperty(nameof(VariableWithHistory<int>._variableHelperWithHistory));
            _drawerWithHistory = new VariableWithHistoryHelperDrawer(variableHelper, variableHelperProperty);
        }

        public override void Update()
        {
            base.Update();
            _drawerWithHistory.Update();
        }

        protected override void DrawGUI()
        {
            EditorGUILayout.PropertyField(_description);
            DrawInitialValue();
            _drawerWithHistory.DrawCurrentValue(_variableHelperDrawer);
            _drawerWithHistory.DrawPreviousValue();
            _variableHelperDrawer.DrawStackTrace();
            _variableHelperDrawer.DrawListeners();
            _drawerWithHistory.DrawListenersWithHistory();
        }

        protected override void DrawInlineGUI()
        {
            if (ApplicationUtil.InEditMode)
            {
                DrawInitialValue();
            }
            else
            {
                _drawerWithHistory.DrawCurrentValue(_variableHelperDrawer);
                _drawerWithHistory.DrawPreviousValue();
            }
        }
    }
}