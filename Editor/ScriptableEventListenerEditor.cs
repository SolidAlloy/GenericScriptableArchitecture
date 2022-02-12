namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.Editor;
    using UnityEditor;

    [CustomEditor(typeof(BaseScriptableEventListener), true)]
    public class ScriptableEventListenerEditor : Editor, IRepaintable
    {
        private SerializedProperty _eventProperty;
        private SerializedProperty _responseProperty;
        private StackTraceDrawer _stackTrace;
        private bool _initialized;
        private BaseScriptableEventListener _target;

        private void OnEnable()
        {
            // The targets length is 0 or the first target is null for a couple frames after the domains reload.
            // We need to avoid exceptions while the target is not set by Unity.
            if (targets.Length == 0 || target == null)
            {
                return;
            }

            _target = target as BaseScriptableEventListener;

            _eventProperty = serializedObject.FindProperty(nameof(VoidScriptableEventListener._event));
            _responseProperty = serializedObject.FindProperty(nameof(VoidScriptableEventListener._response));

            _stackTrace = new StackTraceDrawer((IStackTraceProvider) target, this);

            _initialized = true;
        }

        protected override void OnHeaderGUI()
        {
            GenericHeaderUtility.OnHeaderGUI(this);
        }

        public override void OnInspectorGUI()
        {
            if (!_initialized)
            {
                OnEnable();
                return;
            }

            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            if (_eventProperty.propertyType != SerializedPropertyType.ObjectReference || _target.DrawObjectField)
                EditorGUILayout.PropertyField(_eventProperty);

            EditorGUILayout.PropertyField(_responseProperty);

            Undo.RecordObject(target, "Changed stack trace settings");
            _stackTrace.Draw();
        }
    }
}