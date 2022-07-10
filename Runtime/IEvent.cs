namespace GenericScriptableArchitecture
{
    public interface IBaseEvent { }

    public interface IEvent : IBaseEvent { }

    public interface IEvent<T> : IBaseEvent { }

    public interface IEvent<T1, T2> : IBaseEvent { }

    public interface IEvent<T1, T2, T3> : IBaseEvent { }
}