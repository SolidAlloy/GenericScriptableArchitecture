namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(Reference), true)]
    public class ReferenceDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Object, Editor> _editorCache = new Dictionary<Object, Editor>();

        private SerializedProperty _useConstant;
        private SerializedProperty _constantValue;
        private SerializedProperty _variable;

        private bool UseConstant
        {
            get => _useConstant.boolValue;
            set => _useConstant.boolValue = value;
        }

        private Object Variable => _variable.objectReferenceValue;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            if ( ! UseConstant || ! _constantValue.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            // If a property has a custom property drawer, it will be drown inside a foldout anyway, so we account for
            // it by adding a single line height.
            float additionalHeight = _constantValue.HasCustomPropertyDrawer() ? EditorGUIUtility.singleLineHeight : 0f;
            return EditorGUI.GetPropertyHeight(_constantValue, GUIContent.none) + additionalHeight;
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

                UseConstant = ChoiceButton.DrawAndCheckConstant(buttonRect, UseConstant);
                DrawValue(property, valueRect, fieldRect, previousIndent);

                EditorGUI.indentLevel = previousIndent;
            }
        }

        private void FindProperties(SerializedProperty property)
        {
            _useConstant = property.FindPropertyRelative("_useConstant");
            _constantValue = property.FindPropertyRelative("_constantValue");
            _variable = property.FindPropertyRelative("_variable");
        }

        private void DrawValue(SerializedProperty property, Rect valueRect, Rect totalRect, int indentLevel)
        {
            if (UseConstant)
            {
                DrawConstantValue(valueRect, totalRect, indentLevel);
            }
            else
            {
                DrawVariableValue(valueRect, property, indentLevel);
            }
        }

        private void DrawConstantValue(Rect valueRect, Rect totalRect, int indentLevel)
        {
            if (_constantValue.propertyType == SerializedPropertyType.Generic)
            {
                DrawConstantValueInFoldout(totalRect, indentLevel);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, _constantValue, GUIContent.none);
            }
        }

        private void DrawConstantValueInFoldout(Rect totalRect, int indentLevel)
        {
            if ( ! _constantValue.isExpanded)
                return;

            const float paddingBetweenFields = 2f;
            const float indentPerLevel = 15f;

            totalRect.xMin += (indentLevel + 1) * indentPerLevel;
            totalRect.y += EditorGUIUtility.singleLineHeight + paddingBetweenFields;

            if (_constantValue.HasCustomPropertyDrawer())
            {
                float height = EditorGUI.GetPropertyHeight(_constantValue);
                totalRect.height = height;
                EditorGUI.PropertyField(totalRect, _constantValue, GUIContent.none);
                return;
            }

            // This draws all child fields of the _constantValue property with indent.
            SerializedProperty iterator = _constantValue.Copy();
            var nextProp = _constantValue.Copy();
            nextProp.NextVisible(false);

            while (iterator.NextVisible(true) && ! SerializedProperty.EqualContents(iterator, nextProp))
            {
                float height = EditorGUI.GetPropertyHeight(iterator, false);
                totalRect.height = height;
                EditorGUI.PropertyField(totalRect, iterator, true);
                totalRect.y += height + paddingBetweenFields;
            }
        }

        private void DrawVariableValue(Rect valueRect, SerializedProperty property, int indentLevel)
        {
            EditorGUI.PropertyField(valueRect, _variable, GUIContent.none);

            if ( ! property.isExpanded || Variable == null)
                return;

            using (new EditorDrawHelper.IndentLevel(indentLevel + 1))
            {
                GetInlineEditor(Variable).OnInspectorGUI();
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
            if (UseConstant || Variable == null)
            {
                if (_constantValue.propertyType == SerializedPropertyType.Generic)
                {
                    _constantValue.isExpanded = EditorGUI.Foldout(labelRect, _constantValue.isExpanded, label, true);
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

            private static readonly string[] _popupOptions = { "Use Constant", "Use Variable" };

            public static float Width => _buttonStyle.fixedWidth;

            [Pure]
            public static bool DrawAndCheckConstant(Rect buttonRect, bool constantIsUsed)
            {
                int result = EditorGUI.Popup(buttonRect, constantIsUsed ? 0 : 1, _popupOptions, _buttonStyle);
                bool useConstant = result == 0;
                return useConstant;
            }
        }
    }
}