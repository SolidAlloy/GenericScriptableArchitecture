namespace GenericScriptableArchitecture
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using GenericUnityObjects;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    [CreateGenericAssetMenu(FileName = "New Variable", MenuName = Config.PackageName + "Variable")]
    public class Variable<T> : VariableBase, IEquatable<Variable<T>>, IEquatable<T>
    {
        [SerializeField] internal T _initialValue;

        [SerializeField] internal T _value;

        [SerializeField] internal bool ListenersExpanded;

        private List<ScriptableEventListener<T>> _listeners = new List<ScriptableEventListener<T>>();

        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        internal override List<ScriptableEventListenerBase> Listeners => _listeners.ConvertAll(item => (ScriptableEventListenerBase) item);

        internal override List<ScriptableEventListenerBase> ListenersWithHistory => EmptyList;

        public void AddListenerOnChange(ScriptableEventListener<T> listener) => _listeners.Add(listener);

        public void RemoveListenerOnChange(ScriptableEventListener<T> listener) => _listeners.Remove(listener);

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
            if (ApplicationUtil.InEditMode)
                return;

            for (int i = _listeners.Count - 1; i != -1; i--)
            {
                _listeners[i].OnEventRaised(_value);
            }
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

        [Conditional("UNITY_EDITOR")]
        private void AddToStackTrace()
        {

        }
    }
}