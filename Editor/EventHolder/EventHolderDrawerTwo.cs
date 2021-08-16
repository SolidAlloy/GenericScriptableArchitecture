namespace GenericScriptableArchitecture.Editor
{
    using System;
    using System.Reflection;
    using SolidUtilities.UnityEditorInternals;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Assertions;

    [CustomPropertyDrawer(typeof(EventHolderBaseTwo), true)]
    public class EventHolderDrawerTwo : PropertyDrawer
    {
        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            var helper = new EventHolderDrawerUtil(property, 2);

            if (GenericArgsAreEqual(property))
            {
                helper.DrawButtonAndValue(fieldRect, label);
            }
            else
            {
                helper.DrawEvent(fieldRect, label);
            }
        }

        private bool GenericArgsAreEqual(SerializedProperty property)
        {
            (FieldInfo _, Type propertyType) = property.GetFieldInfoAndType();
            var genericArgs = propertyType.GenericTypeArguments;
            Assert.IsTrue(genericArgs.Length == 2);
            return genericArgs[0] == genericArgs[1];
        }
    }
}