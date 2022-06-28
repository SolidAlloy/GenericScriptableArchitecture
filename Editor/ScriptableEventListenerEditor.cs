namespace GenericScriptableArchitecture.Editor
{
    using System;
    using ExtEvents.Editor;
    using GenericUnityObjects.Editor;
    using UnityEditor;

    [CustomEditor(typeof(BaseScriptableEventListener), true)]
    public class ScriptableEventListenerEditor : Editor, IRepaintable
    {
        private SerializedProperty _eventProperty;
        private SerializedProperty _responseProperty;
        private string[] _argNames;
        private StackTraceDrawer _stackTrace;
        private bool _initialized;
        private BaseScriptableEventListener _target;

        public bool ShowEventField = true;

        private void OnEnable()
        {
            // The targets length is 0 or the first target is null for a couple frames after the domains reload.
            // We need to avoid exceptions while the target is not set by Unity.
            if (targets.Length == 0 || target == null)
                return;

            _target = target as BaseScriptableEventListener;
            _eventProperty = serializedObject.FindProperty(nameof(VoidScriptableEventListener._event));
            _responseProperty = serializedObject.FindProperty(nameof(VoidScriptableEventListener._response));
            _argNames = GetArgNames(_eventProperty);
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

            if (ShowEventField && (_eventProperty.propertyType != SerializedPropertyType.ObjectReference || _target.DrawObjectField))
                EditorGUILayout.PropertyField(_eventProperty);

            // Set custom names for the dynamic arguments of ExtEvent.
            if (_argNames.Length != 0)
                ExtEventDrawer.SetOverrideArgNames(_argNames);

            EditorGUILayout.PropertyField(_responseProperty);

            ExtEventDrawer.ResetOverrideArgNames();

            Undo.RecordObject(target, "Changed stack trace settings");
            _stackTrace.Draw();
        }

        private static string[] GetArgNames(SerializedProperty eventProperty)
        {
            // case: field is ScriptableEvent
            if (eventProperty.type.StartsWith("ScriptableEvent"))
                return GetArgNamesFromScriptableEvent(eventProperty);

            // case: field is EventHolder. Check its type;
            var eventType = eventProperty.FindPropertyRelative("_type");

            if (eventType.enumValueIndex == (int) EventTypes.ScriptableEvent)
            {
                // case: type of EventHolder is scriptable event. Get the field of scriptable event from event holder and do the same operation on it.
                eventProperty = eventProperty.FindPropertyRelative("_event");
                return GetArgNamesFromScriptableEvent(eventProperty);
            }

            // case: type of EventHolder is a variable.
            var variableProperty = eventProperty.FindPropertyRelative("_variable");

            // case: type of variable is VariableWithHistory.
            if (variableProperty.type.StartsWith("VariableWithHistory"))
                return new[] { "Previous Value", "Value" };

            // case: type of variable is Variable.
            return new[] { "Value" };
        }

        private static string[] GetArgNamesFromScriptableEvent(SerializedProperty scriptableEventProperty)
        {
            if (scriptableEventProperty.objectReferenceValue == null)
                return Array.Empty<string>();

            var scriptableEvent = (BaseScriptableEvent) scriptableEventProperty.objectReferenceValue;
            return scriptableEvent._argNames;
        }
    }
}