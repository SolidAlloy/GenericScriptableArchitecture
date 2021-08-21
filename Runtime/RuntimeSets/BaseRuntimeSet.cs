namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;

    public abstract class BaseRuntimeSet : GenericScriptableObject
    {
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        internal abstract List<Object> List { get; }

        [SerializeField] internal bool AddExpanded;
        internal abstract List<Object> AddListeners { get; }

        [SerializeField] internal bool CountChangeExpanded;
        internal abstract List<Object> CountChangeListeners { get; }

        [SerializeField] internal bool MoveExpanded;
        internal abstract List<Object> MoveListeners { get; }

        [SerializeField] internal bool RemoveExpanded;
        internal abstract List<Object> RemoveListeners { get; }

        [SerializeField] internal bool ReplaceExpanded;
        internal abstract List<Object> ReplaceListeners { get; }

        [SerializeField] internal bool ResetExpanded;
        internal abstract List<Object> ResetListeners { get; }
    }
}