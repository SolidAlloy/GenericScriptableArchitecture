namespace GenericScriptableArchitecture.Editor
{
    using System;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(BaseEventReference), true)]
    internal class EventReferenceDrawer : PropertyDrawer
    {
        private static readonly string[] _popupOptions = { "Scriptable Event", "Event Instancer" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumProperty = property.FindPropertyRelative(nameof(EventReference._eventType));
            var chosenValueProperty = GetChosenValueProperty(property, enumProperty.enumValueIndex);
            return MultiValueDrawer.GetPropertyHeight(property, chosenValueProperty);
        }

        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            var enumProperty = property.FindPropertyRelative(nameof(EventReference._eventType));
            var chosenValueProperty = GetChosenValueProperty(property, enumProperty.enumValueIndex);
            enumProperty.enumValueIndex = MultiValueDrawer.OnGUI(property, fieldRect, label, chosenValueProperty, enumProperty.enumValueIndex, _popupOptions);
        }

        private SerializedProperty GetChosenValueProperty(SerializedProperty mainProperty, int enumValueIndex)
        {
            var eventType = (BaseEventReference.EventType) enumValueIndex;

            return eventType switch
            {
                BaseEventReference.EventType.ScriptableEvent => mainProperty.FindPropertyRelative(nameof(EventReference._event)),
                BaseEventReference.EventType.EventInstancer => mainProperty.FindPropertyRelative(nameof(EventReference._eventInstancer)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    [CustomPropertyDrawer(typeof(BaseReference), true)]
    internal class ReferenceDrawer : PropertyDrawer
    {
        private static readonly string[] _popupOptions = { "Value", "Constant", "Variable", "Variable Instancer" };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var enumProperty = property.FindPropertyRelative(nameof(BaseReference._valueType));
            var chosenValueProperty = GetChosenValueProperty(property, enumProperty.enumValueIndex);
            return MultiValueDrawer.GetPropertyHeight(property, chosenValueProperty);
        }

        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            var enumProperty = property.FindPropertyRelative(nameof(BaseReference._valueType));
            var chosenValueProperty = GetChosenValueProperty(property, enumProperty.enumValueIndex);
            enumProperty.enumValueIndex = MultiValueDrawer.OnGUI(property, fieldRect, label, chosenValueProperty, enumProperty.enumValueIndex, _popupOptions);
        }

        private static SerializedProperty GetChosenValueProperty(SerializedProperty mainProperty, int enumIndex)
        {
            var valueType = (BaseReference.ValueTypes) enumIndex;

            return valueType switch
            {
                BaseReference.ValueTypes.Value => mainProperty.FindPropertyRelative(nameof(Reference<int>._value)),
                BaseReference.ValueTypes.Constant => mainProperty.FindPropertyRelative(nameof(Reference<int>._constant)),
                BaseReference.ValueTypes.Variable => mainProperty.FindPropertyRelative(nameof(Reference<int>._variable)),
                BaseReference.ValueTypes.VariableInstancer => mainProperty.FindPropertyRelative(nameof(Reference<int>._variableInstancer)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}