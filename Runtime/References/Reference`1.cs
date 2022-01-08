namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Reference<T> : ReferenceBase, IEquatable<Reference<T>>, IEquatable<T>
    {
        [SerializeField] private T _value;
        [SerializeField] private Variable<T> _variable;
        [SerializeField] private Constant<T> _constant;


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

                    default:
                        throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.");
                }
            }
        }

        public static implicit operator T(Reference<T> reference) => reference.Value;

        public static implicit operator Reference<T>(T value) => new Reference<T>(value);

        public override string ToString()
        {
            return ValueType switch
            {
                ValueTypes.Constant => _constant.ToString(),
                ValueTypes.Value => _value.ToString(),
                ValueTypes.Variable => _variable.ToString(),
                _ => throw new ArgumentOutOfRangeException(nameof(Value), "Unknown value type in the reference.")
            };
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