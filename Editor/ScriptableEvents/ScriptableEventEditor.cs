namespace GenericScriptableArchitecture.Editor
{
    using EasyButtons.Editor;
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;

    [CustomEditor(typeof(ScriptableEventBase), true)]
    internal class ScriptableEventEditor : GenericHeaderEditor
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
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            _buttonsDrawer.DrawButtons(targets);

            if ( ! EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listenersList.DoLayoutList();
        }
    }
}