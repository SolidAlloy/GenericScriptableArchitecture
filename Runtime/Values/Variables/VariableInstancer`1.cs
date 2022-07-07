namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UniRx;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    public class VariableInstancer<T> : BaseVariableInstancer, IVariable<T>
    {
        [SerializeField] internal Variable<T> _variableReference;

        private Variable<T> _variableInstance;
        private EventHelperWithDefaultValue<T> _eventHelper;

        public T InitialValue => _variableInstance == null ? default : _variableInstance._initialValue;

        public T Value
        {
            get => _variableInstance == null ? default : _variableInstance._value;
            set { if (_variableInstance != null) _variableInstance.Value = value; }
        }

        [PublicAPI]
        public Variable<T> VariableReference
        {
            get => _variableReference;
            set
            {
                _variableReference = value;

                _variableInstance -= _eventHelper.NotifyListeners;

                T oldValue = _variableInstance != null ? _variableInstance._value : default;
                T newValue = _variableReference != null ? _variableReference._value : default;
                var equalityComparer = _variableReference?.EqualityComparer // use new equality comparer if possible
                                       ?? _variableInstance?.EqualityComparer // use old equality comparer if new is not accessible
                                       ?? UnityEqualityComparer.GetDefault<T>(); // finally, default back to default comparer

                // publish the new value to old listeners if it is different in the new variable reference.
                // For listeners, it will look like the value changed, and the won't know anything about variable references change.
                if (!equalityComparer.Equals(oldValue, newValue))
                    _eventHelper.NotifyListeners(newValue);

                if (_variableReference == null)
                {
                    _variableInstance = null;
                    return;
                }

                _variableInstance = Instantiate(_variableReference);
                _variableInstance += _eventHelper.NotifyListeners;
                _variableInstance.EqualityComparer = equalityComparer;
            }
        }

        internal override BaseVariable VariableInstance => _variableInstance;

        internal override BaseVariable BaseVariableReference
        {
            get => _variableReference;
            set => VariableReference = (Variable<T>) value;
        }

        private bool _initialized;
        internal override bool Initialized => _initialized;

        private void Awake()
        {
            _eventHelper = new EventHelperWithDefaultValue<T>(this, () => Value);

            if (_variableReference != null)
            {
                _variableInstance = Instantiate(_variableReference);
                _variableInstance += _eventHelper.NotifyListeners;
            }

            _initialized = true;
        }

        private void OnDestroy()
        {
            if (_variableInstance != null)
                Destroy(_variableInstance);

            _eventHelper.Dispose();
        }

        public void SetValueAndForceNotify(T value)
        {
            if (_variableInstance != null) _variableInstance.SetValueAndForceNotify(value);
        }

        #region Adding Removing Listeners

        public void AddListener(IListener<T> listener, bool notifyCurrentValue = false)
        {
            if (_variableInstance != null) _variableInstance.AddListener(listener, notifyCurrentValue);
        }

        public void RemoveListener(IListener<T> listener)
        {
            if (_variableInstance != null) _variableInstance.RemoveListener(listener);
        }

        public void AddListener(Action<T> listener, bool notifyCurrentValue = false)
        {
            if (_variableInstance != null) _variableInstance.AddListener(listener, notifyCurrentValue);
        }

        public void RemoveListener(Action<T> listener)
        {
            if (_variableInstance != null) _variableInstance.RemoveListener(listener);
        }

        #endregion

        #region Operator Overloads

        public static implicit operator T(VariableInstancer<T> variable) => variable.Value;

        public override string ToString() => $"VariableInstancer{{{Value}}}";

        public bool Equals(T other)
        {
            if (_variableInstance == null)
            {
                return other == null || other.Equals(default);
            }

            if (ReferenceEquals(_variableInstance._value, other))
                return true;

            return _variableInstance._value.Equals(other);
        }

        public bool Equals(IVariable<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _variableInstance._value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is IVariable<T> typedObj)
                return Equals(typedObj);

            if (obj is T tObj)
                return Equals(tObj);

            return false;
        }

        /// <summary>
        /// Use with caution. The value contained by a VariableInstancer instance can be changed through inspector.
        /// </summary>
        /// <returns>Hash code of the instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Value?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static bool operator ==(VariableInstancer<T> lhs, VariableInstancer<T> rhs)
        {
            if ((Object)lhs == null)
            {
                return (Object)rhs == null;
            }

            return lhs.Equals((IVariable<T>)rhs);
        }

        public static bool operator !=(VariableInstancer<T> lhs, VariableInstancer<T> rhs)
        {
            return ! (lhs == rhs);
        }

        public static bool operator ==(VariableInstancer<T> lhs, T rhs)
        {
            if ((Object)lhs == null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(VariableInstancer<T> lhs, T rhs)
        {
            return ! (lhs == rhs);
        }

        public static VariableInstancer<T> operator +(VariableInstancer<T> variableInstancer, Action<T> listener)
        {
            if (variableInstancer == null)
                return null;

            variableInstancer.AddListener(listener);
            return variableInstancer;
        }

        public static VariableInstancer<T> operator +(VariableInstancer<T> variableInstancer, (Action<T> Listener, bool NotifyCurrentValue) args)
        {
            if (variableInstancer == null)
                return null;

            variableInstancer.AddListener(args.Listener, args.NotifyCurrentValue);
            return variableInstancer;
        }

        public static VariableInstancer<T> operator -(VariableInstancer<T> variableInstancer, Action<T> listener)
        {
            if (variableInstancer == null)
                return null;

            variableInstancer.RemoveListener(listener);
            return variableInstancer;
        }

        public static VariableInstancer<T> operator +(VariableInstancer<T> variableInstancer, IListener<T> listener)
        {
            if (variableInstancer == null)
                return null;

            variableInstancer.AddListener(listener);
            return variableInstancer;
        }

        public static VariableInstancer<T> operator +(VariableInstancer<T> variableInstancer, (IListener<T> Listener, bool NotifyCurrentValue) args)
        {
            if (variableInstancer == null)
                return null;

            variableInstancer.AddListener(args.Listener, args.NotifyCurrentValue);
            return variableInstancer;
        }

        public static VariableInstancer<T> operator -(VariableInstancer<T> variableInstancer, IListener<T> listener)
        {
            if (variableInstancer == null)
                return null;

            variableInstancer.RemoveListener(listener);
            return variableInstancer;
        }

        #endregion

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _variableInstance == null ? Disposable.Empty : _variableInstance.Subscribe(observer);
        }

        bool IReadOnlyReactiveProperty<T>.HasValue => _variableInstance != null;
#endif
    }
}