namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using EasyButtons.Editor;
    using SolidUtilities.Editor;
    using UnityEditor;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(BaseScriptableEvent), true)]
    internal class ScriptableEventEditor : PlayModeUpdatableEditor, IRepaintable
    {
        private Button _methodDrawer;
        private FoldoutList<Object> _listenersList;
        private SerializedProperty _description;
        private StackTraceDrawer _stackTrace;
        private BaseScriptableEvent _typedTarget;
        private SerializedProperty _argNamesProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _typedTarget = (BaseScriptableEvent) target;

            var responseTargetsExpanded = serializedObject.FindProperty(nameof(BaseScriptableEvent.ListenersExpanded));
            _listenersList = new FoldoutList<Object>(_typedTarget.Listeners, responseTargetsExpanded, "Listeners");

            _description = serializedObject.FindProperty("_description");

            _stackTrace = new StackTraceDrawer(_typedTarget, this);

            var genericArgCount = GetGenericArgumentsCountOfType(target);
            _argNamesProperty = GetArgNamesProperty(serializedObject, genericArgCount);

            var getArgNames = GetArgNamesFuncArray(_argNamesProperty);
            var invokeMethod = target.GetType().GetMethod("Invoke");
            _methodDrawer = Button.Create(invokeMethod, getArgNames);

        }

        protected override void Update() => _listenersList.Update(_typedTarget.Listeners);

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.PropertyField(_description);

            for (int i = 0; i < _argNamesProperty.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_argNamesProperty.GetArrayElementAtIndex(i), GUIContentHelper.Temp($"Arg {i+1}"), null);
            }

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            _methodDrawer.Draw(targets);

            _stackTrace.Draw();

            if (ApplicationUtil.InEditMode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listenersList.DoLayoutList();
        }

        private static int GetGenericArgumentsCountOfType(Object obj)
        {
            if (obj == null)
                return 0;

            var baseType = obj.GetType().BaseType;

            // ReSharper disable once PossibleNullReferenceException
            if (!baseType.IsGenericType)
                return 0;

            return baseType.GetGenericArguments().Length;
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