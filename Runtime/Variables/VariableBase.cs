namespace GenericScriptableArchitecture
{
    using GenericUnityObjects;

    public abstract class VariableBase : GenericScriptableObject
    {
        internal abstract void InvokeValueChangedEvents();
    }
}