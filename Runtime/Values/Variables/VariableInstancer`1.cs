namespace GenericScriptableArchitecture
{
    using System;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class VariableInstancer<T> : MonoBehaviour, IVariable<T>
    {
        // TODO: in edit mode, this needs to draw object reference and initial value
        // In play mode, draw only the object reference.
        [SerializeField] internal Variable<T> _variableReference;

        // TODO: in edit mode, this is null and not drawn.
        // In play mode, this needs to draw the current value.
        internal Variable<T> _variableInstance;

        private EventHelperWithDefaultValue<T> _eventHelper;

        public T InitialValue => _variableInstance == null ? default : _variableInstance._initialValue;

        public T Value
        {
            get => _variableInstance == null ? default : _variableInstance._value;
            set { if (_variableInstance != null) _variableInstance.Value = value; }
        }

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

        private void Awake()
        {
            _eventHelper = new EventHelperWithDefaultValue<T>(this, () => Value);

            if (_variableReference != null)
            {
                _variableInstance = Instantiate(_variableReference);
                _variableInstance += _eventHelper.NotifyListeners;
            }
        }

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

        public void SetValueAndForceNotify(T value)
        {
            if (_variableInstance != null) _variableInstance.SetValueAndForceNotify(value);
        }

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

#if UNIRX
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _variableInstance == null ? Disposable.Empty : _variableInstance.Subscribe(observer);
        }

        bool IReadOnlyReactiveProperty<T>.HasValue => _variableInstance != null;
#endif
    }
}