namespace GenericScriptableArchitecture.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(EventHolderBaseOne), true)]
    public class EventHolderDrawerOne : PropertyDrawer
    {
        public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
        {
            var helper = new EventHolderDrawerUtil(property);
            helper.DrawButtonAndValue(fieldRect, label);
        }
    }
}