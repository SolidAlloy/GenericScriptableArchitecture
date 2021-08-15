namespace GenericScriptableArchitecture
{
    public interface IEventListener
    {
        public void OnEventInvoked();
    }

    public interface IEventListener<in T>
    {
        public void OnEventInvoked(T argument);
    }

    public interface IEventListener<in T1, in T2>
    {
        public void OnEventInvoked(T1 arg0, T2 arg1);
    }

    public interface IEventListener<in T1, in T2, in T3>
    {
        public void OnEventInvoked(T1 arg0, T2 arg1, T3 arg2);
    }
}