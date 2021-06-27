namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(BaseValue), true)]
    internal class ConstantEditor : VariableEditorBase
    {
        private SerializedProperty _description;

        protected override void OnEnable()
        {
            base.OnEnable();
            _description = serializedObject.FindProperty("_description");
        }

        protected override void DrawFields()
        {
            EditorGUILayout.PropertyField(_description);

            DrawInitialValue();
            DrawCurrentValue();
        }
    }
}