namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ClampedInt))]
    internal class ClampedIntDrawer : ClampedStructDrawer
    {
        protected override void DrawSlider(Rect valueRect)
        {
            EditorGUI.IntSlider(valueRect, ValueProperty, MinProperty.intValue, MaxProperty.intValue, GUIContent.none);
        }

        protected override void LimitMinValueIfNeeded()
        {
            int maxAllowedValue = MaxProperty.intValue - 1;

            if (MinProperty.intValue > maxAllowedValue)
                MinProperty.intValue = maxAllowedValue;
        }

        protected override void LimitMaxValueIfNeeded()
        {
            int minAllowedValue = MinProperty.intValue + 1;

            if (MaxProperty.intValue < minAllowedValue)
                MaxProperty.intValue = minAllowedValue;
        }
    }
}