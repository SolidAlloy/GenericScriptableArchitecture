namespace GenericScriptableArchitecture.Editor
{
    using SolidUtilities;
    using SolidUtilities.Editor;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;

    internal static class MultiValueDrawer
    {
        private static GUIStyle _buttonStyle;
        private static GUIStyle ButtonStyle => _buttonStyle ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
        {
            imagePosition = ImagePosition.ImageOnly
        };

        public static float GetPropertyHeight(SerializedProperty mainProperty, SerializedProperty chosenValueProperty)
        {
            if (!CanBeDrawnInFoldout(chosenValueProperty) || ! mainProperty.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            // If a property has a custom property drawer, it will be drown inside a foldout anyway, so we account for it by adding a single line height.
            float additionalHeight = chosenValueProperty.propertyType == SerializedPropertyType.Generic && chosenValueProperty.HasCustomPropertyDrawer() ? EditorGUIUtility.singleLineHeight : 0f;
            return EditorGUI.GetPropertyHeight(chosenValueProperty, GUIContent.none) + additionalHeight;
        }

        public static int OnGUI(SerializedProperty mainProperty, Rect fieldRect, GUIContent label, SerializedProperty chosenValueProperty, int chosenValueIndex, string[] popupValueNames)
        {
            (Rect labelRect, Rect buttonRect, Rect valueRect) = GetLabelButtonValueRects(fieldRect);
            DrawLabel(mainProperty, chosenValueProperty, fieldRect, labelRect, label);

            // The indent level must be made 0 for the button and value to be displayed normally, without any
            // additional indent. Otherwise, the button will not be clickable, and the value will look shifted
            // compared to other fields.
            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var newValueIndex = DrawButton(buttonRect, chosenValueIndex, popupValueNames);

            if (chosenValueProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                DrawObjectReference(valueRect, chosenValueProperty, previousIndent, mainProperty.isExpanded);
            }
            else if (chosenValueProperty.propertyType == SerializedPropertyType.Generic) // generic meaning here that it is a custom serializable class that is most probably multi-line.
            {
                DrawGenericProperty(mainProperty, chosenValueProperty, fieldRect, previousIndent);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, chosenValueProperty, GUIContent.none);
            }

            EditorGUI.indentLevel = previousIndent;
            return newValueIndex;
        }

        private static (Rect label, Rect button, Rect value) GetLabelButtonValueRects(Rect totalRect)
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

        private static void DrawLabel(SerializedProperty mainProperty, SerializedProperty chosenValueProperty, Rect totalRect, Rect labelRect, GUIContent label)
        {
            if (CanBeDrawnInFoldout(chosenValueProperty))
            {
                mainProperty.isExpanded = EditorGUI.Foldout(labelRect, mainProperty.isExpanded, label, true);
            }
            else
            {
                EditorGUI.HandlePrefixLabel(totalRect, labelRect, label);
            }
        }

        private static bool CanBeDrawnInFoldout(SerializedProperty property)
        {
            return PropertyHasInlineDrawer(property) || property.propertyType == SerializedPropertyType.Generic;
        }

        private static bool PropertyHasInlineDrawer(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue == null)
                return false;

            var inlineDrawer = InlineEditorCache.GetInlineDrawer(property.objectReferenceValue);

            if (inlineDrawer == null)
                return false;

            return inlineDrawer.HasContent;
        }

        private static int DrawButton(Rect buttonRect, int currentValue, string[] popupOptions)
        {
            return EditorGUI.Popup(buttonRect, currentValue, popupOptions, ButtonStyle);
        }

        private static void DrawObjectReference(Rect valueRect, SerializedProperty property, int indentLevel, bool isExpanded)
        {
            EditorGUI.PropertyField(valueRect, property, GUIContent.none);

            if ( ! isExpanded || property.objectReferenceValue == null)
                return;

            var inlineDrawer = InlineEditorCache.GetInlineDrawer(property.objectReferenceValue);

            if (inlineDrawer == null || !inlineDrawer.HasContent)
                return;

            using (EditorGUIHelper.IndentLevelBlock(indentLevel + 1))
            {
                inlineDrawer.OnInlineGUI();
            }
        }

        private static void DrawGenericProperty(SerializedProperty mainProperty, SerializedProperty valueProperty, Rect totalRect, int indentLevel)
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
    }
}