namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T1, T2, T3> : IEvent<T1, T2, T3>
    {
        void AddListener(IListener<T1, T2, T3> listener);

        void RemoveListener(IListener<T1, T2, T3> listener);

        void AddListener(Action<T1, T2, T3> listener);

        void RemoveListener(Action<T1, T2, T3> listener);

#if UNIRX
        IDisposable Subscribe(IObserver<(T1, T2, T3)> observer);
#endif
    }
}