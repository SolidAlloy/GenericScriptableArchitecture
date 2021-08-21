namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SolidUtilities.Extensions;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    public class EventHelperWithDefaultValue<T> : IEventHelperWithDefaultValue<T>, IDisposable
#if UNIRX
        , IObserverLinkedList<T>
#endif
    {
        private readonly IEvent<T> _parentEvent;
        private readonly List<ScriptableEventListener<T>> _scriptableListeners = new List<ScriptableEventListener<T>>();
        private readonly List<IEventListener<T>> _singleEventListeners = new List<IEventListener<T>>();
        private readonly List<IMultipleEventsListener<T>> _multipleEventsListeners = new List<IMultipleEventsListener<T>>();
        private readonly List<Action<T>> _responses = new List<Action<T>>();
        private readonly Func<T> _getCurrentValue;

        public List<Object> Listeners => _responses
            .Select(response => response.Target)
            .Concat(_scriptableListeners)
            .Concat(_singleEventListeners)
            .Concat(_multipleEventsListeners)
            .OfType<Object>()
            .ToList();

        public EventHelperWithDefaultValue(Func<T> getCurrentValue)
        {
            _getCurrentValue = getCurrentValue;
        }

        public EventHelperWithDefaultValue(IEvent<T> parentEvent, Func<T> getCurrentValue)
            : this(getCurrentValue)
        {
            _parentEvent = parentEvent;
        }

        public void AddListener(IListener<T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            if (listener is ScriptableEventListener<T> scriptableListener)
            {
                _scriptableListeners.Add(scriptableListener);

                if (notifyCurrentValue)
                    scriptableListener.OnEventInvoked(_getCurrentValue());

                return;
            }

            if (listener is IEventListener<T> eventListener)
            {
                if (_singleEventListeners.AddIfMissing(eventListener) && notifyCurrentValue)
                    eventListener.OnEventInvoked(_getCurrentValue());
            }

            if (listener is IMultipleEventsListener<T> multipleEventsListener)
            {
                if (_multipleEventsListeners.AddIfMissing(multipleEventsListener) && notifyCurrentValue)
                    multipleEventsListener.OnEventInvoked(_parentEvent ?? this, _getCurrentValue());
            }
        }

        public void RemoveListener(IListener<T> listener)
        {
            if (listener is ScriptableEventListener<T> scriptableListener)
            {
                _scriptableListeners.Remove(scriptableListener);
                return;
            }

            if (listener is IEventListener<T> eventListener)
            {
                _singleEventListeners.Remove(eventListener);
            }

            if (listener is IMultipleEventsListener<T> multipleEventsListener)
            {
                _multipleEventsListeners.Remove(multipleEventsListener);
            }
        }

        public void AddListener(Action<T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            if (_responses.Contains(listener))
                return;

            _responses.Add(listener);

            if (notifyCurrentValue)
            {
                listener.Invoke(_getCurrentValue());
            }
        }

        public void RemoveListener(Action<T> listener) => _responses.Remove(listener);

        public void NotifyListeners(T value)
        {
            for (int i = _scriptableListeners.Count - 1; i != -1; i--)
            {
                _scriptableListeners[i].OnEventInvoked(value);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventInvoked(value);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventInvoked(_parentEvent ?? this, value);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(value);
            }

#if UNIRX
            RaiseOnNext(value);
#endif
        }

        #region UniRx
#if UNIRX
        private bool _disposed;

        private ObserverNode<T> _root;
        private ObserverNode<T> _last;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_disposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // raise latest value on subscribe
            observer.OnNext(_getCurrentValue());

            // subscribe node, node as subscription.
            var next = new ObserverNode<T>(this, observer);

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

        void IObserverLinkedList<T>.UnsubscribeNode(ObserverNode<T> node)
        {
            if (node == _root)
                _root = node.Next;

            if (node == _last)
                _last = node.Previous;

            if (node.Previous != null)
                node.Previous.Next = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
        }

        private void RaiseOnNext(T value)
        {
            if (_disposed)
                return;

            var node = _root;
            while (node != null)
            {
                node.OnNext(value);
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