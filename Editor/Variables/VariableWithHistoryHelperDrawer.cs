namespace GenericScriptableArchitecture.Editor
{
    using System;
    using UnityEditor;
    using Object = UnityEngine.Object;

    internal class VariableWithHistoryHelperDrawer
    {
        private FoldoutList<Object> _listenersWithHistory;
        private readonly VariableHelperWithHistory _variableHelperWithHistory;
        private readonly SerializedObject _serializedObject;
        private readonly SerializedProperty _previousValue;
        private readonly Func<bool> _canDrawListeners;
        private readonly SerializedProperty _variableHelperWithHistoryProperty;
        private object _copiedCurrentValue; // need to be an instance field so that we can set it from a lambda

        public VariableWithHistoryHelperDrawer(VariableHelperWithHistory variableHelperWithHistory, SerializedProperty variableHelperWithHistoryProperty, Func<bool> canDrawListeners = null)
        {
            _variableHelperWithHistory = variableHelperWithHistory;
            _variableHelperWithHistoryProperty = variableHelperWithHistoryProperty;
            _serializedObject = variableHelperWithHistoryProperty.serializedObject;
            _canDrawListeners = canDrawListeners;
            _previousValue = variableHelperWithHistoryProperty.FindPropertyRelative(nameof(VariableHelperWithHistory<int>.PreviousValue));
        }

        public void Update()
        {
            _listenersWithHistory?.Update(_variableHelperWithHistory.EventHelper.Listeners);
        }

        private static FoldoutList<Object> InitializeListenersWithHistory(VariableHelperWithHistory variableHelper, SerializedProperty variableHelperProperty)
        {
            var expanded = variableHelperProperty.FindPropertyRelative(nameof(VariableHelperWithHistory.ListenersWithHistoryExpanded));
            return new FoldoutList<Object>(variableHelper.EventHelper.Listeners, expanded, "Listeners For Value Change With History");
        }

        public void DrawCurrentValue(VariableHelperDrawer variableHelperDrawer)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (variableHelperDrawer.DrawCurrentValueChange(() => _copiedCurrentValue = SerializationHelper.CreateCopyWeak(variableHelperDrawer.Value)))
            {
                // Apply the changed value so that it is not lost.
                _serializedObject.ApplyModifiedProperties();

                // Set the previousValue
                _variableHelperWithHistory.SetPreviousValue(_copiedCurrentValue);

                // Set "HasPreviousValue" to true because it might still be false and is not set automatically.
                _variableHelperWithHistory.HasPreviousValue = true;

                // Load the previous value to serialized object so that it is updated in the inspector
                _serializedObject.Update();

                variableHelperDrawer.InvokeChangeEvents();
                _variableHelperWithHistory.InvokeValueChangedEvents(variableHelperDrawer.Value);
            }
        }

        public void DrawPreviousValue()
        {
            if (_variableHelperWithHistory.HasPreviousValue)
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(_previousValue);
            }
            else
            {
                EditorGUILayout.LabelField("Previous Value", "Not set yet");
            }
        }

        public void DrawListenersWithHistory()
        {
            if ( ! EditorApplication.isPlaying || (_canDrawListeners != null && !_canDrawListeners()))
                return;

            if (_listenersWithHistory == null)
                _listenersWithHistory = InitializeListenersWithHistory(_variableHelperWithHistory, _variableHelperWithHistoryProperty);

            _listenersWithHistory.DoLayoutList();
        }
    }
}