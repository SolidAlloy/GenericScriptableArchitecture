namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelperWithDefaultValue<T> : IEvent<T>
    {
        void AddListener(IListener<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IListener<T> listener);

        void AddListener(Action<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T> listener);

#if UNIRX
        IDisposable Subscribe(IObserver<T> observer);
#endif
    }
}