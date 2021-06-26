namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;

    public abstract class RuntimeSetBase : GenericScriptableObject
    {
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        internal abstract List<Object> List { get; }
    }
}