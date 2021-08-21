namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SolidUtilities.Extensions;
    using UniRx;
    using Object = UnityEngine.Object;

    public class EventHelper<T> : IEventHelper<T>, IDisposable
    {
        private readonly IEvent<T> _parentEvent;
        private readonly List<ScriptableEventListener<T>> _scriptableListeners = new List<ScriptableEventListener<T>>();
        private readonly List<IEventListener<T>> _singleEventListeners = new List<IEventListener<T>>();
        private readonly List<IMultipleEventsListener<T>> _multipleEventsListeners = new List<IMultipleEventsListener<T>>();
        private readonly List<Action<T>> _responses = new List<Action<T>>();

        public List<Object> Listeners => _responses
            .Select(response => response.Target)
            .Concat(_scriptableListeners)
            .Concat(_singleEventListeners)
            .Concat(_multipleEventsListeners)
            .OfType<Object>()
            .ToList();

        public EventHelper() { }

        public EventHelper(IEvent<T> parentEvent)
        {
            _parentEvent = parentEvent;
        }

        public void AddListener(IListener<T> listener)
        {
            if (listener == null)
                return;

            if (listener is ScriptableEventListener<T> scriptableListener)
            {
                _scriptableListeners.Add(scriptableListener);
            }
            else if (listener is IEventListener<T> eventListener)
            {
                _singleEventListeners.AddIfMissing(eventListener);
            }
            else if (listener is IMultipleEventsListener<T> multipleEventsListener)
            {
                _multipleEventsListeners.AddIfMissing(multipleEventsListener);
            }
        }

        public void RemoveListener(IListener<T> listener)
        {
            if (listener is ScriptableEventListener<T> scriptableListener)
            {
                _scriptableListeners.Remove(scriptableListener);
            }
            else if (listener is IEventListener<T> eventListener)
            {
                _singleEventListeners.Remove(eventListener);
            }
            else if (listener is IMultipleEventsListener<T> multipleEventsListener)
            {
                _multipleEventsListeners.Remove(multipleEventsListener);
            }
        }

        public void AddListener(Action<T> listener)
        {
            if (listener == null)
                return;

            _responses.AddIfMissing(listener);
        }

        public void RemoveListener(Action<T> listener) => _responses.Remove(listener);

        public void NotifyListeners(T arg0)
        {
            for (int i = _scriptableListeners.Count - 1; i != -1; i--)
            {
                _scriptableListeners[i].OnEventInvoked(arg0);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventInvoked(arg0);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventInvoked(_parentEvent ?? this, arg0);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(arg0);
            }

#if UNIRX
            _subject?.OnNext(arg0);
            _observableHelper?.RaiseOnNext(arg0);
#endif
        }

        #region UniRx

#if UNIRX
        private bool _disposed;

        private Subject<T> _subject;
        public IObservable<T> Observe()
        {
            if (_disposed)
                return Observable.Empty<T>();

            return _subject ??= new Subject<T>();
        }

        private ObservableHelper<T> _observableHelper;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observableHelper ??= new ObservableHelper<T>();
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