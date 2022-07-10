namespace GenericScriptableArchitecture
{
    using System;

#if UNIRX
    using UniRx;
#endif

    public interface IScriptableEvent : IEvent
#if UNIRX
        , IObservable<Unit>
#endif
    {
        void Invoke();
        void AddListener(Action listener);
        void RemoveListener(Action listener);
        void AddListener(IListener listener);
        void RemoveListener(IListener listener);
    }

    public interface IScriptableEvent<T> : IEvent<T>
#if UNIRX
        , IObservable<T>
#endif
    {
        void Invoke(T arg0);
        void AddListener(Action<T> listener);
        void RemoveListener(Action<T> listener);
        void AddListener(IListener<T> listener);
        void RemoveListener(IListener<T> listener);
    }

    public interface IScriptableEvent<T1, T2> : IEvent<T1, T2>
#if UNIRX
        , IObservable<(T1, T2)>
#endif
    {
        void Invoke(T1 arg0, T2 arg1);
        void AddListener(Action<T1, T2> listener);
        void RemoveListener(Action<T1, T2> listener);
        void AddListener(IListener<T1, T2> listener);
        void RemoveListener(IListener<T1, T2> listener);
    }

    public interface IScriptableEvent<T1, T2, T3> : IEvent<T1, T2, T3>
#if UNIRX
        , IObservable<(T1, T2, T3)>
#endif
    {
        void Invoke(T1 arg0, T2 arg1, T3 arg2);
        void AddListener(Action<T1, T2, T3> listener);
        void RemoveListener(Action<T1, T2, T3> listener);
        void AddListener(IListener<T1, T2, T3> listener);
        void RemoveListener(IListener<T1, T2, T3> listener);
    }
}