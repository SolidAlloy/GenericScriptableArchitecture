namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using EasyButtons.Editor;
    using SolidUtilities.Editor;
    using UnityEditor;
    using Object = UnityEngine.Object;

    internal class ScriptableEventHelperDrawer
    {
        private readonly Button _methodDrawer;
        private readonly FoldoutList<Object> _listenersList;
        private readonly StackTraceDrawer _stackTrace;
        private readonly ScriptableEventHelper _helper;

        public ScriptableEventHelperDrawer(SerializedProperty helperProperty, ScriptableEventHelper helper, Action repaint, Func<string>[] getArgNames)
        {
            _helper = helper;

            var responseTargetsExpanded = helperProperty.FindPropertyRelative(nameof(ScriptableEventHelper.ListenersExpanded));
            _listenersList = new FoldoutList<Object>(helper.Listeners, responseTargetsExpanded, "Listeners");

            _stackTrace = new StackTraceDrawer(helper.StackTrace.Entries, helperProperty.FindPropertyRelative(nameof(ScriptableEventHelper.StackTrace)), repaint);

            var invokeMethod = helper.GetType().GetMethod("Invoke");
            _methodDrawer = Button.Create(invokeMethod, getArgNames);
        }

        public void Update() => _listenersList.Update(_helper.Listeners);

        public void DrawMethod(IEnumerable<object> targets)
        {
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight / 2);
            _methodDrawer.Draw(targets);
        }

        public void DrawStackTrace()
        {
            _stackTrace.Draw();
        }

        public void DrawListeners()
        {
            if (ApplicationUtil.InEditMode)
                return;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            _listenersList.DoLayoutList();
        }

        public static int GetGenericArgumentsCountOfType(UnityEngine.Object target)
        {
            Type targetType = target.GetType().BaseType;

            // ReSharper disable once PossibleNullReferenceException
            if (!targetType.IsGenericType)
                return 0;

            return targetType.GetGenericArguments().Length;
        }
    }
}