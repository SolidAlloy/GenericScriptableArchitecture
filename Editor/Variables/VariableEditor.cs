namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(VariableBase), true)]
    internal class VariableEditor : VariableEditorBase
    {
        private SerializedProperty _description;
        private SerializedProperty _changed;
        private SerializedProperty _changedWithHistory;

        protected override void OnEnable()
        {
            base.OnEnable();
            _description = serializedObject.FindProperty("_description");
            _changed = serializedObject.FindProperty("_changed");
            _changedWithHistory = serializedObject.FindProperty("_changedWithHistory");
        }

        protected override void DrawFields()
        {
            EditorGUILayout.PropertyField(_description);

            DrawInitialValue();
            DrawCurrentValue();
            DrawPreviousValue();

            EditorGUILayout.PropertyField(_changed);

            if (WithHistory)
                EditorGUILayout.PropertyField(_changedWithHistory);
        }
    }
}