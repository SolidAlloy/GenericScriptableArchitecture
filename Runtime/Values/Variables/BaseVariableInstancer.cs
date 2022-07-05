namespace GenericScriptableArchitecture
{
    using UnityEngine;

    public abstract class BaseVariableInstancer : MonoBehaviour
    {
        internal abstract BaseVariable Variable { get; set; }
    }
}