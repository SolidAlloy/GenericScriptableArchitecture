namespace GenericScriptableArchitecture
{
    using System;

    public interface IEvent
    {
        public void AddListener(ScriptableEventListener listener);

        public void RemoveListener(ScriptableEventListener listener);

        public void AddResponse(Action response);

        public void RemoveResponse(Action response);

        public void AddListener(IMultipleEventsListener listener);

        public void RemoveListener(IMultipleEventsListener listener);

        public void AddListener(IEventListener listener);

        public void RemoveListener(IEventListener listener);
    }

    public interface IEvent<T>
    {
        public void AddListener(ScriptableEventListener<T> listener);

        public void RemoveListener(ScriptableEventListener<T> listener);

        public void AddResponse(Action<T> response);

        public void RemoveResponse(Action<T> response);

        public void AddListener(IMultipleEventsListener<T> listener);

        public void RemoveListener(IMultipleEventsListener<T> listener);

        public void AddListener(IEventListener<T> listener);

        public void RemoveListener(IEventListener<T> listener);
    }

    public interface IEvent<T1, T2>
    {
        public void AddListener(ScriptableEventListener<T1, T2> listener);

        public void RemoveListener(ScriptableEventListener<T1, T2> listener);

        public void AddResponse(Action<T1, T2> response);

        public void RemoveResponse(Action<T1, T2> response);

        public void AddListener(IMultipleEventsListener<T1, T2> listener);

        public void RemoveListener(IMultipleEventsListener<T1, T2> listener);

        public void AddListener(IEventListener<T1, T2> listener);
    }

    public interface IEvent<T1, T2, T3>
    {
        public void AddListener(ScriptableEventListener<T1, T2, T3> listener);

        public void RemoveListener(ScriptableEventListener<T1, T2, T3> listener);

        public void AddResponse(Action<T1, T2, T3> response);

        public void RemoveResponse(Action<T1, T2, T3> response);

        public void AddListener(IMultipleEventsListener<T1, T2, T3> listener);

        public void RemoveListener(IMultipleEventsListener<T1, T2, T3> listener);

        public void AddListener(IEventListener<T1, T2, T3> listener);

        public void RemoveListener(IEventListener<T1, T2, T3> listener);
    }
}