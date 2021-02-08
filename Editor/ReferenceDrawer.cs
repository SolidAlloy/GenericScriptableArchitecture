namespace ExtendedScriptableObjects.Editor
{
    using JetBrains.Annotations;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    [CustomPropertyDrawer(typeof(Reference), true)]
    public class ReferenceDrawer : PropertyDrawer
    {
        private const string UseConstantName = "_useConstant";
        private const string ConstantValueName = "_constantValue";
        private const string VariableName = "_variable";

        private SerializedProperty _useConstantProp;
        private SerializedProperty _constantValueProp;
        private SerializedProperty _variableProp;

        private bool UseConstant
        {
            get => _useConstantProp.boolValue;
            set => _useConstantProp.boolValue = value;
        }

        private Object Variable => _variableProp.objectReferenceValue;

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

                if (property.serializedObject.hasModifiedProperties)
                    property.serializedObject.ApplyModifiedProperties();
            }
        }

        private void FindProperties(SerializedProperty property)
        {
            _useConstantProp = property.FindPropertyRelative(UseConstantName);
            Assert.IsNotNull(_useConstantProp);

            _constantValueProp = property.FindPropertyRelative(ConstantValueName);
            Assert.IsNotNull(_constantValueProp);

            _variableProp = property.FindPropertyRelative(VariableName);
            Assert.IsNotNull(_variableProp);
        }

        private void DrawValue(SerializedProperty property, Rect valueRect, int indentLevel)
        {
            if (UseConstant)
            {
                EditorGUI.PropertyField(valueRect, _constantValueProp, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, _variableProp, GUIContent.none);

                if ( ! property.isExpanded || Variable == null)
                    return;

                var editor = EditorDrawHelper.CreateEditor<PlayModeUnchangedEditor>(Variable);
                editor.ShowDescription = false;

                using (new EditorDrawHelper.IndentLevel(indentLevel + 2))
                {
                    editor.OnInspectorGUI();
                }
            }
        }

        private (Rect, Rect, Rect) GetLabelButtonValueRects(Rect totalRect)
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