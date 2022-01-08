namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SolidUtilities;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    public class EventHelper : IEventHelper, IDisposable
    {
        private readonly IEvent _parentEvent;
        private readonly List<VoidScriptableEventListener> _scriptableListeners = new List<VoidScriptableEventListener>();
        private readonly List<IEventListener> _singleEventListeners = new List<IEventListener>();
        private readonly List<IMultipleEventsListener> _multipleEventsListeners = new List<IMultipleEventsListener>();
        private readonly List<Action> _responses = new List<Action>();

        public List<Object> Listeners => _responses
            .Select(response => response.Target)
            .Concat(_scriptableListeners)
            .Concat(_singleEventListeners)
            .Concat(_multipleEventsListeners)
#if UNIRX
            .Concat(_observableHelper?.Targets ?? Enumerable.Empty<object>())
#endif
            .OfType<Object>()
            .ToList();

        public EventHelper()
        {
        }

        public EventHelper(IEvent parentEvent)
        {
            _parentEvent = parentEvent;
        }

        public void AddListener(IListener listener)
        {
            if (listener == null)
                return;

            if (listener is VoidScriptableEventListener scriptableListener)
            {
                _scriptableListeners.Add(scriptableListener);
                return;
            }

            bool isValidListener = false;

            if (listener is IEventListener eventListener)
            {
                isValidListener = true;
                _singleEventListeners.AddIfMissing(eventListener);
            }

            if (listener is IMultipleEventsListener multipleEventsListener)
            {
                isValidListener = true;
                _multipleEventsListeners.AddIfMissing(multipleEventsListener);
            }

            if ( ! isValidListener)
                Debug.LogWarning($"Tried to subscribe to {_parentEvent?.ToString() ?? "an event"} with a listener that does not implement any of supported interfaces.");
        }

        public void RemoveListener(IListener listener)
        {
            if (listener is VoidScriptableEventListener scriptableListener)
            {
                _scriptableListeners.Remove(scriptableListener);
                return;
            }

            if (listener is IEventListener eventListener)
            {
                _singleEventListeners.Remove(eventListener);
            }

            if (listener is IMultipleEventsListener multipleEventsListener)
            {
                _multipleEventsListeners.Remove(multipleEventsListener);
            }
        }

        public void AddListener(Action listener)
        {
            if (listener == null)
                return;

            _responses.AddIfMissing(listener);
        }

        public void RemoveListener(Action listener) => _responses.Remove(listener);

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
            _observableHelper?.RaiseOnNext(Unit.Default);
#endif
        }

        #region UniRx
#if UNIRX

        private bool _disposed;

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
#endif
        }

        #endregion
    }
}