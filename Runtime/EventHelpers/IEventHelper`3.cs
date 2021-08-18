namespace GenericScriptableArchitecture
{
    using System;

    public interface IEventHelper<T1, T2, T3> : IEvent<T1, T2, T3>
    {
        public void AddListener(ScriptableEventListener<T1, T2, T3> listener);

        public void RemoveListener(ScriptableEventListener<T1, T2, T3> listener);

        void AddListener(IEventListener<T1, T2, T3> listener);

        void RemoveListener(IEventListener<T1, T2, T3> listener);

        void AddListener(IMultipleEventsListener<T1, T2, T3> listener);

        void RemoveListener(IMultipleEventsListener<T1, T2, T3> listener);

        void AddListener(Action<T1, T2, T3> listener);

        void RemoveListener(Action<T1, T2, T3> listener);

#if UNIRX
        IDisposable Subscribe(IObserver<(T1, T2, T3)> observer);
#endif
    }
}