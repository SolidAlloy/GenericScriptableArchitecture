namespace ScriptableEvents.Editor
{
    using EasyButtons.Editor;
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;

    [CustomEditor(typeof(ScriptableEventBase), true)]
    public class ScriptableEventEditor : GenericHeaderEditor
    {
        private ButtonsDrawer _buttonsDrawer;
        private GenericUnityObjectHelper _helper;

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
            _helper = new GenericUnityObjectHelper(target);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                DrawMissingScript();
                return;
            }

            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty iterator = serializedObject.GetIterator();
            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (iterator.propertyPath == "m_Script")
                {
                    _helper.DrawMonoScript(iterator);
                }
                else
                {
                    EditorGUILayout.PropertyField(iterator, true, null);
                }
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();

            _buttonsDrawer.DrawButtons(targets);
        }

        private void DrawMissingScript()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }
        }
    }
}