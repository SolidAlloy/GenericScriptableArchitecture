namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(BaseValue), true)]
    internal class ConstantEditor : Editor, IInlineDrawer
    {
        private SerializedProperty _description;
        private SerializedProperty _initialValue;
        private SerializedProperty _currentValue;
        private bool _initialValueEnabled;

        private void OnEnable()
        {
            try
            {
                _description = serializedObject.FindProperty(nameof(BaseValue._description));
            }
            catch // SerializedObjectNotCreatableException can be thrown but it is internal, so we can't catch it directly.
            {
                return;
            }

            _initialValue = serializedObject.FindProperty(nameof(Constant<int>._initialValue));
            _currentValue = serializedObject.FindProperty(nameof(Constant<int>._value));
        }

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(serializedObject);

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
            using var guiWrapper = new InspectorGUIWrapper(serializedObject);

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