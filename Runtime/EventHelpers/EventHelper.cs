namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Object = UnityEngine.Object;

    public class EventHelper : IEventHelper, IDisposable
    {
        private readonly IEvent _parentEvent;
        private readonly List<ScriptableEventListener> _scriptableListeners = new List<ScriptableEventListener>();
        private readonly List<IEventListener> _singleEventListeners = new List<IEventListener>();
        private readonly List<IMultipleEventsListener> _multipleEventsListeners = new List<IMultipleEventsListener>();
        private readonly List<Action> _responses = new List<Action>();

        public List<Object> Listeners => _responses
            .Select(response => response.Target)
            .Concat(_scriptableListeners)
            .Concat(_singleEventListeners)
            .Concat(_multipleEventsListeners)
            .OfType<Object>()
            .ToList();

        public EventHelper()
        {
        }

        public EventHelper(IEvent parentEvent)
        {
            _parentEvent = parentEvent;
        }

        public void AddListener(ScriptableEventListener listener) => _scriptableListeners.Add(listener);

        public void RemoveListener(ScriptableEventListener listener) => _scriptableListeners.Remove(listener);

        public void AddListener(IEventListener listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener listener) => _singleEventListeners.Remove(listener);

        public void AddListener(IMultipleEventsListener listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener listener) => _multipleEventsListeners.Remove(listener);

        public void AddResponse(Action response) => _responses.Add(response);

        public void RemoveResponse(Action response) => _responses.Remove(response);

        public void NotifyListeners()
        {
            for (int i = _scriptableListeners.Count - 1; i != -1; i--)
            {
                _scriptableListeners[i].OnEventInvoked();
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventInvoked();
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventInvoked(_parentEvent ?? this);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke();
            }

#if UNIRX
            _subject?.OnNext(Unit.Default);
            _observableHelper?.RaiseOnNext(Unit.Default);
#endif
        }

        #region UniRx
#if UNIRX
        private bool _disposed;

        private Subject<Unit> _subject;

        public IObservable<Unit> Observe()
        {
            if (_disposed)
                return Observable.Empty<Unit>();

            return _subject ??= new Subject<Unit>();
        }

        private ObservableHelper<Unit> _observableHelper;

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            _observableHelper ??= new ObservableHelper<Unit>();
            return _observableHelper.Subscribe(observer);
        }
#endif

        public void Dispose()
        {
#if UNIRX
            if (_disposed)
                return;

            _disposed = true;

            _observableHelper?.Dispose();

            if (_subject == null)
                return;

            try
            {
                _subject.OnCompleted();
            }
            finally
            {
                _subject.Dispose();
                _subject = null;
            }
#endif
        }

        #endregion
    }
}