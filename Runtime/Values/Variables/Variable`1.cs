namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using SolidUtilities.Helpers;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable", MenuName = Config.PackageName + "Variable")]
    public class Variable<T> : BaseVariable, IEquatable<Variable<T>>, IEquatable<T>, IEvent<T>
#if UNIRX
        , IReactiveProperty<T>
#endif
    {
        public IEqualityComparer<T> EqualityComparer = _defaultEqualityComparer;
        private static readonly IEqualityComparer<T> _defaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();

        [SerializeField] internal T _initialValue;
        [SerializeField] internal T _value;
        [SerializeField] internal bool ListenersExpanded;

        private EventHelperWithDefaultValue<T> _eventHelper;

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer.Equals(_value, value))
                    SetValue(value);
            }
        }

        internal override List<Object> Listeners => _eventHelper?.Listeners ?? ListHelper.Empty<Object>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _eventHelper = new EventHelperWithDefaultValue<T>(this, () => _value);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _eventHelper.Dispose();
        }

        public void SetValueAndForceNotify(T value) => SetValue(value);

        #region Adding Removing Listeners

        public void AddListener(ScriptableEventListener<T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(ScriptableEventListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IEventListener<T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(IEventListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddListener(IMultipleEventsListener<T> listener, bool notifyCurrentValue = false) => _eventHelper.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(IMultipleEventsListener<T> listener) => _eventHelper.RemoveListener(listener);

        public void AddResponse(Action<T> response, bool notifyCurrentValue = false) => _eventHelper.AddResponse(response, notifyCurrentValue);

        public void RemoveResponse(Action<T> response) => _eventHelper.RemoveResponse(response);

        #endregion

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

            _eventHelper.NotifyListeners(_value);
        }

        #region Operator Overloads

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

        public static Variable<T> operator +(Variable<T> variable, Action<T> response)
        {
            variable.AddResponse(response);
            return variable;
        }

        public static Variable<T> operator -(Variable<T> variable, Action<T> response)
        {
            variable.RemoveResponse(response);
            return variable;
        }

        public static Variable<T> operator +(Variable<T> variable, ScriptableEventListener<T> listener)
        {
            variable.AddListener(listener);
            return variable;
        }

        public static Variable<T> operator -(Variable<T> variable, ScriptableEventListener<T> listener)
        {
            if (variable == null)
                return null;

            variable.RemoveListener(listener);
            return variable;
        }

        public static Variable<T> operator +(Variable<T> variable, IEventListener<T> listener)
        {
            if (variable == null)
                return null;

            variable.AddListener(listener);
            return variable;
        }

        public static Variable<T> operator -(Variable<T> variable, IEventListener<T> listener)
        {
            if (variable == null)
                return null;

            variable.RemoveListener(listener);
            return variable;
        }

        public static Variable<T> operator +(Variable<T> variable, IMultipleEventsListener<T> response)
        {
            if (variable == null)
                return null;

            variable.AddListener(response);
            return variable;
        }

        public static Variable<T> operator -(Variable<T> variable, IMultipleEventsListener<T> response)
        {
            if (variable == null)
                return null;

            variable.RemoveListener(response);
            return variable;
        }

        #endregion

#if UNIRX
        bool IReadOnlyReactiveProperty<T>.HasValue => true;

        public IDisposable Subscribe(IObserver<T> observer) => _eventHelper.Subscribe(observer);
#endif
    }
}