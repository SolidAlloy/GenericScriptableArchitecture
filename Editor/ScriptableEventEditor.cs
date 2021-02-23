namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Reflection;
    using EasyButtons.Editor;
    using GenericUnityObjects.Editor;
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(ScriptableEventBase), true)]
    public class ScriptableEventEditor : GenericHeaderEditor
    {
        private ButtonsDrawer _buttonsDrawer;
        private ReorderableList _listeners;
        private ScriptableEventBase _scriptableEventTarget;
        private Type _listenersType;
        private SerializedProperty _listenersExpanded;

        private Action _clearCache;

        private static readonly MethodInfo _clearCacheMethod = typeof(ReorderableList).GetMethod("ClearCache",
            BindingFlags.Instance | BindingFlags.NonPublic);

        private void OnEnable()
        {
            _buttonsDrawer = new ButtonsDrawer(target);
            _scriptableEventTarget = (ScriptableEventBase) target;
            _listenersExpanded = serializedObject.FindProperty(nameof(ScriptableEventBase.ListenersExpanded));

            _listeners = GetListenersList();
            _clearCache = (Action) Delegate.CreateDelegate(typeof(Action), _listeners, _clearCacheMethod);
        }

        private ReorderableList GetListenersList()
        {
            return new ReorderableList(_scriptableEventTarget.Listeners, typeof(ScriptableEventListenerBase),
                false, true, false, false)
            {
                drawHeaderCallback = rect =>
                {
                    const float leftMargin = 10f;
                    var shiftedRight = new Rect(rect.x + leftMargin, rect.y, rect.width - leftMargin, rect.height);
                    bool newValue = EditorGUI.Foldout(shiftedRight, _listenersExpanded.boolValue, "Listeners", true);

                    if (_listenersExpanded.boolValue == newValue)
                        return;

                    _listenersExpanded.boolValue = newValue;
                    _clearCache();
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    if ( ! _listenersExpanded.boolValue)
                    {
                        GUI.enabled = index == _scriptableEventTarget.Listeners.Count;
                        return;
                    }

                    var listener = _scriptableEventTarget.Listeners[index];

                    using (new EditorGUI.DisabledScope(true))
                        GenericObjectDrawer.ObjectField(rect, null, listener, GetListenerType(listener), true);
                },
                elementHeightCallback = indexer => _listenersExpanded.boolValue ? EditorGUIUtility.singleLineHeight : 0f,
            };
        }

        private Type GetListenerType(ScriptableEventListenerBase listener)
        {
            if (_listenersType == null)
            {
                _listenersType = listener.GetType().BaseType;
            }

            return _listenersType;
        }

        public override void OnInspectorGUI()
        {
            if (target == null)
            {
                DrawMissingScript();
                return;
            }

            serializedObject.UpdateIfRequiredOrScript();

            _buttonsDrawer.DrawButtons(targets);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            _listeners.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMissingScript()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }
        }
    }
}