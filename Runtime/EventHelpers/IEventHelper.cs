namespace GenericScriptableArchitecture
{
    using System;

#if UNIRX
    using UniRx;
#endif

    public interface IEventHelper : IEvent
#if UNIRX
        , IObservable<Unit>
#endif
    {
        public void AddListener(IListener listener);

        public void RemoveListener(IListener listener);

        void AddListener(Action listener);

        void RemoveListener(Action listener);
    }
}