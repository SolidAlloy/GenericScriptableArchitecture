namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities;
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public abstract class BaseScriptableEvent : BaseEvent
    {
        [SerializeField] internal bool ListenersExpanded;
        [SerializeField] internal string[] _argNames;
        [SerializeField] internal StackTraceProvider _stackTrace;
        [SerializeField, ResizableTextArea, UsedImplicitly] private string _description;

        internal abstract List<Object> Listeners { get; }



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