namespace GenericScriptableArchitecture
{
    using System;

    public interface IVariableWithHistory : IVariable
    {
        internal VariableHelperWithHistory VariableHelperWithHistory { get; }
    }

    public interface IVariableWithHistory<T> : IVariableWithHistory, IEvent<T, T>
#if UNIRX
        , IReactivePropertyWithHistory<T>
#endif
    {
        bool HasPreviousValue { get; }

        void AddListener(IListener<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IListener<T, T> listener);

        void AddListener(Action<T, T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T, T> listener);
    }
}