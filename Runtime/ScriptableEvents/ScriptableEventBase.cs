namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;

    public abstract class ScriptableEventBase : GenericScriptableObject, IStackTraceProvider
    {
        [SerializeField] internal bool ListenersExpanded;
        [SerializeField] internal bool ResponseTargetsExpanded;
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        private readonly Stack<StackTraceEntry> _stackTraceEntries = new Stack<StackTraceEntry>();

        IEnumerable<StackTraceEntry> IStackTraceProvider.StackTraceEntries => _stackTraceEntries;

        internal abstract List<ScriptableEventListenerBase> Listeners { get; }

        internal abstract List<Object> ResponseTargets { get; }

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            _stackTraceEntries.Push(new StackTraceEntry(args));
        }
    }
}