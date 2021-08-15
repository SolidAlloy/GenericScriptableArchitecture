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

        internal abstract List<Object> AddListeners { get; }

        internal abstract List<Object> CountChangeListeners { get; }

        internal abstract List<Object> MoveListeners { get; }

        internal abstract List<Object> RemoveListeners { get; }

        internal abstract List<Object> ReplaceListeners { get; }

        internal abstract List<Object> ResetListeners { get; }
    }
}