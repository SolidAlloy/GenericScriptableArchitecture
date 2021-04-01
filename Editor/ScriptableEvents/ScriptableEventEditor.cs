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
        private FoldoutList<UnityEngine.Object> _responseTargetsList;

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
            var typedTarget = (ScriptableEventBase) target;

            var listenersExpanded = serializedObject.FindProperty(nameof(ScriptableEventBase.ListenersExpanded));
            _listenersList = new FoldoutList<ScriptableEventListenerBase>(typedTarget.Listeners, listenersExpanded, "Listeners");

            var responseTargetsExpanded = serializedObject.FindProperty(nameof(ScriptableEventBase.ResponseTargetsExpanded));
            _responseTargetsList = new FoldoutList<UnityEngine.Object>(typedTarget.ResponseTargets, responseTargetsExpanded, "Response Targets");
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

            _responseTargetsList.DoLayoutList();
        }
    }
}