namespace GenericScriptableArchitecture
{
    using System;
    using GenericUnityObjects;
    using JetBrains.Annotations;
    using SolidUtilities.Attributes;
    using SolidUtilities.UnityEngineInternals;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CreateGenericAssetMenu(FileName = "New Constant", MenuName = Config.PackageName + "Constant")]
    [Serializable]
    public class Constant<T> : ValueBase, IEquatable<Constant<T>>, IEquatable<T>
    {
        [ResizableTextArea, UsedImplicitly]
        [SerializeField] private string _description;

        [SerializeField] internal T _initialValue;
        [SerializeField] internal T _value;

        public T Value => _value;

        protected override void InitializeValues() => _value = _initialValue.DeepCopy();

        public static implicit operator T(Constant<T> variable) => variable.Value;

        public override string ToString() => $"Constant{{{Value}}}";

        public bool Equals(Constant<T> other)
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

            if (obj is Constant<T> typedObj)
                return Equals(typedObj);

            if (obj is T tObj)
                return Equals(tObj);

            return false;
        }

        /// <summary>
        /// Use with caution. The value contained by a Constant instance can be changed through inspector.
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

        public static bool operator ==(Constant<T> lhs, Constant<T> rhs)
        {
            if ((Object)lhs == null)
            {
                return (Object)rhs == null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Constant<T> lhs, Constant<T> rhs)
        {
            return ! (lhs == rhs);
        }

        public static bool operator ==(Constant<T> lhs, T rhs)
        {
            if ((Object)lhs == null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Constant<T> lhs, T rhs)
        {
            return ! (lhs == rhs);
        }
    }
}