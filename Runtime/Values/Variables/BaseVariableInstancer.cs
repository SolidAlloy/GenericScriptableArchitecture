﻿namespace GenericScriptableArchitecture
{
    using UnityEngine;

    public abstract class BaseVariableInstancer : MonoBehaviour
    {
        internal abstract BaseVariable BaseVariableReference { get; set; }

        internal abstract bool Initialized { get; }
    }
}