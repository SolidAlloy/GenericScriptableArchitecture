namespace GenericScriptableArchitecture.Editor
{
    using System;
    using ExtEvents.Editor;
    using GenericUnityObjects.Editor;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using EventType = GenericScriptableArchitecture.EventType;
    using Object = UnityEngine.Object;

    public class ScriptableEventListenerEditorHelper
    {
        private readonly SerializedProperty _eventHolderProperty;
        private readonly SerializedProperty _responseProperty;
        private readonly string[] _argNames;
        private readonly StackTraceDrawer _stackTrace;
        private readonly SerializedProperty _notifyValueProperty;
        private readonly SerializedProperty _eventTypeProperty;

        private readonly SerializedProperty _eventProperty;
        private readonly SerializedProperty _variableProperty;
        private readonly SerializedProperty _variableInstancerProperty;
        private readonly SerializedProperty _eventInstancerProperty;

        public SerializedObject SerializedObject { get; }

        public ScriptableEventListenerEditorHelper(SerializedObject serializedObject, BaseScriptableEventListener target, Action repaint)
        {
            SerializedObject = serializedObject;
            _eventHolderProperty = serializedObject.FindProperty(nameof(VoidScriptableEventListener._event));
            _eventTypeProperty = _eventHolderProperty.FindPropertyRelative(nameof(EventHolder<int>._type));
            _notifyValueProperty = _eventHolderProperty.FindPropertyRelative(nameof(EventHolder<int>._notifyCurrentValue));
            _responseProperty = serializedObject.FindProperty(nameof(VoidScriptableEventListener._response));

            _eventProperty = _eventHolderProperty.FindPropertyRelative(nameof(EventHolder<int>._event));
            _variableProperty = _eventHolderProperty.FindPropertyRelative(nameof(EventHolder<int>._variable));
            _variableInstancerProperty = _eventHolderProperty.FindPropertyRelative(nameof(EventHolder<int>._variableInstancer));
            _eventInstancerProperty = _eventHolderProperty.FindPropertyRelative(nameof(EventHolder<int>._eventInstancer));

            _argNames = GetArgNames(_eventTypeProperty);
            _stackTrace = new StackTraceDrawer(target._stackTrace.Entries, serializedObject.FindProperty(nameof(BaseScriptableEventListener._stackTrace)), repaint);
        }

        public Object GetCurrentEventValue() => GetEventValueProperty(_eventTypeProperty).objectReferenceValue;

        public void DrawInlineGUI()
        {
            SerializedObject.UpdateIfRequiredOrScript();
            DrawNotifyCurrentValue();
            DrawResponse();
            DrawStackTrace();
            SerializedObject.ApplyModifiedProperties();
        }

        public void DrawEvent()
        {
            EditorGUILayout.PropertyField(_eventHolderProperty);
        }

        public void DrawNotifyCurrentValue()
        {
            if (GetEventType(_eventTypeProperty).HasDefaultValue())
                EditorGUILayout.PropertyField(_notifyValueProperty, GUIContentHelper.Temp("Notify current value"));
        }

        public void DrawResponse()
        {
            // Set custom names for the dynamic arguments of ExtEvent.
            if (_argNames.Length != 0)
                ExtEventDrawer.SetOverrideArgNames(_argNames);

            EditorGUILayout.PropertyField(_responseProperty);

            ExtEventDrawer.ResetOverrideArgNames();
        }

        public void DrawStackTrace()
        {
            _stackTrace.Draw();
        }

        public void SetEventValue(Object eventValue)
        {
            if (eventValue is BaseScriptableEvent)
            {
                _eventTypeProperty.enumValueIndex = (int) EventType.ScriptableEvent;
                _eventProperty.objectReferenceValue = eventValue;
            }
            else if (eventValue is BaseEventInstancer)
            {
                _eventTypeProperty.enumValueIndex = (int) EventType.EventInstancer;
                _eventInstancerProperty.objectReferenceValue = eventValue;
            }
            else if (eventValue is BaseVariable)
            {
                _eventTypeProperty.enumValueIndex = (int) EventType.Variable;
                _variableProperty.objectReferenceValue = eventValue;
            }
            else if (eventValue is BaseVariableInstancer)
            {
                _eventTypeProperty.enumValueIndex = (int) EventType.VariableInstancer;
                _variableInstancerProperty.objectReferenceValue = eventValue;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            // this also applies the other changed properties of serialized object.
            SerializedObject.SetHideFlagsPersistently(HideFlags.HideInInspector);
        }

        private static EventType GetEventType(SerializedProperty eventTypeProperty) => (EventType) eventTypeProperty.enumValueIndex;

        private SerializedProperty GetEventValueProperty(SerializedProperty eventTypeProperty)
        {
            return GetEventType(eventTypeProperty) switch
            {
                EventType.ScriptableEvent => _eventProperty,
                EventType.EventInstancer => _eventInstancerProperty,
                EventType.Variable => _variableProperty,
                EventType.VariableInstancer => _variableInstancerProperty,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private string[] GetArgNames(SerializedProperty eventTypeProperty)
        {
            switch (GetEventType(eventTypeProperty))
            {
                case EventType.ScriptableEvent:
                    return GetArgNamesFromScriptableEvent(_eventProperty);

                case EventType.EventInstancer:
                    return GetArgNamesFromEventInstancer(_eventInstancerProperty);

                case EventType.Variable:
                    return _variableProperty.type.StartsWith("VariableWithHistory")
                        ? new[] { "Previous Value", "Value" }
                        : new[] { "Value" };

                case EventType.VariableInstancer:
                    return _variableInstancerProperty.type.StartsWith("VariableInstancerWithHistory")
                        ? new[] { "Previous Value", "Value" }
                        : new[] { "Value" };
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

        private static string[] GetArgNamesFromEventInstancer(SerializedProperty eventInstancerProperty)
        {
            if (eventInstancerProperty.objectReferenceValue == null)
                return Array.Empty<string>();

            var eventInstancer = (BaseEventInstancer) eventInstancerProperty.objectReferenceValue;
            var scriptableEvent = eventInstancer.Base;

            if (scriptableEvent == null)
                return Array.Empty<string>();

            return scriptableEvent._argNames;
        }
    }

    [CustomEditor(typeof(BaseScriptableEventListener), true)]
    public class ScriptableEventListenerEditor : Editor
    {
        private ScriptableEventListenerEditorHelper _drawer;
        private bool _initialized;

        private void OnEnable()
        {
            // The targets length is 0 or the first target is null for a couple frames after the domains reload.
            // We need to avoid exceptions while the target is not set by Unity.
            if (targets.Length == 0 || target == null)
                return;

            _drawer = new ScriptableEventListenerEditorHelper(serializedObject, (BaseScriptableEventListener) target, Repaint);
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

            _drawer.DrawEvent();
            _drawer.DrawResponse();
            _drawer.DrawStackTrace();
        }
    }
}