namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;

    public abstract class ScriptableEventBase : GenericScriptableObject
    {
        [SerializeField] internal bool ListenersExpanded;
        [SerializeField] internal bool ResponseTargetsExpanded;
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        internal abstract List<ScriptableEventListenerBase> Listeners { get; }

        internal abstract List<Object> ResponseTargets { get; }
    }
}