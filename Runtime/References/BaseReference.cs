namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    public abstract class ReferenceBase
    {
        [SerializeField] protected ValueTypes ValueType = ValueTypes.Value;

        public enum ValueTypes { Value, Constant, Variable }
    }
}