namespace GenericScriptableArchitecture
{
    public interface IEventListener
    {
        public void OnEventRaised();
    }

    public interface IEventListener<T>
    {
        public void OnEventRaised(T argument);
    }

    public interface IEventListener<T1, T2>
    {
        public void OnEventRaised(T1 arg0, T2 arg1);
    }

    public interface IEventListener<T1, T2, T3>
    {
        public void OnEventRaised(T1 arg0, T2 arg1, T3 arg2);
    }
}