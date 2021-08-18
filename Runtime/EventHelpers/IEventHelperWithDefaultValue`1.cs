namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelperWithDefaultValue<T> : IEvent<T>
    {
        void AddListener(IEventListener<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IEventListener<T> listener);

        void AddListener(IMultipleEventsListener<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IMultipleEventsListener<T> listener);

        void AddListener(Action<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T> listener);

#if UNIRX
        IObservable<T> Observe(bool notifyCurrentValue = false);

        IDisposable Subscribe(IObserver<T> observer);
#endif
    }
}