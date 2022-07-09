namespace GenericScriptableArchitecture
{
    using System;
    using System.Diagnostics;
    using UnityEngine;

    [Serializable]
    internal class StackTraceProvider
    {
        [SerializeField] public bool Enabled;

        public readonly StackCollection<StackTraceEntry> Entries = new StackCollection<StackTraceEntry>();

        [Conditional("UNITY_EDITOR")]
        public void AddStackTrace(params object[] args)
        {
            if (Enabled)
                Entries.Push(new StackTraceEntry(args));
        }
    }
}