namespace GenericScriptableArchitecture.Editor
{
    using System;
    using UnityEditor;

#if MISSING_SCRIPT_TYPE
    using MissingScriptType.Editor;
#endif

    internal class InspectorGUIHelper
    {
        private readonly SerializedObject _serializedObject;

#if MISSING_SCRIPT_TYPE
        private readonly MissingScriptTypeUtility _missingScriptTypeUtility;
#endif

        public InspectorGUIHelper(SerializedObject serializedObject)
        {
#if MISSING_SCRIPT_TYPE
            _missingScriptTypeUtility = new MissingScriptTypeUtility(serializedObject);
#endif
            _serializedObject = serializedObject;
        }

        public Wrapper Wrap()
        {
            bool hasMissingScript = _serializedObject.targetObject == null;

            if (hasMissingScript)
            {
#if MISSING_SCRIPT_TYPE
                _missingScriptTypeUtility.Draw();
#else
                using (new EditorGUI.DisabledScope(true))
                {
                    var scriptProperty = _serializedObject.FindProperty("m_Script");

                    if (scriptProperty != null)
                        EditorGUILayout.PropertyField(scriptProperty);
                }
#endif
            }
            else
            {
                _serializedObject.UpdateIfRequiredOrScript();
            }

            return new Wrapper(_serializedObject, hasMissingScript);
        }

        public readonly struct Wrapper : IDisposable
        {
            private readonly SerializedObject _serializedObject;
            public readonly bool HasMissingScript;

            public Wrapper(SerializedObject serializedObject, bool hasMissingScript)
            {
                _serializedObject = serializedObject;
                HasMissingScript = hasMissingScript;
            }

            public void Dispose()
            {
                if ( ! HasMissingScript)
                    _serializedObject.ApplyModifiedProperties();
            }
        }
    }
}