namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
    using UnityEditor;
#endif

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

        protected bool CanBeInvoked()
        {
#if UNITY_EDITOR
            if ( ! EditorApplication.isPlaying)
            {
                Debug.LogError($"Tried to invoke the {name} event in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }
    }
}