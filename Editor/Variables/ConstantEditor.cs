namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.Editor;
    using UnityEditor;

    [CustomEditor(typeof(BaseValue), true)]
    internal class ConstantEditor : Editor, IInlineDrawer
    {
        private SerializedProperty _description;
        private SerializedProperty _initialValue;
        private SerializedProperty _currentValue;
        private bool _initialValueEnabled;
        private InspectorGUIHelper _inspectorGUIHelper;

        private void OnEnable()
        {
            try { _inspectorGUIHelper = new InspectorGUIHelper(serializedObject); }
            catch { return; }

            if (target == null)
                return;

            _description = serializedObject.FindProperty(nameof(BaseValue._description));
            _initialValue = serializedObject.FindProperty(nameof(Constant<int>._initialValue));
            _currentValue = serializedObject.FindProperty(nameof(Constant<int>._value));
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

            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.PropertyField(_description);

            _initialValueEnabled = GenericVariableEditor.DrawInitialValue(_initialValue, _initialValueEnabled);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                EditorGUILayout.PropertyField(_currentValue, VariableHelperDrawer.CurrentValueLabel);
        }

        public bool HasContent => true;

        public void OnInlineGUI()
        {
            if (_inspectorGUIHelper == null)
            {
                base.OnInspectorGUI();
                return;
            }

            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            if (ApplicationUtil.InEditMode)
            {
                _initialValueEnabled = GenericVariableEditor.DrawInitialValue(_initialValue, _initialValueEnabled);
            }
            else
            {
                EditorGUILayout.PropertyField(_currentValue, VariableHelperDrawer.CurrentValueLabel);
            }
        }
    }
}