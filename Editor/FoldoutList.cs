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

        private readonly ReorderableList _list;
        private readonly Action _clearCache;

        private Type _realElementType;

        public FoldoutList(List<TElement> elements, SerializedProperty expandedProperty)
        {
            _list = new ReorderableList(elements, typeof(TElement), false, true,
                false, false)
            {
                drawHeaderCallback = rect =>
                {
                    const float leftMargin = 10f;
                    var shiftedRight = new Rect(rect.x + leftMargin, rect.y, rect.width - leftMargin, rect.height);
                    bool newValue = EditorGUI.Foldout(shiftedRight, expandedProperty.boolValue, "Listeners", true);

                    if (expandedProperty.boolValue == newValue)
                        return;

                    expandedProperty.boolValue = newValue;
                    _clearCache();
                },
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    if ( ! expandedProperty.boolValue)
                    {
                        GUI.enabled = index == elements.Count;
                        return;
                    }

                    var element = elements[index];

                    using (new EditorGUI.DisabledScope(true))
                        GenericObjectDrawer.ObjectField(rect, null, element, GetRealElementType(element), true);
                },
                elementHeightCallback = indexer => expandedProperty.boolValue ? EditorGUIUtility.singleLineHeight : 0f,
            };

            _clearCache = (Action) Delegate.CreateDelegate(typeof(Action), _list, _clearCacheMethod);
        }

        public void DoLayoutList()
        {
            _list.DoLayoutList();
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