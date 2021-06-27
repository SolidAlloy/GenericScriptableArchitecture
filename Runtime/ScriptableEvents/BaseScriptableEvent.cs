namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;

    public abstract class BaseScriptableEvent : GenericScriptableObject, IStackTraceProvider
    {
        [SerializeField] internal bool ListenersExpanded;
        [SerializeField] internal bool ResponseTargetsExpanded;
        [SerializeField] private bool _stackTraceExpanded;

        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        private readonly StackCollection<StackTraceEntry> _stackTraceEntries = new StackCollection<StackTraceEntry>();

        ICollection<StackTraceEntry> IStackTraceProvider.Entries => _stackTraceEntries;

        bool IStackTraceProvider.Expanded
        {
            get => _stackTraceExpanded;
            set => _stackTraceExpanded = value;
        }

        internal abstract List<BaseScriptableEventListener> Listeners { get; }

        internal abstract List<Object> ResponseTargets { get; }

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}