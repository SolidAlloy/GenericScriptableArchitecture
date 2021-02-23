namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using UnityEngine;

    public abstract class ScriptableEventBase : GenericScriptableObject
    {
        [SerializeField] internal bool ListenersExpanded;

        internal abstract List<ScriptableEventListenerBase> Listeners { get; }
    }
}