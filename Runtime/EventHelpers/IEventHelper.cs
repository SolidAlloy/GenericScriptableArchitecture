namespace GenericScriptableArchitecture
{
    using System;
    using UniRx;

    public interface IEventHelper : IEvent
    {
        public void AddListener(IListener listener);

        public void RemoveListener(IListener listener);

        void AddListener(Action listener);

        void RemoveListener(Action listener);

#if UNIRX
        IObservable<Unit> Observe();

        IDisposable Subscribe(IObserver<Unit> observer);
#endif
    }
}