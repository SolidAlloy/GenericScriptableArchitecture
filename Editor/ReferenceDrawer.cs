namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(ReferenceBase), true)]
    internal class ReferenceDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Object, Editor> _editorCache = new Dictionary<Object, Editor>();

        private SerializedProperty _valueType;
        private SerializedProperty _value;
        private SerializedProperty _variable;
        private SerializedProperty _constant;

        private ReferenceBase.ValueTypes ValueType
        {
            get => (ReferenceBase.ValueTypes) _valueType.enumValueIndex;
            set => _valueType.enumValueIndex = (int) value;
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

        private Object ObjectReference => ExposedProperty.objectReferenceValue;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            if ( ValueType != ReferenceBase.ValueTypes.Value || ! _value.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            // If a property has a custom property drawer, it will be drown inside a foldout anyway, so we account for
            // it by adding a single line height.

            float additionalHeight = ExposedProperty.HasCustomPropertyDrawer() ? EditorGUIUtility.singleLineHeight : 0f;
            return EditorGUI.GetPropertyHeight(ExposedProperty, GUIContent.none) + additionalHeight;
        }

        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            using (new EditorDrawHelper.PropertyWrapper(fieldRect, label, property))
            {
                (Rect labelRect, Rect buttonRect, Rect valueRect) = GetLabelButtonValueRects(fieldRect);

                DrawLabel(property, fieldRect, labelRect, label);

                // The indent level must be made 0 for the button and value to be displayed normally, without any
                // additional indent. Otherwise, the button will not be clickable, and the value will look shifted
                // compared to other fields.
                int previousIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                ValueType = ChoiceButton.DrawAndCheckConstant(buttonRect, ValueType);
                DrawValue(property, valueRect, fieldRect, previousIndent);

                EditorGUI.indentLevel = previousIndent;
            }
        }

        private void FindProperties(SerializedProperty property)
        {
            _valueType = property.FindPropertyRelative("ValueType");
            _value = property.FindPropertyRelative("_value");
            _variable = property.FindPropertyRelative("_variable");
            _constant = property.FindPropertyRelative("_constant");
        }

        private void DrawValue(SerializedProperty property, Rect valueRect, Rect totalRect, int indentLevel)
        {
            switch (ValueType)
            {
                case ReferenceBase.ValueTypes.Constant:
                    DrawConstant(valueRect, property, indentLevel);
                    break;

                case ReferenceBase.ValueTypes.Value:
                    DrawValue(valueRect, totalRect, indentLevel);
                    break;

                case ReferenceBase.ValueTypes.Variable:
                    DrawVariable(valueRect, property, indentLevel);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ExposedProperty),
                        "Unknown value type in the reference.");
            }
        }

        private void DrawValue(Rect valueRect, Rect totalRect, int indentLevel)
        {
            if (_value.propertyType == SerializedPropertyType.Generic)
            {
                DrawValueInFoldout(totalRect, indentLevel);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, _value, GUIContent.none);
            }
        }

        private void DrawValueInFoldout(Rect totalRect, int indentLevel)
        {
            if ( ! _value.isExpanded)
                return;

            const float paddingBetweenFields = 2f;
            const float indentPerLevel = 15f;

            totalRect.xMin += (indentLevel + 1) * indentPerLevel;
            totalRect.y += EditorGUIUtility.singleLineHeight + paddingBetweenFields;

            if (_value.HasCustomPropertyDrawer())
            {
                float height = EditorGUI.GetPropertyHeight(_value);
                totalRect.height = height;
                EditorGUI.PropertyField(totalRect, _value, GUIContent.none);
                return;
            }

            // This draws all child fields of the _constantValue property with indent.
            SerializedProperty iterator = _value.Copy();
            var nextProp = _value.Copy();
            nextProp.NextVisible(false);

            while (iterator.NextVisible(true) && ! SerializedProperty.EqualContents(iterator, nextProp))
            {
                float height = EditorGUI.GetPropertyHeight(iterator, false);
                totalRect.height = height;
                EditorGUI.PropertyField(totalRect, iterator, true);
                totalRect.y += height + paddingBetweenFields;
            }
        }

        private void DrawVariable(Rect valueRect, SerializedProperty property, int indentLevel)
        {
            EditorGUI.PropertyField(valueRect, _variable, GUIContent.none);

            if ( ! property.isExpanded || ObjectReference == null)
                return;

            using (new EditorDrawHelper.IndentLevel(indentLevel + 1))
            {
                GetInlineEditor(ObjectReference).OnInspectorGUI();
            }
        }

        private void DrawConstant(Rect valueRect, SerializedProperty property, int indentLevel)
        {
            EditorGUI.PropertyField(valueRect, _variable, GUIContent.none);

            if ( ! property.isExpanded || ObjectReference == null)
                return;

            using (new EditorDrawHelper.IndentLevel(indentLevel + 1))
            {
                // GetInlineEditor(Variable).OnInspectorGUI();
                // TODO: create custom constant editor
                // perhaps unify with DrawVariable, if GetInlineEditor can be agnostic.
            }
        }

        private static Editor GetInlineEditor(Object variable)
        {
            if ( ! _editorCache.TryGetValue(variable, out Editor editor))
            {
                editor = EditorDrawHelper.CreateEditor<VariableInlineEditor>(variable);
                _editorCache.Add(variable, editor);
            }

            return editor;
        }

        private (Rect label, Rect button, Rect value) GetLabelButtonValueRects(Rect totalRect)
        {
            const float indentWidth = 15f;
            const float valueLeftIndent = 2f;

            totalRect.height = EditorGUIUtility.singleLineHeight;

            (Rect labelAndButtonRect, Rect valueRect) = totalRect.CutVertically(EditorGUIUtility.labelWidth);

            labelAndButtonRect.xMin += EditorGUI.indentLevel * indentWidth;

            float buttonWidth = ChoiceButton.Width;

            (Rect labelRect, Rect buttonRect) =
                labelAndButtonRect.CutVertically(buttonWidth, fromRightBorder: true);

            valueRect.xMin += valueLeftIndent;
            return (labelRect, buttonRect, valueRect);
        }

        private void DrawLabel(SerializedProperty property, Rect totalRect, Rect labelRect, GUIContent label)
        {
            if (ValueType == ReferenceBase.ValueTypes.Value || ObjectReference == null)
            {
                if (_value.propertyType == SerializedPropertyType.Generic)
                {
                    _value.isExpanded = EditorGUI.Foldout(labelRect, _value.isExpanded, label, true);
                }
                else
                {
                    EditorGUI.HandlePrefixLabel(totalRect, labelRect, label);
                }
            }
            else
            {
                property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
            }
        }

        private static class ChoiceButton
        {
            private static readonly GUIStyle _buttonStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };

            private static readonly string[] _popupOptions = { "Value", "Constant", "Variable" };

            public static float Width => _buttonStyle.fixedWidth;

            [Pure]
            public static ReferenceBase.ValueTypes DrawAndCheckConstant(Rect buttonRect, ReferenceBase.ValueTypes currentType)
            {
                int result = EditorGUI.Popup(buttonRect, (int) currentType, _popupOptions, _buttonStyle);
                return (ReferenceBase.ValueTypes) result;
            }
        }
    }
}