namespace GenericScriptableArchitecture
{
    public interface IListener { }

    public interface IListener<in T> { }

    public interface IListener<in T1, in T2> { }

    public interface IListener<in T1, in T2, in T3> { }
}