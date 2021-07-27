namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;
    using UnityEngine;

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

            if (WithHistory)
                InitializeListenersWithHistory();

            _stackTrace = new StackTraceDrawer(VariableBase, this);
        }

        private void InitializeListeners()
        {
            var expanded = serializedObject.FindProperty(nameof(Variable<bool>.ListenersExpanded));

            _listeners = new FoldoutList<Object>(VariableBase.Listeners,
                expanded, "Listeners For Value Change");
        }

        private void InitializeListenersWithHistory()
        {
            var expanded = serializedObject.FindProperty(
                nameof(VariableWithHistory<bool>.ListenersWithHistoryExpanded));

            _listenersWithHistory = new FoldoutList<Object>(VariableBase.ListenersWithHistory,
                expanded, "Listeners For Value Change With History");
        }

        protected override void DrawFields()
        {
            EditorGUILayout.PropertyField(_description);

            DrawInitialValue();
            DrawCurrentValue();
            DrawPreviousValue();

            _stackTrace.Draw();

            if ( ! EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listeners.DoLayoutList();
            _listenersWithHistory?.DoLayoutList();
        }
    }
}