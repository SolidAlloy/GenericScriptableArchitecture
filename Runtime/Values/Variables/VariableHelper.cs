namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    internal abstract class VariableHelper
    {
        [SerializeField] public StackTraceProvider StackTrace;

        public abstract EventHelperWithDefaultValue EventHelper { get; }

        public abstract void InvokeValueChangedEvents();
    }

    [Serializable]
    internal class VariableHelper<T> : VariableHelper
    {
        public static readonly IEqualityComparer<T> DefaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();

        [SerializeField] public T Value;
        [SerializeField] public bool ListenersExpanded;

        public EventHelperWithDefaultValue<T> Event;

        public override EventHelperWithDefaultValue EventHelper => Event;

        private string _objectName;
        private string _typeName;

        public void Initialize(IVariable<T> parentEvent, string objectName, string typeName)
        {
            _objectName = objectName;
            _typeName = typeName;
            Event = new EventHelperWithDefaultValue<T>(parentEvent, () => Value);
        }

        public void SetValue(T value)
        {
            Value = value;
            StackTrace.AddStackTrace(Value);
            InvokeValueChangedEvents();
        }

        public override void InvokeValueChangedEvents()
        {
            if ( !BaseEvent.CanBeInvoked(_objectName, _typeName))
                return;

            Event.NotifyListeners(Value);
        }
    }

}