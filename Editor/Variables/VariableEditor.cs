namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(BaseVariable), true)]
    internal class VariableEditor : VariableEditorBase, IRepaintable
    {
        private SerializedProperty _description;
        private FoldoutList<Object> _listeners;
        private FoldoutList<Object> _listenersWithHistory;
        private StackTraceDrawer _stackTrace;

        protected override void OnEnable()
        {
            base.OnEnable();
            _description = serializedObject.FindProperty("_description");

            InitializeListeners();

            if (_withHistory)
                InitializeListenersWithHistory();

            _stackTrace = new StackTraceDrawer(_baseVariable, this);
        }

        private void InitializeListeners()
        {
            var expanded = serializedObject.FindProperty(nameof(Variable<bool>.ListenersExpanded));
            _listeners = new FoldoutList<Object>(_baseVariable.Listeners, expanded, "Listeners For Value Change");
        }

        private void InitializeListenersWithHistory()
        {
            var expanded = serializedObject.FindProperty(nameof(VariableWithHistory<bool>.ListenersWithHistoryExpanded));
            _listenersWithHistory = new FoldoutList<Object>(_baseVariable.ListenersWithHistory, expanded, "Listeners For Value Change With History");
        }

        protected override void DrawFields()
        {
            EditorGUILayout.PropertyField(_description);

            DrawInitialValue();

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DrawCurrentValue();
                DrawPreviousValue();
            }

            _stackTrace.Draw();

            if ( ! EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listeners.DoLayoutList();
            _listenersWithHistory?.DoLayoutList();
        }
    }
}