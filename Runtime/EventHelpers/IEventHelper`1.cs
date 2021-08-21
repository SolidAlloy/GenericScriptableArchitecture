namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T> : IEvent<T>
    {
        void AddListener(IListener<T> listener);

        void RemoveListener(IListener<T> listener);

        void AddListener(Action<T> listener);

        void RemoveListener(Action<T> listener);

#if UNIRX
        IObservable<T> Observe();

        IDisposable Subscribe(IObserver<T> observer);
#endif
    }
}