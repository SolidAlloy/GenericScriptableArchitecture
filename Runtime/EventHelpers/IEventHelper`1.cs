namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T> : IEvent<T>
    {
        public void AddListener(ScriptableEventListener<T> listener);

        public void RemoveListener(ScriptableEventListener<T> listener);

        void AddListener(IEventListener<T> listener);

        void RemoveListener(IEventListener<T> listener);

        void AddListener(IMultipleEventsListener<T> listener);

        void RemoveListener(IMultipleEventsListener<T> listener);

        void AddResponse(Action<T> response);

        void RemoveResponse(Action<T> response);

#if UNIRX
        IObservable<T> Observe();

        IDisposable Subscribe(IObserver<T> observer);
#endif
    }
}