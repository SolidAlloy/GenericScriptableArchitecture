namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using GenericUnityObjects.Editor;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using Object = UnityEngine.Object;

    internal class FoldoutList<TElement> where TElement : Object
    {
        private static readonly MethodInfo _clearCacheMethod = typeof(ReorderableList)
            .GetMethod("ClearCache", BindingFlags.Instance | BindingFlags.NonPublic);

        private ReorderableList _list;
        private readonly Action _clearCache;
        private readonly SerializedProperty _expandedProperty;
        private readonly string _title;

        private Type _realElementType;

        public FoldoutList(List<TElement> elements, SerializedProperty expandedProperty, string title)
        {
            _expandedProperty = expandedProperty;
            _title = title;

            CreateReorderableList(elements);

            _clearCache = (Action) Delegate.CreateDelegate(typeof(Action), _list, _clearCacheMethod);
        }

        private void CreateReorderableList(List<TElement> elements)
        {
            _list = new ReorderableList(elements, typeof(TElement), false, true,
                false, false)
            {
                drawHeaderCallback = rect =>
                {
                    const float leftMargin = 10f;
                    var shiftedRight = new Rect(rect.x + leftMargin, rect.y, rect.width - leftMargin, rect.height);

                    bool newValue = EditorGUI.Foldout(shiftedRight, _expandedProperty.boolValue, _title, true);

                    if (_expandedProperty.boolValue == newValue)
                        return;

                    _expandedProperty.boolValue = newValue;
                    _clearCache();
                },
                drawElementCallback = (rect, index, _, __) =>
                {
                    if ( ! _expandedProperty.boolValue)
                        return;

                    var element = elements[index];

                    using (new EditorGUI.DisabledScope(true))
                        GenericObjectDrawer.ObjectField(rect, null, element, GetRealElementType(element), true);
                },
                elementHeightCallback = indexer => _expandedProperty.boolValue ? EditorGUIUtility.singleLineHeight : 0f,
            };
        }

        public void DoLayoutList()
        {
            _list.DoLayoutList();
        }

        public void Update(List<TElement> newList)
        {
            if (!_expandedProperty.boolValue)
                return;

            CreateReorderableList(newList);
        }

        private Type GetRealElementType(TElement element)
        {
            if (_realElementType == null)
            {
                _realElementType = element.GetType().BaseType;
            }

            return _realElementType;
        }
    }
}