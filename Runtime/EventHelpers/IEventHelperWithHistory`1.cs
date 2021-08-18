namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelperWithHistory<T> : IEvent<T, T>
    {
        void AddListener(IEventListener<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IEventListener<T, T> listener);

        void AddListener(IMultipleEventsListener<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IMultipleEventsListener<T, T> listener);

        void AddListener(Action<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T, T> listener);

#if UNIRX
        IDisposable Subscribe(IObserver<(T Previous, T Current)> observer);
#endif
    }
}