namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EasyButtons;
    using UnityEngine;
#if UNIRX
    using UniRx;
#endif

    [Serializable]
    [CreateAssetMenu(menuName = Config.PackageName + Config.Events + "ScriptableEvent")]
    public class ScriptableEvent : BaseScriptableEvent, IEvent
#if UNIRX
        , IObservable<Unit>, IDisposable
#endif
    {
        private List<ScriptableEventListener> _listeners = new List<ScriptableEventListener>();
        private List<IMultipleEventsListener> _multipleEventsListeners = new List<IMultipleEventsListener>();
        private List<IEventListener> _singleEventListeners = new List<IEventListener>();
        private List<Action> _responses = new List<Action>();

        internal override List<UnityEngine.Object> Listeners
            => _responses
                .Select(response => response.Target)
                .Concat(_singleEventListeners)
                .Concat(_multipleEventsListeners)
                .Concat(_listeners)
                .OfType<UnityEngine.Object>()
                .ToList();

        [Button(Mode = ButtonMode.EnabledInPlayMode)]
        public void Invoke()
        {
            if ( ! CanBeInvoked())
                return;

            AddStackTrace();

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised();
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke();
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventRaised(this);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventRaised();
            }

#if UNIRX
            _observableHelper?.RaiseOnNext(Unit.Default);
#endif
        }

        public void AddListener(ScriptableEventListener listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener listener) => _listeners.Remove(listener);

        public void AddResponse(Action response) => _responses.Add(response);

        public void RemoveResponse(Action response) => _responses.Remove(response);

        public void AddListener(IMultipleEventsListener listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener listener) => _multipleEventsListeners.Remove(listener);

        public void AddListener(IEventListener listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener listener) => _singleEventListeners.Remove(listener);

        #region UniRx
#if UNIRX
        private ObservableHelper<Unit> _observableHelper;

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            _observableHelper ??= new ObservableHelper<Unit>();
            return _observableHelper.Subscribe(observer);
        }

        public void Dispose() => _observableHelper?.Dispose();
#endif
        #endregion
    }
}