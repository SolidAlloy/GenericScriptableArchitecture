namespace ExtendedScriptableObjects
{
    using GenericUnityObjects;
    using UnityEngine;

    /// <summary>
    /// A scriptable object whose data is not saved automatically in play mode.
    /// </summary>
    public abstract class PlayModeUnchangedScriptableObject : GenericScriptableObject
    {
        [SerializeField, HideInInspector] internal bool IsSecondObject;
    }
}