namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using UnityEngine;

    public abstract class RuntimeSetBase : GenericScriptableObject
    {
        internal abstract List<Object> List { get; }
    }
}