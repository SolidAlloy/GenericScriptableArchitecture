namespace GenericScriptableArchitecture.Editor
{
    using EasyButtons.Editor;
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;

    [CustomEditor(typeof(ScriptableEventBase), true)]
    public class ScriptableEventEditor : GenericHeaderEditor
    {
        private ButtonsDrawer _buttonsDrawer;
        private FoldoutList<ScriptableEventListenerBase> _listenersList;

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
            var listenersExpanded = serializedObject.FindProperty(nameof(ScriptableEventBase.ListenersExpanded));
            _listenersList = new FoldoutList<ScriptableEventListenerBase>(((ScriptableEventBase)target).Listeners, listenersExpanded);
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                DrawMissingScript();
                return;
            }

            serializedObject.UpdateIfRequiredOrScript();

            _buttonsDrawer.DrawButtons(targets);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
                _listenersList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
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