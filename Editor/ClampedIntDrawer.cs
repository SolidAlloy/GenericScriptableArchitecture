namespace GenericScriptableArchitecture.Editor
{
    using SolidUtilities.Editor.Extensions;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ClampedInt))]
    public class ClampedIntDrawer : PropertyDrawer
    {
        private SerializedProperty _valueProperty;
        private SerializedProperty _minProperty;
        private SerializedProperty _maxProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if ( ! property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            const float paddingBetweenLines = 2f;
            return EditorGUIUtility.singleLineHeight * 2f + paddingBetweenLines;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            InitializeProperties(property);
            DrawFoldoutSlider(rect, property, label);

            if (property.isExpanded)
                DrawMinMax(rect);
        }

        private void InitializeProperties(SerializedProperty mainProperty)
        {
            _valueProperty = mainProperty.FindPropertyRelative("_value");
            _minProperty = mainProperty.FindPropertyRelative("_min");
            _maxProperty = mainProperty.FindPropertyRelative("_max");
        }

        private void DrawFoldoutSlider(Rect rect, SerializedProperty property, GUIContent label)
        {
            var firstLineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            float labelWidth = label == GUIContent.none ? 5f : EditorGUIUtility.labelWidth;
            using var _ = new EditorDrawHelper.LabelWidth(labelWidth);

            var labelRect = GetLabelRect(firstLineRect);
            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            // Return a very long rect if label is guicontent.none
            var valueRect = GetRectWithoutLabel(firstLineRect);
            EditorGUI.IntSlider(valueRect, _valueProperty, _minProperty.intValue, _maxProperty.intValue, GUIContent.none);
        }

        private void DrawMinMax(Rect totalRect)
        {
            var secondLineRect = totalRect.ShiftLinesDown(1);
            secondLineRect.height = EditorGUIUtility.singleLineHeight;

            (Rect left, Rect right) = secondLineRect.CutVertically(secondLineRect.width / 2f);
            AddPadding(ref left, ref right, 20f);

            using var _ = new EditorDrawHelper.LabelWidth(50f);
            EditorGUI.PropertyField(left, _minProperty);
            EditorGUI.PropertyField(right, _maxProperty);
        }

        private void AddPadding(ref Rect left, ref Rect right, float padding)
        {
            left.xMax -= padding / 2f;
            right.xMin += padding / 2f;
        }

        private Rect GetLabelRect(Rect rect)
        {
            return new Rect(
                rect.x + EditorGUI.indentLevel * 15f,
                rect.y,
                EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15f,
                rect.height);
        }

        private static Rect GetRectWithoutLabel(Rect totalPosition)
        {
            const float prefixPaddingRight = 2f;

            return new Rect(
                totalPosition.x + EditorGUIUtility.labelWidth + prefixPaddingRight,
                totalPosition.y,
                totalPosition.width - EditorGUIUtility.labelWidth - prefixPaddingRight,
                totalPosition.height);
        }
    }
}