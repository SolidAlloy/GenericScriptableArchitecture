namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    public abstract class Reference
    {
        [field: SerializeField] public ValueTypes ValueType { get; protected set; }

        public enum ValueTypes { Value, Constant, Variable }
    }
}