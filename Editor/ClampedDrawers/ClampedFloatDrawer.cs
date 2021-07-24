namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ClampedFloat))]
    internal class ClampedFloatDrawer : ClampedStructDrawer
    {
        private const float InspectorMinFloat = 0.00001f;

        protected override void DrawSlider(Rect valueRect)
        {
            EditorGUI.Slider(valueRect, ValueProperty, MinProperty.floatValue, MaxProperty.floatValue, GUIContent.none);
        }

        protected override void LimitMinValueIfNeeded()
        {
            float maxAllowedValue = MaxProperty.floatValue - InspectorMinFloat;

            if (MinProperty.floatValue > maxAllowedValue)
                MinProperty.floatValue = maxAllowedValue;
        }

        protected override void LimitMaxValueIfNeeded()
        {
            float minAllowedValue = MinProperty.floatValue + InspectorMinFloat;

            if (MaxProperty.floatValue < minAllowedValue)
                MaxProperty.floatValue = minAllowedValue;
        }
    }
}