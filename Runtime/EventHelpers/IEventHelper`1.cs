namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T> : IEvent<T>
#if UNIRX
        , IObservable<T>
#endif
    {
        void AddListener(IListener<T> listener);

        void RemoveListener(IListener<T> listener);

        void AddListener(Action<T> listener);

        void RemoveListener(Action<T> listener);
    }
}