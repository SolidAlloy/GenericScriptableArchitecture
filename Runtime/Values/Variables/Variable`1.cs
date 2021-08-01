namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GenericUnityObjects;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable", MenuName = Config.PackageName + "Variable")]
    public class Variable<T> : BaseVariable, IEquatable<Variable<T>>, IEquatable<T>, IEvent<T>
#if UNIRX
        , IReactiveProperty<T>, IDisposable, IObserverLinkedList<T>
#endif
    {
        public IEqualityComparer<T> EqualityComparer = _defaultEqualityComparer;
        private static readonly IEqualityComparer<T> _defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();

        [SerializeField] internal T _initialValue;
        [SerializeField] internal T _value;
        [SerializeField] internal bool ListenersExpanded;

        private List<ScriptableEventListener<T>> _listeners = new List<ScriptableEventListener<T>>();
        private List<IMultipleEventsListener<T>> _multipleEventsListeners = new List<IMultipleEventsListener<T>>();
        private List<IEventListener<T>> _singleEventListeners = new List<IEventListener<T>>();
        private List<Action<T>> _responses = new List<Action<T>>();

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer.Equals(_value, value))
                    SetValue(value);
            }
        }

        internal override List<Object> Listeners
            => _responses
                .Select(response => response.Target)
                .Concat(_singleEventListeners)
                .Concat(_multipleEventsListeners)
                .Concat(_listeners)
                .OfType<Object>()
                .ToList();

        public void SetValueAndForceNotify(T value) => SetValue(value);

        public void AddListener(ScriptableEventListener<T> listener) => _listeners.Add(listener);

        public void RemoveListener(ScriptableEventListener<T> listener) => _listeners.Remove(listener);

        public void AddListener(IMultipleEventsListener<T> listener) => _multipleEventsListeners.Add(listener);

        public void RemoveListener(IMultipleEventsListener<T> listener) => _multipleEventsListeners.Remove(listener);

        public void AddListener(IEventListener<T> listener) => _singleEventListeners.Add(listener);

        public void RemoveListener(IEventListener<T> listener) => _singleEventListeners.Remove(listener);

        public void AddResponse(Action<T> response) => _responses.Add(response);

        public void RemoveResponse(Action<T> response) => _responses.Remove(response);

        protected override void InitializeValues()
        {
            _value = _initialValue.DeepCopyInEditor();
        }

        protected virtual void SetValue(T value)
        {
            _value = value;
            AddStackTrace(_value);
            InvokeValueChangedEvents();
        }

        internal override void InvokeValueChangedEvents()
        {
            if ( ! CanBeInvoked())
                return;

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(_value);
            }

            for (int i = _responses.Count - 1; i != -1; i--)
            {
                _responses[i].Invoke(_value);
            }

            for (int i = _multipleEventsListeners.Count - 1; i != -1; i--)
            {
                _multipleEventsListeners[i].OnEventRaised(this, _value);
            }

            for (int i = _singleEventListeners.Count - 1; i != -1; i--)
            {
                _singleEventListeners[i].OnEventRaised(_value);
            }

#if UNIRX
            RaiseOnNext();
#endif
        }

        public static implicit operator T(Variable<T> variable) => variable.Value;

        public override string ToString() => $"Variable{{{Value}}}";

        public bool Equals(Variable<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _value.Equals(other._value);
        }

        public bool Equals(T other)
        {
            return ! (other is null) && Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is Variable<T> typedObj)
                return Equals(typedObj);

            if (obj is T tObj)
                return Equals(tObj);

            return false;
        }

        /// <summary>
        /// Use with caution. The value contained by a Variable instance can be changed through inspector.
        /// </summary>
        /// <returns>Hash code of the instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _value?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static bool operator ==(Variable<T> lhs, Variable<T> rhs)
        {
            if ((Object)lhs == null)
            {
                return (Object)rhs == null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Variable<T> lhs, Variable<T> rhs)
        {
            return ! (lhs == rhs);
        }

        public static bool operator ==(Variable<T> lhs, T rhs)
        {
            if ((Object)lhs == null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Variable<T> lhs, T rhs)
        {
            return ! (lhs == rhs);
        }

        #region UniRx
#if UNIRX
        private bool _disposed;
        private ObserverNode<T> _root;
        private ObserverNode<T> _last;

        bool IReadOnlyReactiveProperty<T>.HasValue => true;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_disposed)
            {
                observer.OnCompleted();
                return Disposable.Empty;
            }

            // raise latest value on subscribe
            observer.OnNext(_value);

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

        public virtual void Dispose()
        {
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
        }

        protected virtual void RaiseOnNext()
        {
            if (_disposed)
                return;

            var node = _root;
            while (node != null)
            {
                node.OnNext(_value);
                node = node.Next;
            }
        }
#endif
        #endregion
    }
}