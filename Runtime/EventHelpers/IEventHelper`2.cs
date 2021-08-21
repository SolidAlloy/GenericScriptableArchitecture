namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T1, T2> : IEvent<T1, T2>
    {
        void AddListener(IListener<T1, T2> listener);

        void RemoveListener(IListener<T1, T2> listener);

        void AddListener(Action<T1, T2> listener);

        void RemoveListener(Action<T1, T2> listener);

#if UNIRX
        IDisposable Subscribe(IObserver<(T1, T2)> observer);
#endif
    }
}