namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(VariableBase), true)]
    internal class VariableEditor : VariableEditorBase
    {
        private SerializedProperty _description;
        private FoldoutList<ScriptableEventListenerBase> _listenersOnChanged;
        private FoldoutList<ScriptableEventListenerBase> _listenersOnChangedWithHistory;

        protected override void OnEnable()
        {
            base.OnEnable();
            _description = serializedObject.FindProperty("_description");

            InitializeListeners();

            if (WithHistory)
                InitializeListenersWithHistory();
        }

        private void InitializeListeners()
        {
            var expanded = serializedObject.FindProperty(nameof(Variable<bool>.ListenersExpanded));

            _listenersOnChanged = new FoldoutList<ScriptableEventListenerBase>(VariableBase.Listeners,
                expanded, "Listeners For Value Change");
        }

        private void InitializeListenersWithHistory()
        {
            var expanded = serializedObject.FindProperty(
                nameof(VariableWithHistory<bool>.ListenersWithHistoryExpanded));

            _listenersOnChangedWithHistory = new FoldoutList<ScriptableEventListenerBase>(VariableBase.ListenersWithHistory,
                expanded, "Listeners For Value Change With History");
        }

        protected override void DrawFields()
        {
            EditorGUILayout.PropertyField(_description);

            DrawInitialValue();
            DrawCurrentValue();
            DrawPreviousValue();

            if ( ! EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            _listenersOnChanged.DoLayoutList();
            _listenersOnChangedWithHistory?.DoLayoutList();
        }
    }
}