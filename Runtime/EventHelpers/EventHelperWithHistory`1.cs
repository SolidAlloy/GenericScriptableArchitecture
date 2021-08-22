namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SolidUtilities.Extensions;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    public class EventHelperWithHistory<T> : IEventHelperWithHistory<T>, IDisposable
#if UNIRX
        , IObserverLinkedList<(T, T)>
#endif
    {
        private readonly IEvent<T, T> _parentEvent;
        private readonly List<ScriptableEventListener<T, T>> _scriptableListeners = new List<ScriptableEventListener<T, T>>();
        private readonly List<IEventListener<T, T>> _singleEventListeners = new List<IEventListener<T, T>>();
        private readonly List<IMultipleEventsListener<T, T>> _multipleEventsListeners = new List<IMultipleEventsListener<T, T>>();
        private readonly List<Action<T, T>> _responses = new List<Action<T, T>>();

        private readonly Func<bool> _hasPreviousValue;
        private readonly Func<T> _getPreviousValue;
        private readonly Func<T> _getCurrentValue;

        public List<Object> Listeners => _responses
            .Select(response => response.Target)
            .Concat(_scriptableListeners)
            .Concat(_singleEventListeners)
            .Concat(_multipleEventsListeners)
#if UNIRX
            .Concat(_observers.Select(node => node.GetTarget()))
#endif
            .OfType<Object>()
            .ToList();

        public EventHelperWithHistory(Func<bool> hasPreviousValue, Func<T> getPreviousValue, Func<T> getCurrentValue)
        {
            _hasPreviousValue = hasPreviousValue;
            _getPreviousValue = getPreviousValue;
            _getCurrentValue = getCurrentValue;
        }

        public EventHelperWithHistory(IEvent<T, T> parentEvent, Func<bool> hasPreviousValue, Func<T> getPreviousValue, Func<T> getCurrentValue)
            : this(hasPreviousValue, getPreviousValue, getCurrentValue)
        {
            _parentEvent = parentEvent;
        }

        public void AddListener(IListener<T, T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            if (listener is ScriptableEventListener<T, T> scriptableListener)
            {
                _scriptableListeners.Add(scriptableListener);

                if (notifyCurrentValue && _hasPreviousValue())
                    scriptableListener.OnEventInvoked(_getCurrentValue(), _getCurrentValue());

                return;
            }

            bool isValidListener = false;

            if (listener is IEventListener<T, T> eventListener)
            {
                isValidListener = true;

                if (_singleEventListeners.AddIfMissing(eventListener) && notifyCurrentValue && _hasPreviousValue())
                    eventListener.OnEventInvoked(_getCurrentValue(), _getCurrentValue());
            }

            if (listener is IMultipleEventsListener<T, T> multipleEventsListener)
            {
                isValidListener = true;

                if (_multipleEventsListeners.AddIfMissing(multipleEventsListener) && notifyCurrentValue && _hasPreviousValue())
                    multipleEventsListener.OnEventInvoked(_parentEvent ?? this, _getCurrentValue(), _getCurrentValue());
            }

            if ( ! isValidListener)
                Debug.LogWarning($"Tried to subscribe to {_parentEvent?.ToString() ?? "an event"} with a listener that does not implement any of supported interfaces.");
        }

        public void RemoveListener(IListener<T, T> listener)
        {
            if (listener is ScriptableEventListener<T, T> scriptableListener)
            {
                _scriptableListeners.Remove(scriptableListener);
                return;
            }

            if (listener is IEventListener<T, T> eventListener)
            {
                _singleEventListeners.Remove(eventListener);
            }

            if (listener is IMultipleEventsListener<T, T> multipleEventsListener)
            {
                _multipleEventsListeners.Remove(multipleEventsListener);
            }
        }

        public void AddListener(Action<T, T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            if (_responses.Contains(listener))
                return;

            _responses.Add(listener);

            if (notifyCurrentValue && _hasPreviousValue())
            {
                listener.Invoke(_getPreviousValue(), _getCurrentValue());
            }
        }

        public void RemoveListener(Action<T, T> listener) => _responses.Remove(listener);

        public void NotifyListeners(T previousValue, T currentValue)
        {
            for (int i = _scriptableListeners.Count - 1; i != -1; i--)
            {
                _scriptableListeners[i].OnEventInvoked(previousValue, currentValue);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventInvoked(previousValue, currentValue);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventInvoked(_parentEvent ?? this, previousValue, currentValue);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(previousValue, currentValue);
            }

#if UNIRX
            RaiseOnNext(previousValue, currentValue);
#endif
        }

        #region UniRx
#if UNIRX
        private bool _disposed;
        private ObserverNode<(T, T)> _root;
        private ObserverNode<(T, T)> _last;
        private readonly List<ObserverNode<(T, T)>> _observers = new List<ObserverNode<(T, T)>>();

        public IDisposable Subscribe(Action<T, T> onNext)
        {
            return Subscribe(ObserverUtil.CreateSubscribeObserver(onNext, Stubs.Throw, Stubs.Nop));
        }

        public IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError)
        {
            return Subscribe(ObserverUtil.CreateSubscribeObserver(onNext, onError, Stubs.Nop));
        }

        public IDisposable Subscribe(Action<T, T> onNext, Action onCompleted)
        {
            return Subscribe(ObserverUtil.CreateSubscribeObserver(onNext, Stubs.Throw, onCompleted));
        }

        public IDisposable Subscribe(Action<T, T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return Subscribe(ObserverUtil.CreateSubscribeObserver(onNext, onError, onCompleted));
        }

        public IDisposable Subscribe(IObserver<(T Previous, T Current)> observer)
        {
            if (_disposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // raise latest value on subscribe
            if (_hasPreviousValue())
                observer.OnNext((_getPreviousValue(), _getCurrentValue()));

            // subscribe node, node as subscription.
            var next = new ObserverNode<(T, T)>(this, observer);

            _observers.Add(next);

            if (_root == null)
            {
                _root = _last = next;
            }
            else
            {
                _last.Next = next;
                next.Previous = _last;
                _last = next;
            }

            return next;
        }

        void IObserverLinkedList<(T, T)>.UnsubscribeNode(ObserverNode<(T, T)> node)
        {
            _observers.Remove(node);

            if (node == _root)
                _root = node.Next;

            if (node == _last)
                _last = node.Previous;

            if (node.Previous != null)
                node.Previous.Next = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
        }

        private void RaiseOnNext(T previousValue, T currentValue)
        {
            if (_disposed)
                return;

            var node = _root;
            while (node != null)
            {
                node.OnNext((previousValue, currentValue));
                node = node.Next;
            }
        }
#endif

        public void Dispose()
        {
#if UNIRX
            if (_disposed)
                return;

            _disposed = true;

            var node = _root;
            _root = _last = null;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
#endif
        }

        #endregion
    }
}