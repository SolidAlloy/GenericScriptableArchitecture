namespace GenericScriptableArchitecture
{
    public interface IMultipleEventsListener : IListener
    {
        public void OnEventInvoked(IEvent invokedEvent);
    }

    public interface IMultipleEventsListener<T> : IListener<T>
    {
        public void OnEventInvoked(IEvent<T> invokedEvent, T argument);
    }

    public interface IMultipleEventsListener<T1, T2> : IListener<T1, T2>
    {
        public void OnEventInvoked(IEvent<T1, T2> invokedEvent, T1 arg0, T2 arg1);
    }

    public interface IMultipleEventsListener<T1, T2, T3> : IListener<T1, T2, T3>
    {
        public void OnEventInvoked(IEvent<T1, T2, T3> invokedEvent, T1 arg0, T2 arg1, T3 arg2);
    }
}