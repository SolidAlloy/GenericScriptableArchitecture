namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelperWithHistory<T> : IEvent<T, T>
#if UNIRX
        , IObservable<(T Previous, T Current)>
#endif
    {
        void AddListener(IListener<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IListener<T, T> listener);

        void AddListener(Action<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T, T> listener);
    }
}