namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using UnityEngine;

    public abstract class CollectionBase : GenericScriptableObject
    {
        internal abstract List<Object> List { get; }
    }
}