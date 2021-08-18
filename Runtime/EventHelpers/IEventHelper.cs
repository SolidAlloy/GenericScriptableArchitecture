namespace GenericScriptableArchitecture
{
    using System;
    using UniRx;

    public interface IEventHelper : IEvent
    {
        public void AddListener(ScriptableEventListener listener);

        public void RemoveListener(ScriptableEventListener listener);

        void AddListener(IEventListener listener);

        void RemoveListener(IEventListener listener);

        void AddListener(IMultipleEventsListener listener);

        void RemoveListener(IMultipleEventsListener listener);

        void AddListener(Action listener);

        void RemoveListener(Action listener);

#if UNIRX
        IObservable<Unit> Observe();

        IDisposable Subscribe(IObserver<Unit> observer);
#endif
    }
}