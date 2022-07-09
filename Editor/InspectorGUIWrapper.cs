namespace GenericScriptableArchitecture.Editor
{
    using System;
    using UnityEditor;

    internal readonly struct InspectorGUIWrapper : IDisposable
    {
        private readonly SerializedObject _serializedObject;
        public readonly bool HasMissingScript;

        public InspectorGUIWrapper(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;

            HasMissingScript = serializedObject.targetObject == null;

            if (HasMissingScript)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    var scriptProperty = serializedObject.FindProperty("m_Script");

                    if (scriptProperty != null)
                        EditorGUILayout.PropertyField(scriptProperty);
                }
            }
            else
            {
                serializedObject.UpdateIfRequiredOrScript();
            }
        }

        public void Dispose()
        {
            if ( ! HasMissingScript)
                _serializedObject.ApplyModifiedProperties();
        }
    }
}