namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class BaseScriptableEventListener : MonoBehaviour, IStackTraceProvider
    {
        [SerializeField] private bool _stackTraceEnabled;
        [SerializeField] private bool _stackTraceExpanded;

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
                Debug.LogError($"Tried to listen with the {name} listener in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }
    }
}