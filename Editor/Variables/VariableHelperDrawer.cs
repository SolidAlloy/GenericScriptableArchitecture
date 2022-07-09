namespace GenericScriptableArchitecture.Editor
{
    using System;
    using SolidUtilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    internal class VariableHelperDrawer
    {
        public static readonly GUIContent CurrentValueLabel = new GUIContent("Current Value");

        private readonly SerializedProperty _value;
        private FoldoutList<Object> _listeners;
        private readonly VariableHelper _variableHelper;
        private readonly PropertyChangeObserver _propertyChangeObserver;
        private readonly StackTraceDrawer _stackTrace;
        private readonly SerializedProperty _variableHelperProperty;
        private readonly Func<bool> _canDrawListeners;

        public object Value => _value.GetObject();

        public VariableHelperDrawer(VariableHelper variableHelper, SerializedProperty variableHelperProperty, Action repaint, Func<bool> canDrawListeners = null)
        {
            _variableHelper = variableHelper;
            _variableHelperProperty = variableHelperProperty;
            _canDrawListeners = canDrawListeners;
            _value = variableHelperProperty.FindPropertyRelative(nameof(VariableHelper<int>.Value));
            _propertyChangeObserver = new PropertyChangeObserver(_value);
            _stackTrace = new StackTraceDrawer(variableHelper.StackTrace.Entries, _variableHelperProperty.FindPropertyRelative(nameof(VariableHelper.StackTrace)), repaint);
        }

        public void Update()
        {
            _listeners?.Update(_variableHelper.EventHelper.Listeners);
        }

        public void DrawCurrentValue()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (DrawCurrentValueChange())
                InvokeChangeEvents();
        }

        public bool DrawCurrentValueChange(Action onChange = null)
        {
            return _propertyChangeObserver.DrawProperty(property => EditorGUILayout.PropertyField(property, CurrentValueLabel), onChange);
        }

        public void InvokeChangeEvents()
        {
            // Apply the change value, otherwise the event will be invoked with the old value.
            _value.serializedObject.ApplyModifiedProperties();
            _variableHelper.InvokeValueChangedEvents();
        }

        public void DrawStackTrace() => _stackTrace.Draw();

        public void DrawListeners()
        {
            if ( ! EditorApplication.isPlaying || (_canDrawListeners != null && !_canDrawListeners()))
                return;

            if (_listeners == null)
                _listeners = InitializeListeners(_variableHelperProperty, _variableHelper);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listeners.DoLayoutList();
        }

        private static FoldoutList<Object> InitializeListeners(SerializedProperty variableHelperProperty, VariableHelper variableHelper)
        {
            var expanded = variableHelperProperty.FindPropertyRelative(nameof(VariableHelper<int>.ListenersExpanded));
            return new FoldoutList<Object>(variableHelper.EventHelper.Listeners, expanded, "Listeners For Value Change");
        }
    }
}