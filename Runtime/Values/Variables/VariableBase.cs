namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;

    public abstract class VariableBase : ValueBase, IStackTraceProvider
    {
        protected static readonly List<ScriptableEventListenerBase> EmptyList = new List<ScriptableEventListenerBase>();

        internal abstract void InvokeValueChangedEvents();

        internal abstract List<ScriptableEventListenerBase> Listeners { get; }

        internal abstract List<ScriptableEventListenerBase> ListenersWithHistory { get; }

        private readonly Stack<StackTraceEntry> _stackTraceEntries = new Stack<StackTraceEntry>();

        IEnumerable<StackTraceEntry> IStackTraceProvider.StackTraceEntries => _stackTraceEntries;

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}