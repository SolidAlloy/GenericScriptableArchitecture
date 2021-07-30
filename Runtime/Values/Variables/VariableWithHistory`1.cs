namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using UnityEngine;
#if UNIRX
    using UniRx;
#endif

    using Object = UnityEngine.Object;

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable With History", MenuName = Config.PackageName + "Variable With History")]
    public class VariableWithHistory<T> : Variable<T>, IEvent<T, T>
#if UNIRX
        , IReactivePropertyWithHistory<T>, IObserverLinkedList<(T, T)>
#endif
    {
        [SerializeField] internal T _previousValue;
        [SerializeField] internal bool ListenersWithHistoryExpanded;

        // It can be named _listeners, but Unity falsely claims it is serialized multiple times
        private List<ScriptableEventListener<T, T>> _listenersWithHistory = new List<ScriptableEventListener<T, T>>();
        private List<IMultipleEventsListener<T, T>> _multipleEventsListeners = new List<IMultipleEventsListener<T, T>>();
        private List<IEventListener<T, T>> _singleEventListeners = new List<IEventListener<T, T>>();
        private List<Action<T, T>> _responses = new List<Action<T, T>>();

        [PublicAPI]
        public T PreviousValue => _previousValue;

        internal override List<Object> ListenersWithHistory
            => _responses
                .Select(response => response.Target)
                .Concat(_singleEventListeners)
                .Concat(_multipleEventsListeners)
                .Concat(_listenersWithHistory)
                .OfType<Object>()
                .ToList();

        public void AddListener(ScriptableEventListener<T, T> listener) => _listenersWithHistory.Add(listener);

        public void RemoveListener(ScriptableEventListener<T, T> listener) => _listenersWithHistory.Remove(listener);

        public void AddListener(IMultipleEventsListener<T, T> listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener<T, T> listener) => _multipleEventsListeners.Remove(listener);

        public void AddListener(IEventListener<T, T> listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener<T, T> listener) => _singleEventListeners.Remove(listener);

        public void AddResponse(Action<T, T> response) => _responses.Add(response);

        public void RemoveResponse(Action<T, T> response) => _responses.Remove(response);

        protected override void InitializeValues()
        {
            base.InitializeValues();
            _previousValue = _initialValue.DeepCopyInEditor();
        }

        protected override void SetValue(T value)
        {
            _previousValue = _value;
            _value = value;
            AddStackTrace(_previousValue, _value);
            InvokeValueChangedEvents();
        }

        internal override void InvokeValueChangedEvents()
        {
            if ( ! CanBeInvoked())
                return;

            base.InvokeValueChangedEvents();

            for (int i = _listenersWithHistory.Count - 1; i != -1; i--)
            {
                _listenersWithHistory[i].OnEventRaised(_previousValue, _value);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(_previousValue, _value);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventRaised(this, _previousValue, _value);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventRaised(_previousValue, _value);
            }
        }

        public override string ToString() => $"VariableWithHistory{{{Value}}}";

        #region UNIRX
#if UNIRX
        // It can be named _disposed, but Unity falsely claims it is serialized multiple times.
        private bool _disposedWithHistory;
        private ObserverNode<(T, T)> _root;
        private ObserverNode<(T, T)> _last;

        bool IReadOnlyReactivePropertyWithHistory<T>.HasValue => true;

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
            if (_disposedWithHistory)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // raise latest value on subscribe
            observer.OnNext((_previousValue, _value));

            // subscribe node, node as subscription.
            var next = new ObserverNode<(T, T)>(this, observer);

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
            if (node == _root)
                _root = node.Next;

            if (node == _last)
                _last = node.Previous;

            if (node.Previous != null)
                node.Previous.Next = node.Next;

            if (node.Next != null)
                node.Next.Previous = node.Previous;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (_disposedWithHistory)
                return;

            _disposedWithHistory = true;

            var node = _root;
            _root = _last = null;

            while (node != null)
            {
                node.OnCompleted();
                node = node.Next;
            }
        }

        protected override void RaiseOnNext()
        {
            base.RaiseOnNext();

            if (_disposedWithHistory)
                return;

            var node = _root;
            while (node != null)
            {
                node.OnNext((_previousValue, _value));
                node = node.Next;
            }
        }
#endif
        #endregion

    }
}