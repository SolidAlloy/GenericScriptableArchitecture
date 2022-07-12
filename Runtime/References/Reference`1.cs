namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable]
    public abstract class BaseReference
    {
        [SerializeField, FormerlySerializedAs("ValueType")] internal ValueType _valueType;

        public ValueType Type => _valueType;

        public enum ValueType { Value, Constant, Variable, VariableInstancer }
    }

    [Serializable]
    public class Reference<T> : BaseReference, IEquatable<Reference<T>>, IEquatable<T>
    {
        [SerializeField] internal T _value;
        [SerializeField] internal Variable<T> _variable;
        [SerializeField] internal Constant<T> _constant;
        [SerializeField] internal VariableInstancer<T> _variableInstancer;

        public Reference(T value)
        {
            _valueType = ValueType.Value;
            _value = value;
        }

        public Reference(Variable<T> variable)
        {
            _valueType = ValueType.Variable;
            _variable = variable;
        }

        public Reference(Constant<T> constant)
        {
            _valueType = ValueType.Constant;
            _constant = constant;
        }

        public T Value
        {
            get
            {
                return Type switch
                {
                    ValueType.Constant => _constant == null ? default : _constant.Value,
                    ValueType.Value => _value,
                    ValueType.Variable => _variable == null ? default : _variable.Value,
                    ValueType.VariableInstancer => _variableInstancer == null ? default : _variableInstancer.Value,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.")
                };
            }

            set
            {
                switch (Type)
                {
                    case ValueType.Constant:
                        throw new InvalidOperationException("Unable to change the value of a constant behind the reference");

                    case ValueType.Value:
                        _value = value;
                        break;

                    case ValueType.Variable:
                        _variable.Value = value;
                        break;

                    case ValueType.VariableInstancer:
                        _variableInstancer.Value = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.");
                }
            }
        }

        /// <summary>
        /// Returns a variable assigned to the reference if the <seealso cref="BaseReference.Type"/> is Variable, otherwise throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="BaseReference.Type"/> is not Variable.</exception>
        [PublicAPI]
        public Variable<T> VariableValue
        {
            get
            {
                if (Type == ValueType.Variable)
                    return _variable;

                throw new InvalidOperationException($"Tried to get a variable value of reference but the value type was {Type}.");
            }
        }

        /// <summary>
        /// Returns a constant assigned to the reference if the <seealso cref="BaseReference.Type"/> is Constant, otherwise throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="BaseReference.Type"/> is not Constant.</exception>
        [PublicAPI]
        public Constant<T> ConstantValue
        {
            get
            {
                if (Type == ValueType.Constant)
                    return _constant;

                throw new InvalidOperationException($"Tried to get a constant value of reference but the value type was {Type}.");
            }
        }

        /// <summary>
        /// Returns a variable instancer assigned to the reference if the <seealso cref="BaseReference.Type"/> is VariableInstancer, otherwise throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="BaseReference.Type"/> is not Constant.</exception>
        [PublicAPI]
        public VariableInstancer<T> InstancerValue
        {
            get
            {
                if (Type == ValueType.VariableInstancer)
                    return _variableInstancer;

                throw new InvalidOperationException($"Tried to get a variable instancer value of reference but the value type was {Type}.");
            }
        }

        public static implicit operator T(Reference<T> reference) => reference.Value;

        public static implicit operator Reference<T>(T value) => new Reference<T>(value);

        public override string ToString()
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format("Reference{{{0}}}", Type switch
            {
                ValueType.Constant => _constant.ToString(),
                ValueType.Value => _value.ToString(),
                ValueType.Variable => _variable.ToString(),
                ValueType.VariableInstancer => _variableInstancer.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.")
            });
        }

        public bool Equals(Reference<T> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value.Equals(other.Value);
        }

        public bool Equals(T other)
        {
            return other is { } && Value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is Reference<T> typedObj)
                return Equals(typedObj);

            if (obj is T tObj)
                return Equals(tObj);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Value?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static bool operator ==(Reference<T> lhs, Reference<T> rhs)
        {
            return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
        }

        public static bool operator !=(Reference<T> lhs, Reference<T> rhs)
        {
            return ! (lhs == rhs);
        }

        public static bool operator ==(Reference<T> lhs, T rhs)
        {
            if (lhs is null)
            {
                return rhs is null;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Reference<T> lhs, T rhs)
        {
            return ! (lhs == rhs);
        }
    }
}