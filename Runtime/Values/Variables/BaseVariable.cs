namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;

    public abstract class BaseVariable : BaseValue, IStackTraceProvider
    {
        [SerializeField] private bool _stackTraceExpanded;

        bool IStackTraceProvider.Expanded
        {
            get => _stackTraceExpanded;
            set => _stackTraceExpanded = value;
        }

        protected static readonly List<BaseScriptableEventListener> EmptyList = new List<BaseScriptableEventListener>();

        internal abstract void InvokeValueChangedEvents();

        internal abstract List<BaseScriptableEventListener> Listeners { get; }

        internal abstract List<BaseScriptableEventListener> ListenersWithHistory { get; }

        private readonly StackCollection<StackTraceEntry> _stackTraceEntries = new StackCollection<StackTraceEntry>();

        ICollection<StackTraceEntry> IStackTraceProvider.Entries => _stackTraceEntries;

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}