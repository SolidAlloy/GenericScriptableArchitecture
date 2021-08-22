namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomEditor(typeof(BaseRuntimeSet), true)]
    internal class RuntimeSetEditor : PlayModeUpdatableEditor
    {
        private ReorderableList _listDrawer;
        private List<FoldoutList<Object>> _eventListeners;
        private BaseRuntimeSet _typedTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            _listDrawer = GetReorderableList();

            _typedTarget = (BaseRuntimeSet) target;

            _eventListeners = new List<FoldoutList<Object>>
            {
                new FoldoutList<Object>(_typedTarget.AddListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.AddExpanded)), "Add Event Listeners"),
                new FoldoutList<Object>(_typedTarget.CountChangeListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.CountChangeExpanded)), "Count Change Event Listeners"),
                new FoldoutList<Object>(_typedTarget.MoveListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.MoveExpanded)), "Move Event Listeners"),
                new FoldoutList<Object>(_typedTarget.RemoveListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.RemoveExpanded)), "Remove Event Listeners"),
                new FoldoutList<Object>(_typedTarget.ReplaceListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.ReplaceExpanded)), "Replace Event Listeners"),
                new FoldoutList<Object>(_typedTarget.ResetListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.ResetExpanded)), "Reset Event Listeners"),
            };
        }

        protected override void Update()
        {
            _eventListeners[0].Update(_typedTarget.AddListeners);
            _eventListeners[1].Update(_typedTarget.CountChangeListeners);
            _eventListeners[2].Update(_typedTarget.MoveListeners);
            _eventListeners[3].Update(_typedTarget.RemoveListeners);
            _eventListeners[4].Update(_typedTarget.ReplaceListeners);
            _eventListeners[5].Update(_typedTarget.ResetListeners);
        }

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.Space(10f);

            _listDrawer.DoLayoutList();

            if ( ! EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            foreach (var listenersList in _eventListeners)
                listenersList.DoLayoutList();
        }

        private ReorderableList GetReorderableList()
        {
            const float spacingBetweenElements = 4f;
            var elements = ((BaseRuntimeSet) target).List;

            return new ReorderableList(elements, typeof(Object), false, true, false, false)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Collection Items"),
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var objectFieldRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.ObjectField(objectFieldRect, GUIContent.none, elements[index], typeof(Object), true);
                    }
                },
                elementHeightCallback = index => EditorGUIUtility.singleLineHeight + (index == elements.Count - 1 ? 0f : spacingBetweenElements)
            };
        }
    }
}