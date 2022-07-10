namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using SolidUtilities;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    internal abstract class ScriptableEventHelper
    {
        [SerializeField] public bool ListenersExpanded;
        [SerializeField] public StackTraceProvider StackTrace;

        private string _objectName;
        private string _typeName;

        internal List<Object> Listeners => _listeners ?? ListHelper.Empty<Object>();

        protected abstract List<Object> _listeners { get; }

        public void Initialize(string objectName, string typeName)
        {
            _objectName = objectName;
            _typeName = typeName;
        }

        protected bool CanBeInvoked() => BaseEvent.CanBeInvoked(_objectName, _typeName);
    }

    [Serializable]
    internal class ScriptableEventHelperVoid : ScriptableEventHelper
    {
        public EventHelper Event;

        protected override List<Object> _listeners => Event?.Listeners;

        public void Initialize(IEvent parentEvent, string objectName, string typeName)
        {
            Initialize(objectName, typeName);
            Event = new EventHelper(parentEvent);
        }

        public void Invoke()
        {
            if ( ! CanBeInvoked())
                return;

            StackTrace.AddStackTrace();
            Event.NotifyListeners();
        }
    }

    [Serializable]
    internal class ScriptableEventHelper<T> : ScriptableEventHelper
    {
        public EventHelper<T> Event;

        protected override List<Object> _listeners => Event?.Listeners;

        public void Initialize(IEvent<T> parentEvent, string objectName, string typeName)
        {
            Initialize(objectName, typeName);
            Event = new EventHelper<T>(parentEvent);
        }

        public void Invoke(T arg0)
        {
            if ( ! CanBeInvoked())
                return;

            StackTrace.AddStackTrace(arg0);
            Event.NotifyListeners(arg0);
        }
    }

    [Serializable]
    internal class ScriptableEventHelper<T1, T2> : ScriptableEventHelper
    {
        public EventHelper<T1, T2> Event;

        protected override List<Object> _listeners => Event?.Listeners;

        public void Initialize(IEvent<T1, T2> parentEvent, string objectName, string typeName)
        {
            Initialize(objectName, typeName);
            Event = new EventHelper<T1, T2>(parentEvent);
        }

        public void Invoke(T1 arg0, T2 arg1)
        {
            if ( ! CanBeInvoked())
                return;

            StackTrace.AddStackTrace(arg0, arg1);
            Event.NotifyListeners(arg0, arg1);
        }
    }

    [Serializable]
    internal class ScriptableEventHelper<T1, T2, T3> : ScriptableEventHelper
    {
        public EventHelper<T1, T2, T3> Event;

        protected override List<Object> _listeners => Event?.Listeners;

        public void Initialize(IEvent<T1, T2, T3> parentEvent, string objectName, string typeName)
        {
            Initialize(objectName, typeName);
            Event = new EventHelper<T1, T2, T3>(parentEvent);
        }

        public void Invoke(T1 arg0, T2 arg1, T3 arg2)
        {
            if ( ! CanBeInvoked())
                return;

            StackTrace.AddStackTrace(arg0, arg1, arg2);
            Event.NotifyListeners(arg0, arg1, arg2);
        }
    }
}