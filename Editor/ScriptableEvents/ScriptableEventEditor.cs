namespace GenericScriptableArchitecture.Editor
{
    using EasyButtons.Editor;
    using UnityEditor;

    [CustomEditor(typeof(BaseScriptableEvent), true)]
    internal class ScriptableEventEditor : PlayModeUpdatableEditor, IRepaintable
    {
        private ButtonsDrawer _buttonsDrawer;
        private FoldoutList<UnityEngine.Object> _listenersList;
        private SerializedProperty _description;
        private StackTraceDrawer _stackTrace;
        private BaseScriptableEvent _typedTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            _buttonsDrawer = new ButtonsDrawer(target);
            _typedTarget = (BaseScriptableEvent) target;

            var responseTargetsExpanded = serializedObject.FindProperty(nameof(BaseScriptableEvent.ListenersExpanded));
            _listenersList = new FoldoutList<UnityEngine.Object>(_typedTarget.Listeners, responseTargetsExpanded, "Listeners");

            _description = serializedObject.FindProperty("_description");

            _stackTrace = new StackTraceDrawer(_typedTarget, this);
        }

        protected override void Update() => _listenersList.Update(_typedTarget.Listeners);

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.PropertyField(_description);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            _buttonsDrawer.DrawButtons(targets);

            _stackTrace.Draw();

            if (ApplicationUtil.InEditMode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listenersList.DoLayoutList();
        }
    }
}