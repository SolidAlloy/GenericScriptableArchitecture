namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;

    [CustomEditor(typeof(BaseScriptableEventListener), true)]
    public class ScriptableEventListenerEditor : GenericHeaderEditor, IRepaintable
    {
        private SerializedProperty _eventProperty;
        private SerializedProperty _responseProperty;

        private StackTraceDrawer _stackTrace;

        private void OnEnable()
        {
            _eventProperty = serializedObject.FindProperty(nameof(ScriptableEventListener._event));
            _responseProperty = serializedObject.FindProperty(nameof(ScriptableEventListener._response));

            _stackTrace = new StackTraceDrawer((IStackTraceProvider) target, this);
        }

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.PropertyField(_eventProperty);
            EditorGUILayout.PropertyField(_responseProperty);

            Undo.RecordObject(target, "Changed stack trace settings");
            _stackTrace.Draw();
        }
    }
}