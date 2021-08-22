namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelperWithDefaultValue<T> : IEvent<T>
#if UNIRX
        , IObservable<T>
#endif
    {
        void AddListener(IListener<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IListener<T> listener);

        void AddListener(Action<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T> listener);
    }
}