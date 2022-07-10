namespace GenericScriptableArchitecture
{
    using GenericUnityObjects;
    using UnityEditor;
    using UnityEngine;

    public abstract class BaseEvent : GenericScriptableObject
    {
        internal static bool CanBeInvoked(string objectName, string typeName)
        {
#if UNITY_EDITOR
            if ( ! EditorApplication.isPlaying)
            {
                Debug.LogError($"Tried to change the {objectName} {typeName} in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }
    }
}