namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(CollectionBase), true)]
    public class CollectionEditor : GenericHeaderEditor
    {
        private SerializedProperty _onItemAdded;
        private SerializedProperty _onItemRemoved;

        private ReorderableList _listDrawer;

        private void OnEnable()
        {
            _onItemAdded = serializedObject.FindProperty(nameof(Collection<Object>._onItemAdded));
            _onItemRemoved = serializedObject.FindProperty(nameof(Collection<Object>._onItemRemoved));
            _listDrawer = GetReorderableList();
        }

        public override void OnInspectorGUI()
        {
            using var guiWrapper = new InspectorGUIWrapper(this);

            if (guiWrapper.HasMissingScript)
                return;

            EditorGUILayout.PropertyField(_onItemAdded);
            EditorGUILayout.PropertyField(_onItemRemoved);

            EditorGUILayout.Space(10f);

            _listDrawer.DoLayoutList();
        }

        private ReorderableList GetReorderableList()
        {
            var elements = ((CollectionBase) target).List;

            return new ReorderableList(elements, typeof(Object), false, true, false, false)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Collection Items"),
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.ObjectField(rect, GUIContent.none, elements[index], typeof(Object), true);
                    }
                }
            };
        }
    }
}