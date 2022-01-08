namespace GenericScriptableArchitecture.Editor
{
    using SolidUtilities.Editor;
    using SolidUtilities;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;

    internal abstract class ClampedStructDrawer : PropertyDrawer
    {
        protected SerializedProperty ValueProperty;
        protected SerializedProperty MinProperty;
        protected SerializedProperty MaxProperty;

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

        protected abstract void DrawSlider(Rect valueRect);

        protected abstract void LimitMinValueIfNeeded();

        protected abstract void LimitMaxValueIfNeeded();

        private void InitializeProperties(SerializedProperty mainProperty)
        {
            ValueProperty = mainProperty.FindPropertyRelative("_value");
            MinProperty = mainProperty.FindPropertyRelative("_min");
            MaxProperty = mainProperty.FindPropertyRelative("_max");
        }

        private void DrawFoldoutSlider(Rect rect, SerializedProperty property, GUIContent label)
        {
            var firstLineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            float labelWidth = label == GUIContent.none ? 5f : EditorGUIUtility.labelWidth;
            using var _ = EditorGUIUtilityHelper.LabelWidthBlock(labelWidth);

            var labelRect = GetLabelRect(firstLineRect);
            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            var valueRect = GetRectWithoutLabel(firstLineRect);

            DrawSlider(valueRect);
        }

        private void DrawMinMax(Rect totalRect)
        {
            var secondLineRect = totalRect.ShiftOneLineDown();
            secondLineRect.height = EditorGUIUtility.singleLineHeight;

            (Rect left, Rect right) = secondLineRect.CutVertically(secondLineRect.width / 2f);
            AddPadding(ref left, ref right, 20f);

            using var _ = EditorGUIUtilityHelper.LabelWidthBlock(50f);
            DrawAndLimitMinValue(left);
            DrawAndLimitMaxValue(right);
        }

        private void DrawAndLimitMinValue(Rect rect)
        {
            using var changeCheck = new EditorGUI.ChangeCheckScope();

            EditorGUI.PropertyField(rect, MinProperty);

            if ( ! changeCheck.changed)
                return;

            LimitMinValueIfNeeded();
        }

        private void DrawAndLimitMaxValue(Rect rect)
        {
            using var changeCheck = new EditorGUI.ChangeCheckScope();

            EditorGUI.PropertyField(rect, MaxProperty);

            if ( ! changeCheck.changed)
                return;

            LimitMaxValueIfNeeded();
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