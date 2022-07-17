namespace GenericScriptableArchitecture.Editor
{
    using System;
    using GenericUnityObjects.Editor;
    using UnityEditor;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(BaseEventInstancer), true)]
    internal class EventInstancerEditor : Editor, IInlineDrawer
    {
        private SerializedProperty _baseField;
        private SerializedProperty _argNamesProperty;

        private ScriptableEventHelperDrawer _helperDrawer;
        private InspectorGUIHelper _inspectorGUIHelper;
        private PlayModeUpdateHelper _updateHelper;

        private void OnEnable()
        {
            try { _inspectorGUIHelper = new InspectorGUIHelper(serializedObject); }
            catch { return; }

            if (target == null)
                return;

            _baseField = serializedObject.FindProperty(nameof(EventInstancer._base));

            if (_baseField.objectReferenceValue != null)
                _argNamesProperty = GetArgNamesProperty(_baseField.objectReferenceValue);

            var helper = ((BaseEventInstancer)target).ScriptableEventHelper;
            var genericArgCount = ScriptableEventHelperDrawer.GetGenericArgumentsCountOfType(target);
            var getArgNames = GetArgNamesFuncArray(genericArgCount);

            var eventHelperProperty = serializedObject.FindProperty(nameof(ScriptableEvent._scriptableEventHelper));
            _helperDrawer = new ScriptableEventHelperDrawer(eventHelperProperty, helper, Repaint, getArgNames);
            _updateHelper = new PlayModeUpdateHelper(this, () => _helperDrawer.Update());
        }

        private void OnDisable()
        {
            _updateHelper?.Dispose();
        }

        protected override void OnHeaderGUI()
        {
            GenericHeaderUtility.OnHeaderGUI(this);
        }

        public override void OnInspectorGUI()
        {
            if (_inspectorGUIHelper == null)
            {
                base.OnInspectorGUI();
                return;
            }

            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_baseField);

            if (EditorGUI.EndChangeCheck() && _baseField.objectReferenceValue != null)
                _argNamesProperty = GetArgNamesProperty(_baseField.objectReferenceValue);

            if (!EditorApplication.isPlaying)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            _helperDrawer.DrawMethod(targets);
            _helperDrawer.DrawStackTrace();
            _helperDrawer.DrawListeners();
        }

        public bool HasContent => EditorApplication.isPlaying;

        public void OnInlineGUI()
        {
            using var guiWrapper = _inspectorGUIHelper.Wrap();

            if (!guiWrapper.HasMissingScript)
                _helperDrawer.DrawMethod(targets);
        }

        private static SerializedProperty GetArgNamesProperty(Object scriptableEvent)
        {
            var serializedObject = new SerializedObject(scriptableEvent);
            return serializedObject.FindProperty(nameof(BaseScriptableEvent._argNames));
        }

        private Func<string>[] GetArgNamesFuncArray(int genericArgCount)
        {
            var getArgNames = new Func<string>[genericArgCount];

            for (int i = 0; i < getArgNames.Length; i++)
            {
                int capturedI = i;
                getArgNames[i] = () =>
                {
                    if (_argNamesProperty == null || _argNamesProperty.arraySize <= capturedI)
                        return $"Arg {capturedI + 1}";

                    return _argNamesProperty.GetArrayElementAtIndex(capturedI).stringValue;
                };
            }

            return getArgNames;
        }
    }
}