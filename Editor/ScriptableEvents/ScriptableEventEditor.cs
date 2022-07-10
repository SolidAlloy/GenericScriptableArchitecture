namespace GenericScriptableArchitecture.Editor
{
    using System;
    using SolidUtilities.Editor;
    using UnityEditor;

    [CustomEditor(typeof(BaseScriptableEvent), true)]
    internal class ScriptableEventEditor : PlayModeUpdatableEditor
    {
        private SerializedProperty _description;
        private BaseScriptableEvent _typedTarget;
        private SerializedProperty _argNamesProperty;

        private ScriptableEventHelperDrawer _helperDrawer;

        protected override void OnEnable()
        {
            base.OnEnable();

            // TODO: No handling of null target?
            _typedTarget = (BaseScriptableEvent) target;

            var helper = _typedTarget.ScriptableEventHelper;
            var genericArgCount = ScriptableEventHelperDrawer.GetGenericArgumentsCountOfType(target);
            _argNamesProperty = GetArgNamesProperty(serializedObject, genericArgCount);
            var getArgNames = GetArgNamesFuncArray(_argNamesProperty);

            var eventHelperProperty = serializedObject.FindProperty(nameof(ScriptableEvent._scriptableEventHelper));
            _helperDrawer = new ScriptableEventHelperDrawer(eventHelperProperty, helper, Repaint, getArgNames);

            _description = serializedObject.FindProperty("_description");
        }

        protected override void Update() => _helperDrawer.Update();

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(serializedObject);

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.PropertyField(_description);

            for (int i = 0; i < _argNamesProperty.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_argNamesProperty.GetArrayElementAtIndex(i),
                    GUIContentHelper.Temp($"Arg {i + 1}"), null);
            }

            _helperDrawer.DrawMethod(targets);
            _helperDrawer.DrawStackTrace();
            _helperDrawer.DrawListeners();
        }

        private static SerializedProperty GetArgNamesProperty(SerializedObject serializedObject, int genericArgCount)
        {
            var argNamesProperty = serializedObject.FindProperty(nameof(BaseScriptableEvent._argNames));

            if (argNamesProperty.arraySize != genericArgCount)
            {
                InitializeArgNames(argNamesProperty, genericArgCount);
            }

            return argNamesProperty;
        }

        private static void InitializeArgNames(SerializedProperty argNamesProperty, int argsCount)
        {
            argNamesProperty.arraySize = argsCount;

            for (int i = 0; i < argsCount; i++)
            {
                var argNameProperty = argNamesProperty.GetArrayElementAtIndex(i);
                argNameProperty.stringValue = $"Arg {i+1}";
            }

            argNamesProperty.serializedObject.ApplyModifiedProperties();
        }

        private static Func<string>[] GetArgNamesFuncArray(SerializedProperty argNamesProperty)
        {
            var getArgNames = new Func<string>[argNamesProperty.arraySize];

            for (int i = 0; i < getArgNames.Length; i++)
            {
                int capturedI = i;
                getArgNames[i] = () => argNamesProperty.GetArrayElementAtIndex(capturedI).stringValue;
            }

            return getArgNames;
        }
    }
}