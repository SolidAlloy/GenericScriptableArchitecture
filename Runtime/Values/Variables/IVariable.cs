namespace GenericScriptableArchitecture
{
    using System;

#if UNIRX
    using UniRx;
#endif

    public interface IVariable
    {
        internal VariableHelper VariableHelper { get; }
    }

    public interface IVariable<T> : IVariable, IEquatable<T>, IEvent<T>, IEquatable<IVariable<T>>
#if UNIRX
        , IReactiveProperty<T>
#endif
    {
        T InitialValue { get; }

#if UNIRX
        new // This property is also declared in IReactiveProperty<T>
#endif
        T Value { get; set; }

        void SetValueAndForceNotify(T value);

        void AddListener(IListener<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(IListener<T> listener);

        void AddListener(Action<T> listener, bool notifyCurrentValue = false);

        void RemoveListener(Action<T> listener);
    }
}