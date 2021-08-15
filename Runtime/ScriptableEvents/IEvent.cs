namespace GenericScriptableArchitecture
{
    using System;

    public interface IEvent { }

    public interface IEvent<T> { }

    public interface IEvent<T1, T2> { }

    public interface IEvent<T1, T2, T3> { }
}