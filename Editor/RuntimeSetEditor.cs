namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(BaseRuntimeSet), true)]
    public class RuntimeSetEditor : GenericHeaderEditor
    {
        private ReorderableList _listDrawer;

        private List<FoldoutList<Object>> _eventListeners;

        private void OnEnable()
        {
            _listDrawer = GetReorderableList();

            var baseRuntimeSet = (BaseRuntimeSet) target;

            _eventListeners = new List<FoldoutList<Object>>
            {
                new FoldoutList<Object>(baseRuntimeSet.AddListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.AddExpanded)), "Add Event Listeners"),
                new FoldoutList<Object>(baseRuntimeSet.CountChangeListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.CountChangeExpanded)), "Count Change Event Listeners"),
                new FoldoutList<Object>(baseRuntimeSet.MoveListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.MoveExpanded)), "Move Event Listeners"),
                new FoldoutList<Object>(baseRuntimeSet.RemoveListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.RemoveExpanded)), "Remove Event Listeners"),
                new FoldoutList<Object>(baseRuntimeSet.ReplaceListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.ReplaceExpanded)), "Replace Event Listeners"),
                new FoldoutList<Object>(baseRuntimeSet.ResetListeners, serializedObject.FindProperty(nameof(BaseRuntimeSet.ResetExpanded)), "Reset Event Listeners"),
            };
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