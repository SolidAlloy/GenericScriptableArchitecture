namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using UnityEngine;

    public abstract class ScriptableEventBase : GenericScriptableObject
    {
        [SerializeField] internal bool ListenersExpanded;
        [SerializeField] internal bool ResponseTargetsExpanded;

        internal abstract List<ScriptableEventListenerBase> Listeners { get; }

        internal abstract List<Object> ResponseTargets { get; }
    }
}