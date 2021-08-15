namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelperWithHistory<T> : IEvent<T, T>
    {
        void AddListener(IEventListener<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IEventListener<T, T> listener);

        void AddListener(IMultipleEventsListener<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IMultipleEventsListener<T, T> listener);

        void AddResponse(Action<T, T> response, bool notifyCurrentValue = false);

        void RemoveResponse(Action<T, T> response);

#if UNIRX
        IDisposable Subscribe(IObserver<(T Previous, T Current)> observer);
#endif
    }
}