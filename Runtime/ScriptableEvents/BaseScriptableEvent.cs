namespace GenericScriptableArchitecture
{
    using JetBrains.Annotations;
    using SolidUtilities;
    using UnityEngine;

    public abstract class BaseScriptableEvent : BaseEvent
    {
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;
        [SerializeField] internal string[] _argNames;

        internal abstract ScriptableEventHelper ScriptableEventHelper { get; }
    }
}