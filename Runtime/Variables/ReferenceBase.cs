namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    public abstract class ReferenceBase
    {
        [SerializeField] protected bool _useConstant = true;
    }
}