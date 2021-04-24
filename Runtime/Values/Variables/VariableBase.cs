namespace GenericScriptableArchitecture
{
    using System.Collections.Generic;

    public abstract class VariableBase : ValueBase
    {
        protected static readonly List<ScriptableEventListenerBase> EmptyList = new List<ScriptableEventListenerBase>();

        internal abstract void InvokeValueChangedEvents();

        internal abstract List<ScriptableEventListenerBase> Listeners { get; }

        internal abstract List<ScriptableEventListenerBase> ListenersWithHistory { get; }
    }
}