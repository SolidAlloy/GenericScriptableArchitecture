namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UniRx;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    public class VariableInstancer<T> : BaseVariableInstancer, IVariable<T>
    {
        [SerializeField] internal Variable<T> _variableReference;

        private EventHelperWithDefaultValue<T> _eventHelper;

        public T InitialValue => _variableReference == null ? default : _variableReference._initialValue;

        [SerializeField] internal T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (_variableReference == null)
                    return;

                if (_variableReference.EqualityComparer.Equals(_value, value))
                    return;

                SetValue(value);
            }
        }

        [PublicAPI]
        public Variable<T> VariableReference
        {
            get => _variableReference;
            set
            {
                var newVariableReference = value; // just for better naming clarity.

                T oldValue = _value;
                T newValue = newVariableReference != null ? newVariableReference._value : default;

                var equalityComparer = newVariableReference?.EqualityComparer // use new equality comparer if possible
                                       ?? _variableReference?.EqualityComparer // use old equality comparer if new is not accessible
                                       ?? UnityEqualityComparer.GetDefault<T>(); // finally, default back to default comparer

                // publish the new value to listeners if it is different in the new variable reference.
                // For listeners, it will look like the value changed, and they won't know anything about variable references change.
                if (!equalityComparer.Equals(oldValue, newValue))
                    _eventHelper.NotifyListeners(newValue);

                _variableReference = newVariableReference;
                _value = newValue;
            }
        }

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
                _value = _variableReference._initialValue;
            }

            _initialized = true;
        }

        private void OnDestroy()
        {
            _eventHelper.Dispose();
        }

        public void SetValueAndForceNotify(T value)
        {
            if (_variableReference == null)
                return;

            SetValue(value);
        }

        private void SetValue(T value)
        {
            _value = value;

            // No stack trace here like in variable but should we add it?
            if ( ! CanBeInvoked())
                return;

            _eventHelper.NotifyListeners(_value);
        }

        protected bool CanBeInvoked()
        {
#if UNITY_EDITOR
            if ( ! EditorApplication.isPlaying)
            {
                Debug.LogError($"Tried to change the {name} variable in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }

        #region Adding Removing Listeners

        public void AddListener(IListener<T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(IListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(Action<T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(Action<T> listener) => _eventHelper.RemoveListener(listener);

        #endregion

        #region Operator Overloads

        public static implicit operator T(VariableInstancer<T> variable) => variable.Value;

        public override string ToString() => $"VariableInstancer{{{Value}}}";

        public bool Equals(T other)
        {
            if (ReferenceEquals(_value, other))
                return true;

            return Value.Equals(other);
        }

        public bool Equals(IVariable<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _value.Equals(other.Value);
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
        public IDisposable Subscribe(IObserver<T> observer) => _eventHelper.Subscribe(observer);

        bool IReadOnlyReactiveProperty<T>.HasValue => _variableReference != null;
#endif
    }
}