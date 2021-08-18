namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public void AddListener(ScriptableEventListener<T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            _scriptableListeners.Add(listener);

            if (notifyCurrentValue)
            {
                listener.OnEventInvoked(_getCurrentValue());
            }
        }

        public void RemoveListener(ScriptableEventListener<T> listener) => _scriptableListeners.Remove(listener);

        public void AddListener(IEventListener<T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            if (_singleEventListeners.Contains(listener))
                return;

            _singleEventListeners.Add(listener);

            if (notifyCurrentValue)
            {
                listener.OnEventInvoked(_getCurrentValue());
            }
        }

        public void RemoveListener(IEventListener<T> listener) => _singleEventListeners.Remove(listener);

        public void AddListener(IMultipleEventsListener<T> listener, bool notifyCurrentValue = false)
        {
            if (listener == null)
                return;

            if (_multipleEventsListeners.Contains(listener))
                return;

            _multipleEventsListeners.Add(listener);

            if (notifyCurrentValue)
            {
                listener.OnEventInvoked(_parentEvent ?? this, _getCurrentValue());
            }
        }

        public void RemoveListener(IMultipleEventsListener<T> listener) => _multipleEventsListeners.Remove(listener);

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
            _subject?.OnNext(value);
            RaiseOnNext(value);
#endif
        }

        #region UniRx
#if UNIRX
        private bool _disposed;
        private Subject<T> _subject;
        public IObservable<T> Observe(bool notifyCurrentValue = false)
        {
            if (_disposed)
                return Observable.Empty<T>();

            _subject ??= new Subject<T>();
            return notifyCurrentValue ? _subject.StartWith(() => _getCurrentValue()) : _subject;
        }

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

        private void DisposeObservable()
        {
            var node = _root;
            _root = _last = null;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        private void DisposeSubject()
        {
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
        }

#endif

        public void Dispose()
        {
#if UNIRX
            if (_disposed)
                return;

            _disposed = true;
            DisposeObservable();
            DisposeSubject();
#endif
        }
        #endregion
    }
}