namespace GenericScriptableArchitecture
{
    using System;
    using System.Globalization;
    using UnityEngine;

    [Serializable]
    public struct ClampedFloat : IEquatable<float>, IEquatable<ClampedFloat>
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;
        [SerializeField] private float _value;

        public float Min => _min;

        public float Max => _max;

        public float Value
        {
            get => _value;
            set => _value = Mathf.Clamp(value, _min, _max);
        }

        public ClampedFloat(float value, float min, float max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException(nameof(max), $"Min argument must be less or equal the max argument ({max}), passed {min} instead.");

            _min = min;
            _max = max;
            _value = Mathf.Clamp(value, _min, _max);
        }

        public static implicit operator float(ClampedFloat clampedFloat) => clampedFloat.Value;

        public bool Equals(ClampedFloat other) => _value == other._value;

        public bool Equals(float other) => _value == other;

        public override bool Equals(object obj)
        {
            return obj is ClampedFloat other && Equals(other);
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => _value.ToString(CultureInfo.InvariantCulture);

        public static bool operator ==(ClampedFloat lhs, ClampedFloat rhs) => lhs.Equals(rhs);

        public static bool operator !=(ClampedFloat lhs, ClampedFloat rhs) => ! (lhs == rhs);
    }
}