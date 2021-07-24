namespace GenericScriptableArchitecture
{
    public interface IMultipleEventsListener
    {
        public void OnEventRaised(ScriptableEvent scriptableEvent);
    }

    public interface IMultipleEventsListener<T>
    {
        public void OnEventRaised(ScriptableEvent<T> scriptableEvent, T argument);
    }

    public interface IMultipleEventsListener<T1, T2>
    {
        public void OnEventRaised(ScriptableEvent<T1, T2> scriptableEvent, T1 arg0, T2 arg1);
    }

    public interface IMultipleEventsListener<T1, T2, T3>
    {
        public void OnEventRaised(ScriptableEvent<T1, T2, T3> scriptableEvent, T1 arg0, T2 arg1, T3 arg2);
    }
}