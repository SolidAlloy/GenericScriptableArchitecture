namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;

    public abstract class ScriptableEventListenerBase : MonoBehaviour, IStackTraceProvider
    {
        private readonly Stack<StackTraceEntry> _stackTraceEntries = new Stack<StackTraceEntry>();

        IEnumerable<StackTraceEntry> IStackTraceProvider.StackTraceEntries => _stackTraceEntries;

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}