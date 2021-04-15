namespace GenericScriptableArchitecture
{
    using System;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using SolidUtilities.UnityEngineInternals;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CreateGenericAssetMenu(FileName = "New Variable")]
    [Serializable]
    public class Variable<T> : VariableBase, IEquatable<Variable<T>>, IEquatable<Reference<T>>
    {
        [ResizableTextArea, UsedImplicitly]
        [SerializeField] private string _description;

        [SerializeField] internal T _initialValue;
        [SerializeField] internal T _value;

        [SerializeField] private ScriptableEvent<T> _changed;

        public T Value
        {
            get => _value;
            set => SetValue(value);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            // If domain reload is disabled, OnEnable will not be called on Play Mode start, so we need to plug into
            // the 'play mode state changed' event.
            if (EditorSettings.enterPlayModeOptionsEnabled
                && EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload))
            {
                EditorApplication.playModeStateChanged += InitializeValues;
            }
#endif

            // DeepCopy() is not very performant, so execute it only in Play Mode.
            if (ApplicationUtil.InPlayMode)
                InitializeValues();
        }

        private void InitializeValues(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.ExitingEditMode)
                InitializeValues();
        }

        protected virtual void InitializeValues()
        {
            _value = _initialValue.DeepCopy();
        }

        protected virtual void SetValue(T value)
        {
            _value = value;
            InvokeValueChangedEvents();
        }

        internal override void InvokeValueChangedEvents()
        {
            if (ApplicationUtil.InEditMode)
                return;

            if (_changed != null) _changed.Invoke(_value);
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

        public bool Equals(Reference<T> other)
        {
            return ! (other is null) && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals((Reference<T>) obj);
        }

        /// <summary>
        /// Use with caution. The value contained by a Variable instance can change at any time.
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

        public static bool operator ==(Variable<T> lhs, Reference<T> rhs)
        {
            if ((Object)lhs == null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Variable<T> lhs, Reference<T> rhs)
        {
            return ! (lhs == rhs);
        }
    }
}