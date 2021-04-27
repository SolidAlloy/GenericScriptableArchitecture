namespace GenericScriptableArchitecture.Editor
{
    using SolidUtilities.Editor.Extensions;
    using SolidUtilities.Editor.Helpers;
    using SolidUtilities.Extensions;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;

    internal abstract class ClampedStructDrawer : PropertyDrawer, IDelayable
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
            OnGUI(rect, property, label, false);
        }

        public void OnGUIDelayed(Rect rect, SerializedProperty property, GUIContent label)
        {
            OnGUI(rect, property, label, true);
        }

        protected abstract void DrawSlider(Rect valueRect, bool delayed);

        protected abstract void LimitMinValueIfNeeded();

        protected abstract void LimitMaxValueIfNeeded();

        private void OnGUI(Rect rect, SerializedProperty property, GUIContent label, bool delayed)
        {
            InitializeProperties(property);
            DrawFoldoutSlider(rect, property, label, delayed);

            if (property.isExpanded)
                DrawMinMax(rect, delayed);
        }

        private void InitializeProperties(SerializedProperty mainProperty)
        {
            ValueProperty = mainProperty.FindPropertyRelative("_value");
            MinProperty = mainProperty.FindPropertyRelative("_min");
            MaxProperty = mainProperty.FindPropertyRelative("_max");
        }

        private void DrawFoldoutSlider(Rect rect, SerializedProperty property, GUIContent label, bool delayed)
        {
            var firstLineRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

            float labelWidth = label == GUIContent.none ? 5f : EditorGUIUtility.labelWidth;
            using var _ = new EditorDrawHelper.LabelWidth(labelWidth);

            var labelRect = GetLabelRect(firstLineRect);
            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            var valueRect = GetRectWithoutLabel(firstLineRect);

            DrawSlider(valueRect, delayed);
        }

        private void DrawMinMax(Rect totalRect, bool delayed)
        {
            var secondLineRect = totalRect.ShiftOneLineDown();
            secondLineRect.height = EditorGUIUtility.singleLineHeight;

            (Rect left, Rect right) = secondLineRect.CutVertically(secondLineRect.width / 2f);
            AddPadding(ref left, ref right, 20f);

            using var _ = new EditorDrawHelper.LabelWidth(50f);
            DrawAndLimitMinValue(left, delayed);
            DrawAndLimitMaxValue(right, delayed);
        }

        private void DrawAndLimitMinValue(Rect rect, bool delayed)
        {
            using var changeCheck = new EditorGUI.ChangeCheckScope();

            DrawPropertyField(rect, MinProperty, delayed);

            if ( ! changeCheck.changed)
                return;

            LimitMinValueIfNeeded();
        }

        private void DrawAndLimitMaxValue(Rect rect, bool delayed)
        {
            using var changeCheck = new EditorGUI.ChangeCheckScope();

            DrawPropertyField(rect, MaxProperty, delayed);

            if ( ! changeCheck.changed)
                return;

            LimitMaxValueIfNeeded();
        }

        private static void DrawPropertyField(Rect rect, SerializedProperty property, bool delayed)
        {
            if (delayed)
            {
                EditorDrawHelper.DelayedPropertyField(rect, property);
            }
            else
            {
                EditorGUI.PropertyField(rect, property);
            }
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