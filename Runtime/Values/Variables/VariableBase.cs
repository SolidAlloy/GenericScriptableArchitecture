namespace GenericScriptableArchitecture
{
    using GenericUnityObjects;

    public abstract class VariableBase : ValueBase
    {
        internal abstract void InvokeValueChangedEvents();
    }
}