namespace GenericScriptableArchitecture.Editor
{
    using GenericUnityObjects.UnityEditorInternals;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(RuntimeSetBase), true)]
    public class RuntimeSetEditor : GenericHeaderEditor
    {
        private SerializedProperty _onItemAdded;
        private SerializedProperty _onItemRemoved;

        private ReorderableList _listDrawer;

        private void OnEnable()
        {
            _onItemAdded = serializedObject.FindProperty(nameof(RuntimeSet<Object>._onItemAdded));
            _onItemRemoved = serializedObject.FindProperty(nameof(RuntimeSet<Object>._onItemRemoved));
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
            const float spacingBetweenElements = 4f;
            var elements = ((RuntimeSetBase) target).List;

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
                elementHeightCallback = index =>
                {
                    return EditorGUIUtility.singleLineHeight +
                           (index == elements.Count - 1 ? 0f : spacingBetweenElements);
                }
            };
        }
    }
}