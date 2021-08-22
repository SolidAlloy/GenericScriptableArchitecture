namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T1, T2> : IEvent<T1, T2>
#if UNIRX
        , IObservable<(T1, T2)>
#endif
    {
        void AddListener(IListener<T1, T2> listener);

        void RemoveListener(IListener<T1, T2> listener);

        void AddListener(Action<T1, T2> listener);

        void RemoveListener(Action<T1, T2> listener);
    }
}