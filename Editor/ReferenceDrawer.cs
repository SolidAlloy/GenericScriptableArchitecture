namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using SolidUtilities;
    using SolidUtilities.Editor;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(ReferenceBase), true)]
    internal class ReferenceDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Object, Editor> _editorCache = new Dictionary<Object, Editor>();
        
        private static GUIStyle _buttonStyle;
        private static GUIStyle ButtonStyle => _buttonStyle ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
        {
            imagePosition = ImagePosition.ImageOnly
        };

        private SerializedProperty _mainProperty;
        private SerializedProperty _valueType;
        private SerializedProperty _value;
        private SerializedProperty _variable;
        private SerializedProperty _constant;

        private int PopupValue
        {
            get => _valueType.enumValueIndex;
            set => _valueType.enumValueIndex = value;
        }

        private SerializedProperty ExposedProperty
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

        private static readonly string[] _popupOptions = { "Value", "Constant", "Variable" };

        private ReferenceBase.ValueTypes ValueType => (ReferenceBase.ValueTypes) PopupValue;

        private Object ObjectReference => ExposedProperty.objectReferenceValue;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            if (ValueType != ReferenceBase.ValueTypes.Value || ! property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            // If a property has a custom property drawer, it will be drown inside a foldout anyway, so we account for
            // it by adding a single line height.
            float additionalHeight = ExposedProperty.HasCustomPropertyDrawer() ? EditorGUIUtility.singleLineHeight : 0f;
            return EditorGUI.GetPropertyHeight(ExposedProperty, GUIContent.none) + additionalHeight;
        }

        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);
            (Rect labelRect, Rect buttonRect, Rect valueRect) = GetLabelButtonValueRects(fieldRect);
            DrawLabel(property, fieldRect, labelRect, label);

            // The indent level must be made 0 for the button and value to be displayed normally, without any
            // additional indent. Otherwise, the button will not be clickable, and the value will look shifted
            // compared to other fields.
            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            PopupValue = DrawButton(buttonRect, PopupValue);
            DrawValue(property, valueRect, fieldRect, previousIndent);
            EditorGUI.indentLevel = previousIndent;
        }

        private void DrawValue(SerializedProperty property, Rect valueRect, Rect totalRect, int indentLevel)
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
        
        private void DrawValueProperty(SerializedProperty mainProperty, Rect valueRect, Rect totalRect, int indentLevel)
        {
            if (ExposedProperty.propertyType == SerializedPropertyType.Generic)
            {
                DrawValueInFoldout(mainProperty, ExposedProperty, totalRect, indentLevel);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, ExposedProperty, GUIContent.none);
            }
        }

        private void DrawLabel(SerializedProperty property, Rect totalRect, Rect labelRect, GUIContent label)
        {
            if (ValueType != ReferenceBase.ValueTypes.Value && ObjectReference != null || _value.propertyType == SerializedPropertyType.Generic)
            {
                property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
            }
            else
            {
                EditorGUI.HandlePrefixLabel(totalRect, labelRect, label);
            }
        }

        private (Rect label, Rect button, Rect value) GetLabelButtonValueRects(Rect totalRect)
        {
            const float indentWidth = 15f;
            const float valueLeftIndent = 2f;

            totalRect.height = EditorGUIUtility.singleLineHeight;

            (Rect labelAndButtonRect, Rect valueRect) = totalRect.CutVertically(EditorGUIUtility.labelWidth);

            labelAndButtonRect.xMin += EditorGUI.indentLevel * indentWidth;

            (Rect labelRect, Rect buttonRect) =
                labelAndButtonRect.CutVertically(ButtonStyle.fixedWidth, fromRightSide: true);

            valueRect.xMin += valueLeftIndent;
            return (labelRect, buttonRect, valueRect);
        }

        private static void DrawValueInFoldout(SerializedProperty mainProperty, SerializedProperty valueProperty, Rect totalRect, int indentLevel)
        {
            valueProperty.isExpanded = mainProperty.isExpanded;

            if ( ! mainProperty.isExpanded)
                return;

            var shiftedRect = totalRect.ShiftOneLineDown(indentLevel + 1);

            if (valueProperty.HasCustomPropertyDrawer())
            {
                shiftedRect.height = EditorGUI.GetPropertyHeight(valueProperty);
                EditorGUI.PropertyField(shiftedRect, valueProperty, GUIContent.none);
                return;
            }

            // This draws all child fields of the _constantValue property with indent.
            SerializedProperty iterator = valueProperty.Copy();
            var nextProp = valueProperty.Copy();
            nextProp.NextVisible(false);

            while (iterator.NextVisible(true) && ! SerializedProperty.EqualContents(iterator, nextProp))
            {
                shiftedRect.height = EditorGUI.GetPropertyHeight(iterator, false);
                EditorGUI.PropertyField(shiftedRect, iterator, true);
                shiftedRect = shiftedRect.ShiftOneLineDown(lineHeight: shiftedRect.height);
            }
        }

        private int DrawButton(Rect buttonRect, int currentValue)
        {
            return EditorGUI.Popup(buttonRect, currentValue, _popupOptions, ButtonStyle);
        }
    }
}