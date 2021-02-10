namespace GenericScriptableArchitecture.Editor
{
    using System.Linq;
    using SolidUtilities.Editor.Extensions;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(GameEventBase), true)]
    public class GameEventDrawer : PropertyDrawer
    {
        private const float TopBottomMargin = 2f;
        private const string ListFieldName = nameof(GameEvent<bool>.ScriptableEvents);

        private ReorderableList _list;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetList(property).GetHeight();
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            ReorderableList list = GetList(property);

            if (rect == default)
            {
                list.DoLayoutList();
            }
            else
            {
                list.DoList(rect);
            }

            SerializedObject parentObject = property.serializedObject;

            if (parentObject.hasModifiedProperties)
                parentObject.ApplyModifiedProperties();
        }

        private ReorderableList GetList(SerializedProperty parentProperty)
        {
            if (_list != null)
                return _list;

            SerializedProperty listProperty = parentProperty.FindPropertyRelative(ListFieldName);

            var typeNameList = parentProperty.GetObjectType()
                .GetGenericArguments()
                .Select(type => type.Name);

            string typeNames = string.Join(", ", typeNameList);

            _list = new ReorderableList(listProperty.serializedObject, listProperty, false, true, true, true)
            {
                drawHeaderCallback = rect =>
                    EditorGUI.LabelField(rect, $"{parentProperty.displayName} ({typeNames})"),

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = listProperty.GetArrayElementAtIndex(index);
                    rect = new Rect(rect.x, rect.y + TopBottomMargin, rect.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rect, element, GUIContent.none, true);
                },

                elementHeightCallback = index =>
                    EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(index)) + TopBottomMargin * 2f
            };

            return _list;
        }
    }
}