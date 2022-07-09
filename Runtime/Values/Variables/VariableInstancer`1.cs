namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    // Such an execution order allows the instance to be awaken before all the default classes including those predefined in execution order by Unity (like EventSystem with -1000 order).
    // It's ok that instancers are awaken even before EventSystem because they don't depend on any of those classes.
    [Serializable, DefaultExecutionOrder(-2000)]
    public class VariableInstancer<T> : BaseVariableInstancer, IVariable<T>
    {
        [SerializeField] internal Variable<T> _variableReference;
        [SerializeField] internal VariableHelper<T> _variableHelper;

        public T InitialValue => _variableReference == null ? default : _variableReference._initialValue;

        public T Value
        {
            get => _variableHelper.Value;
            set
            {
                if (_variableReference == null)
                    return;

                if (!_variableReference.EqualityComparer.Equals(_variableHelper.Value, value))
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

                T oldValue = _variableHelper.Value;
                T newValue = newVariableReference != null ? newVariableReference.Value : default;

                var equalityComparer = newVariableReference?.EqualityComparer // use new equality comparer if possible
                                       ?? _variableReference?.EqualityComparer // use old equality comparer if new is not accessible
                                       ?? UnityEqualityComparer.GetDefault<T>(); // finally, default back to default comparer

                // publish the new value to listeners if it is different in the new variable reference.
                // For listeners, it will look like the value changed, and they won't know anything about variable references change.
                if (!equalityComparer.Equals(oldValue, newValue))
                    SetValue(newValue);

                _variableReference = newVariableReference;
            }
        }

        internal override BaseVariable BaseVariableReference
        {
            get => _variableReference;
            set => VariableReference = (Variable<T>) value;
        }

        private bool _initialized;
        internal override bool Initialized => _initialized;

        VariableHelper IVariable.VariableHelper => _variableHelper;

        protected virtual void Awake()
        {
            _variableHelper.Initialize(this, name, "variable instancer");
            if (_variableReference != null) _variableHelper.Value = _variableReference._initialValue;
            _initialized = true;
        }

        protected virtual void OnDestroy()
        {
            _variableHelper.Event.Dispose();
        }

        public void SetValueAndForceNotify(T value)
        {
            if (_variableReference != null)
                SetValue(value);
        }

        protected virtual void SetValue(T value) => _variableHelper.SetValue(value);

        #region Adding Removing Listeners

        public void AddListener(IListener<T> listener, bool notifyCurrentValue = false)
        {
            if (!_initialized)
                Debug.LogError("Tried to add a listener before the instancer was awaken.");

            _variableHelper.Event.AddListener(listener, notifyCurrentValue);
        }

        public void RemoveListener(IListener<T> listener) => _variableHelper.Event.RemoveListener(listener);

        public void AddListener(Action<T> listener, bool notifyCurrentValue = false) => _variableHelper.Event.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(Action<T> listener) => _variableHelper.Event.RemoveListener(listener);

        #endregion

        #region Operator Overloads

        public static implicit operator T(VariableInstancer<T> variable) => variable.Value;

        public override string ToString() => $"VariableInstancer '{name}' {{{Value}}}";

        public bool Equals(T other)
        {
            if (ReferenceEquals(_variableHelper.Value, other))
                return true;

            return Value.Equals(other);
        }

        public bool Equals(IVariable<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _variableHelper.Value.Equals(other.Value);
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
        public IDisposable Subscribe(IObserver<T> observer) => _variableHelper.Event.Subscribe(observer);

        bool IReadOnlyReactiveProperty<T>.HasValue => _variableReference != null;
#endif
    }
}