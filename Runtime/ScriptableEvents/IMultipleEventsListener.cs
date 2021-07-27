namespace GenericScriptableArchitecture
{
    public interface IMultipleEventsListener
    {
        public void OnEventRaised(IEvent raisedEvent);
    }

    public interface IMultipleEventsListener<T>
    {
        public void OnEventRaised(IEvent<T> raisedEvent, T argument);
    }

    public interface IMultipleEventsListener<T1, T2>
    {
        public void OnEventRaised(IEvent<T1, T2> raisedEvent, T1 arg0, T2 arg1);
    }

    public interface IMultipleEventsListener<T1, T2, T3>
    {
        public void OnEventRaised(IEvent<T1, T2, T3> raisedEvent, T1 arg0, T2 arg1, T3 arg2);
    }
}