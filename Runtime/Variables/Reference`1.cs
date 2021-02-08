namespace ExtendedScriptableObjects
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Reference<T> : Reference
    {
        [SerializeField] private T _constantValue;
        [SerializeField] private Variable<T> _variable;

        public Reference() { }

        public Reference(T value)
        {
            _useConstant = true;
            _constantValue = value;
        }

        public T Value => _useConstant ? _constantValue : _variable.Value;

        public static implicit operator T(Reference<T> reference) => reference.Value;

        public static implicit operator Reference<T>(T value) => new Reference<T>(value);

        public override string ToString() => _useConstant ? _constantValue.ToString() : _variable.ToString();
    }

    [Serializable]
    public abstract class Reference
    {
        [SerializeField] protected bool _useConstant = true;
    }
}