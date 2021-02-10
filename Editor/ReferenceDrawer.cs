namespace GenericScriptableArchitecture.Editor
{
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    [CustomPropertyDrawer(typeof(Reference), true)]
    public class ReferenceDrawer : PropertyDrawer
    {
        private SerializedProperty _useConstant;
        private SerializedProperty _constantValue;
        private SerializedProperty _variable;

        private static readonly Dictionary<Object, Editor> _editorCache = new Dictionary<Object, Editor>();

        private bool UseConstant
        {
            get => _useConstant.boolValue;
            set => _useConstant.boolValue = value;
        }

        private Object Variable => _variable.objectReferenceValue;

        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            using (new EditorDrawHelper.PropertyWrapper(fieldRect, label, property))
            {
                (Rect labelRect, Rect buttonRect, Rect valueRect) = GetLabelButtonValueRects(fieldRect);

                DrawLabel(property, fieldRect, labelRect, label);

                // The indent level must be made 0 for button and value to be displayed normally, without any
                // additional indent. Otherwise, the button will not be clickable, and the value will look shifted
                // compared to other fields.
                int previousIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                UseConstant = ChoiceButton.DrawAndCheckConstant(buttonRect, UseConstant);
                DrawValue(property, valueRect, previousIndent);

                EditorGUI.indentLevel = previousIndent;
            }
        }

        private void FindProperties(SerializedProperty property)
        {
            _useConstant = property.FindPropertyRelative("_useConstant");
            Assert.IsNotNull(_useConstant);

            _constantValue = property.FindPropertyRelative("_constantValue");
            Assert.IsNotNull(_constantValue);

            _variable = property.FindPropertyRelative("_variable");
            Assert.IsNotNull(_variable);
        }

        private void DrawValue(SerializedProperty property, Rect valueRect, int indentLevel)
        {
            if (UseConstant)
            {
                EditorGUI.PropertyField(valueRect, _constantValue, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, _variable, GUIContent.none);

                if ( ! property.isExpanded || Variable == null)
                    return;

                using (new EditorDrawHelper.IndentLevel(indentLevel + 2))
                {
                    GetInlineEditor(Variable).OnInspectorGUI();
                }
            }
        }

        private Editor GetInlineEditor(Object variable)
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
                EditorGUI.HandlePrefixLabel(totalRect, labelRect, label);
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