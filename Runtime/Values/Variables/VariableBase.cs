namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public abstract class VariableBase : ValueBase, IStackTraceProvider
    {
        protected static readonly List<BaseScriptableEventListener> EmptyList = new List<BaseScriptableEventListener>();

        internal abstract void InvokeValueChangedEvents();

        internal abstract List<BaseScriptableEventListener> Listeners { get; }

        internal abstract List<BaseScriptableEventListener> ListenersWithHistory { get; }

        private readonly StackCollection<StackTraceEntry> _stackTraceEntries = new StackCollection<StackTraceEntry>();

        ICollection<StackTraceEntry> IStackTraceProvider.StackTraceEntries => _stackTraceEntries;

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}