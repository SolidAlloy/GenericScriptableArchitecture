namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using SolidUtilities.Editor;
    using SolidUtilities.Editor.Helpers;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(ReferenceBase), true)]
    internal class ReferenceDrawer : DrawerWithModes
    {
        private static readonly Dictionary<Object, Editor> _editorCache = new Dictionary<Object, Editor>();

        private SerializedProperty _mainProperty;
        private SerializedProperty _valueType;
        private SerializedProperty _value;
        private SerializedProperty _variable;
        private SerializedProperty _constant;

        protected override int PopupValue
        {
            get => _valueType.enumValueIndex;
            set => _valueType.enumValueIndex = value;
        }

        protected override SerializedProperty ExposedProperty
        {
            get
            {
                return ValueType switch
                {
                    ReferenceBase.ValueTypes.Constant => _constant,
                    ReferenceBase.ValueTypes.Value => _value,
                    ReferenceBase.ValueTypes.Variable => _variable,
                    _ => throw new ArgumentOutOfRangeException(nameof(ExposedProperty), "Unknown value type in the reference.")
                };
            }
        }

        protected override bool ShouldDrawFoldout =>
            ValueType != ReferenceBase.ValueTypes.Value && ObjectReference != null || _value.propertyType == SerializedPropertyType.Generic;

        private static readonly string[] _popupOptions = { "Value", "Constant", "Variable" };
        protected override string[] PopupOptions => _popupOptions;

        private ReferenceBase.ValueTypes ValueType => (ReferenceBase.ValueTypes) PopupValue;

        private Object ObjectReference => ExposedProperty.objectReferenceValue;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            if ( ValueType != ReferenceBase.ValueTypes.Value)
                return EditorGUIUtility.singleLineHeight;

            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);
            base.OnGUI(fieldRect, property, label);
        }

        protected override void DrawValue(SerializedProperty property, Rect valueRect, Rect totalRect, int indentLevel)
        {
            switch (ValueType)
            {
                case ReferenceBase.ValueTypes.Constant:
                case ReferenceBase.ValueTypes.Variable:
                    DrawObjectReference(valueRect, property, indentLevel);
                    break;

                case ReferenceBase.ValueTypes.Value:
                    DrawValueProperty(property, valueRect, totalRect, indentLevel);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ExposedProperty), "Unknown value type in the reference.");
            }
        }

        private static Editor GetInlineEditor(Object variable)
        {
            if ( ! _editorCache.TryGetValue(variable, out Editor editor))
            {
                editor = EditorHelper.CreateEditor<VariableInlineEditor>(variable);
                _editorCache.Add(variable, editor);
            }

            return editor;
        }

        private void FindProperties(SerializedProperty property)
        {
            _mainProperty = property;
            _valueType = _mainProperty.FindPropertyRelative("ValueType");
            _value = _mainProperty.FindPropertyRelative("_value");
            _variable = _mainProperty.FindPropertyRelative("_variable");
            _constant = _mainProperty.FindPropertyRelative("_constant");
        }

        private void DrawObjectReference(Rect valueRect, SerializedProperty property, int indentLevel)
        {
            EditorGUI.PropertyField(valueRect, ExposedProperty, GUIContent.none);

            if ( ! property.isExpanded || ObjectReference == null)
                return;

            using (EditorGUIHelper.IndentLevelBlock(indentLevel + 1))
            {
                GetInlineEditor(ObjectReference).OnInspectorGUI();
            }
        }
    }
}