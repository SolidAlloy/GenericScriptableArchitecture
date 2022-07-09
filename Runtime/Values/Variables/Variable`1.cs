namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using GenericUnityObjects;
    using UnityEditor;
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;

#if UNIRX
    using UniRx;
#endif

    [Serializable]
    internal abstract class VariableHelper
    {
        [SerializeField] public StackTraceProvider StackTrace;

        public abstract EventHelperWithDefaultValue EventHelper { get; }

        // TODO: Placing it here for now but we might want to change the location of the method.
        public static bool CanBeInvoked(string objectName, string typeName)
        {
#if UNITY_EDITOR
            if ( ! EditorApplication.isPlaying)
            {
                Debug.LogError($"Tried to change the {objectName} {typeName} in edit mode. This is not allowed.");
                return false;
            }
#endif
            return true;
        }

        public abstract void InvokeValueChangedEvents();
    }

    [Serializable]
    internal class VariableHelper<T> : VariableHelper
    {
        public static readonly IEqualityComparer<T> DefaultEqualityComparer = UnityEqualityComparer.GetDefault<T>();

        [SerializeField] public T Value;
        [SerializeField] public bool ListenersExpanded;

        public EventHelperWithDefaultValue<T> Event;

        public override EventHelperWithDefaultValue EventHelper => Event;

        private string _objectName;
        private string _typeName;

        public void Initialize(IVariable<T> parentEvent, string objectName, string typeName)
        {
            _objectName = objectName;
            _typeName = typeName;
            Event = new EventHelperWithDefaultValue<T>(parentEvent, () => Value);
        }

        public void SetValue(T value)
        {
            Value = value;
            StackTrace.AddStackTrace(Value);
            InvokeValueChangedEvents();
        }

        public override void InvokeValueChangedEvents()
        {
            if ( ! CanBeInvoked(_objectName, _typeName))
                return;

            Event.NotifyListeners(Value);
        }
    }

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable", MenuName = Config.PackageName + "Variable")]
    public class Variable<T> : BaseVariable, IVariable<T>
    {
        [SerializeField] internal T _initialValue;
        [SerializeField] internal VariableHelper<T> _variableHelper;

        public IEqualityComparer<T> EqualityComparer = VariableHelper<T>.DefaultEqualityComparer;

        public T InitialValue => _initialValue;

        public T Value
        {
            get => _variableHelper.Value;
            set
            {
                if ( ! EqualityComparer.Equals(Value, value))
                    SetValue(value);
            }
        }

        VariableHelper IVariable.VariableHelper => _variableHelper;

        protected virtual void SetValue(T value) => _variableHelper.SetValue(value);

        protected override void OnDisable()
        {
            base.OnDisable();
            _variableHelper.Event?.Dispose();
        }

        public void SetValueAndForceNotify(T value) => _variableHelper.SetValue(value);

        protected override void InitializeValues()
        {
            _variableHelper.Initialize(this, name, "variable");
            _variableHelper.Value = SerializedCopyInEditor(_initialValue);
        }

        #region Adding Removing Listeners

        public void AddListener(IListener<T> listener, bool notifyCurrentValue = false) => _variableHelper.Event.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(IListener<T> listener) => _variableHelper.Event.RemoveListener(listener);

        public void AddListener(Action<T> listener, bool notifyCurrentValue = false) => _variableHelper.Event.AddListener(listener, notifyCurrentValue);

        public void RemoveListener(Action<T> listener) => _variableHelper.Event.RemoveListener(listener);

        #endregion

        #region Operator Overloads

        public static implicit operator T(Variable<T> variable) => variable.Value;

        public override string ToString() => $"Variable '{name}' {{{Value}}}";

        public bool Equals(IVariable<T> other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _variableHelper.Value.Equals(other.Value);
        }

        public bool Equals(T other)
        {
            if (ReferenceEquals(_variableHelper.Value, other))
                return true;

            return Value.Equals(other);
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
        /// Use with caution. The value contained by a Variable instance can be changed through inspector.
        /// </summary>
        /// <returns>Hash code of the instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _variableHelper.Value?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static bool operator ==(Variable<T> lhs, Variable<T> rhs)
        {
            if ((Object)lhs == null)
            {
                return (Object)rhs == null;
            }

            return lhs.Equals((IVariable<T>)rhs);
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

        public static Variable<T> operator +(Variable<T> variable, Action<T> listener)
        {
            if (variable == null)
                return null;

            variable.AddListener(listener);
            return variable;
        }

        public static Variable<T> operator +(Variable<T> variable, (Action<T> Listener, bool NotifyCurrentValue) args)
        {
            if (variable == null)
                return null;

            variable.AddListener(args.Listener, args.NotifyCurrentValue);
            return variable;
        }

        public static Variable<T> operator -(Variable<T> variable, Action<T> listener)
        {
            if (variable == null)
                return null;

            variable.RemoveListener(listener);
            return variable;
        }

        public static Variable<T> operator +(Variable<T> variable, IListener<T> listener)
        {
            if (variable == null)
                return null;

            variable.AddListener(listener);
            return variable;
        }

        public static Variable<T> operator +(Variable<T> variable, (IListener<T> Listener, bool NotifyCurrentValue) args)
        {
            if (variable == null)
                return null;

            variable.AddListener(args.Listener, args.NotifyCurrentValue);
            return variable;
        }

        public static Variable<T> operator -(Variable<T> variable, IListener<T> listener)
        {
            if (variable == null)
                return null;

            variable.RemoveListener(listener);
            return variable;
        }

        #endregion

#if UNIRX
        bool IReadOnlyReactiveProperty<T>.HasValue => true;

        public IDisposable Subscribe(IObserver<T> observer) => _variableHelper.Event.Subscribe(observer);
#endif
    }
}