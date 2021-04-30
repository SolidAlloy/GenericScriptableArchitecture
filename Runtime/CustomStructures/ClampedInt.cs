namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ClampedInt : IEquatable<int>
    {
        [SerializeField] private int _min;

        [SerializeField] private int _max;

        [SerializeField] private int _value;

        public int Value
        {
            get => _value;
            set => _value = Mathf.Clamp(value, _min, _max);
        }

        public ClampedInt(int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(max),
                    $"Min argument must be less or equal the max argument ({max}), passed {min} instead.");

            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, _min, _max);
        }

        public static implicit operator int(ClampedInt clampedInt) => clampedInt.Value;

        public bool Equals(ClampedInt other)
        {
            return _value == other._value;
        }

        public bool Equals(int other)
        {
            return _value == other;
        }

        public override bool Equals(object obj)
        {
            return obj is ClampedInt other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static bool operator ==(ClampedInt lhs, ClampedInt rhs) => lhs.Equals(rhs);

        public static bool operator !=(ClampedInt lhs, ClampedInt rhs) => ! (lhs == rhs);
    }
}