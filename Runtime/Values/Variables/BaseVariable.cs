namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class BaseVariable : BaseValue, IStackTraceProvider
    {
        [SerializeField] private bool _stackTraceEnabled;
        [SerializeField] private bool _stackTraceExpanded;

        private static readonly List<Object> _emptyObjectList = new List<Object>();

        bool IStackTraceProvider.Enabled
        {
            get => _stackTraceEnabled;
            set => _stackTraceEnabled = value;
        }

        bool IStackTraceProvider.Expanded
        {
            get => _stackTraceExpanded;
            set => _stackTraceExpanded = value;
        }

        internal abstract void InvokeValueChangedEvents();

        internal abstract List<Object> Listeners { get; }

        internal virtual List<Object> ListenersWithHistory => _emptyObjectList;

        private readonly StackCollection<StackTraceEntry> _stackTraceEntries = new StackCollection<StackTraceEntry>();

        ICollection<StackTraceEntry> IStackTraceProvider.Entries => _stackTraceEntries;

        [Conditional("UNITY_EDITOR")]
        protected void AddStackTrace(params object[] args)
        {
            if (_stackTraceEnabled)
                _stackTraceEntries.Push(new StackTraceEntry(args));
        }

        protected bool CanBeInvoked()
        {
#if UNITY_EDITOR
            if ( ! EditorApplication.isPlaying)
            {
                Debug.LogError($"Tried to change the {name} variable in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }
    }
}