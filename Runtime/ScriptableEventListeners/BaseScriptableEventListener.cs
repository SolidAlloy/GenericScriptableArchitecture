namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;

    public abstract class BaseScriptableEventListener : MonoBehaviour, IStackTraceProvider
    {
        private readonly StackCollection<StackTraceEntry> _stackTraceEntries = new StackCollection<StackTraceEntry>();

        ICollection<StackTraceEntry> IStackTraceProvider.StackTraceEntries => _stackTraceEntries;

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}