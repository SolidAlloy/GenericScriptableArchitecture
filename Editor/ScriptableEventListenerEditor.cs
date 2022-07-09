namespace GenericScriptableArchitecture.Editor
{
    using System;
    using ExtEvents.Editor;
    using GenericUnityObjects.Editor;
    using SolidUtilities.Editor;
    using UnityEditor;

    [CustomEditor(typeof(BaseScriptableEventListener), true)]
    public class ScriptableEventListenerEditor : Editor
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
            _stackTrace = new StackTraceDrawer(_target._stackTrace.Entries, serializedObject.FindProperty(nameof(BaseScriptableEventListener._stackTrace)), Repaint);
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

            using var guiWrapper = new InspectorGUIWrapper(serializedObject);

            if (guiWrapper.HasMissingScript)
                return;

            DrawEventField(_eventProperty);

            // Set custom names for the dynamic arguments of ExtEvent.
            if (_argNames.Length != 0)
                ExtEventDrawer.SetOverrideArgNames(_argNames);

            EditorGUILayout.PropertyField(_responseProperty);

            ExtEventDrawer.ResetOverrideArgNames();

            Undo.RecordObject(target, "Changed stack trace settings");
            _stackTrace.Draw();
        }

        private void DrawEventField(SerializedProperty eventProperty)
        {
            if (ShowEventField && (_eventProperty.propertyType != SerializedPropertyType.ObjectReference || _target.DrawObjectField))
            {
                EditorGUILayout.PropertyField(eventProperty);
                return;
            }

            if (_target.Event is BaseScriptableEvent)
                return;

            // If it is not a scriptable event, it means it is an event holder.
            // Since we shouldn't draw an object field, we only need to draw the notifyCurrentValue bool field if it is required.
            var eventTypeProperty = eventProperty.FindPropertyRelative(nameof(EventHolder<int>._type));
            var eventType = (EventTypes) eventTypeProperty.enumValueIndex;

            if (eventType.HasDefaultValue())
            {
                EditorGUILayout.PropertyField(
                    eventProperty.FindPropertyRelative(nameof(EventHolder<int>._notifyCurrentValue)),
                    GUIContentHelper.Temp(EventHolderDrawerUtil.NotifyCurrentValueLabel));
            }
        }

        private static string[] GetArgNames(SerializedProperty eventProperty)
        {
            // case: field is ScriptableEvent
            if (eventProperty.type.StartsWith("ScriptableEvent"))
                return GetArgNamesFromScriptableEvent(eventProperty);

            // case: field is EventHolder. Check its type;
            var eventTypeProperty = eventProperty.FindPropertyRelative(nameof(EventHolder<int>._type));
            var eventType = (EventTypes) eventTypeProperty.enumValueIndex;

            switch (eventType)
            {
                case EventTypes.ScriptableEvent:
                    eventProperty = eventProperty.FindPropertyRelative(nameof(EventHolder<int>._event));
                    return GetArgNamesFromScriptableEvent(eventProperty);

                case EventTypes.Variable:
                    var variableProperty = eventProperty.FindPropertyRelative(nameof(EventHolder<int>._variable));

                    return variableProperty.type.StartsWith("VariableWithHistory")
                        ? new[] { "Previous Value", "Value" }
                        : new[] { "Value" };

                case EventTypes.VariableInstancer:
                    return new[] { "Value" };
            }

            return Array.Empty<string>();
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