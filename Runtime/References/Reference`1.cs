namespace GenericScriptableArchitecture
{
    using System;
    using JetBrains.Annotations;
    using UnityEngine;

    [Serializable]
    public class Reference<T> : Reference, IEquatable<Reference<T>>, IEquatable<T>
    {
        [SerializeField] internal T _value;
        [SerializeField] internal Variable<T> _variable;
        [SerializeField] internal Constant<T> _constant;
        [SerializeField] internal VariableInstancer<T> _variableInstancer;

        public Reference(T value)
        {
            ValueType = ValueTypes.Value;
            _value = value;
        }

        public Reference(Variable<T> variable)
        {
            ValueType = ValueTypes.Variable;
            _variable = variable;
        }

        public Reference(Constant<T> constant)
        {
            ValueType = ValueTypes.Constant;
            _constant = constant;
        }

        public T Value
        {
            get
            {
                return ValueType switch
                {
                    ValueTypes.Constant => _constant == null ? default : _constant.Value,
                    ValueTypes.Value => _value,
                    ValueTypes.Variable => _variable == null ? default : _variable.Value,
                    ValueTypes.VariableInstancer => _variableInstancer == null ? default : _variableInstancer.Value,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.")
                };
            }

            set
            {
                switch (ValueType)
                {
                    case ValueTypes.Constant:
                        throw new InvalidOperationException("Unable to change the value of a constant behind the reference");

                    case ValueTypes.Value:
                        _value = value;
                        break;

                    case ValueTypes.Variable:
                        _variable.Value = value;
                        break;

                    case ValueTypes.VariableInstancer:
                        _variableInstancer.Value = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.");
                }
            }
        }

        /// <summary>
        /// Returns a variable assigned to the reference if the <seealso cref="Reference.ValueType"/> is Variable, otherwise throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="Reference.ValueType"/> is not Variable.</exception>
        [PublicAPI]
        public Variable<T> VariableValue
        {
            get
            {
                if (ValueType == ValueTypes.Variable)
                    return _variable;

                throw new InvalidOperationException($"Tried to get a variable value of reference but the value type was {ValueType}.");
            }
        }

        /// <summary>
        /// Returns a constant assigned to the reference if the <seealso cref="Reference.ValueType"/> is Constant, otherwise throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="Reference.ValueType"/> is not Constant.</exception>
        [PublicAPI]
        public Constant<T> ConstantValue
        {
            get
            {
                if (ValueType == ValueTypes.Constant)
                    return _constant;

                throw new InvalidOperationException($"Tried to get a constant value of reference but the value type was {ValueType}.");
            }
        }

        /// <summary>
        /// Returns a variable instancer assigned to the reference if the <seealso cref="Reference.ValueType"/> is VariableInstancer, otherwise throws an exception.
        /// </summary>
        /// <exception cref="InvalidOperationException"><seealso cref="Reference.ValueType"/> is not Constant.</exception>
        [PublicAPI]
        public VariableInstancer<T> InstancerValue
        {
            get
            {
                if (ValueType == ValueTypes.VariableInstancer)
                    return _variableInstancer;

                throw new InvalidOperationException($"Tried to get a variable instancer value of reference but the value type was {ValueType}.");
            }
        }

        public static implicit operator T(Reference<T> reference) => reference.Value;

        public static implicit operator Reference<T>(T value) => new Reference<T>(value);

        public override string ToString()
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format("Reference{{{0}}}", ValueType switch
            {
                ValueTypes.Constant => _constant.ToString(),
                ValueTypes.Value => _value.ToString(),
                ValueTypes.Variable => _variable.ToString(),
                ValueTypes.VariableInstancer => _variableInstancer.ToString(),
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