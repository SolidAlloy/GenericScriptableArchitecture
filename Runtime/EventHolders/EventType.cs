namespace GenericScriptableArchitecture
{
    public enum EventType
    {
        ScriptableEvent,
        Variable,
        VariableInstancer,
        EventInstancer // appending this type to the end of enum so that the serialization does not mess up when people upgrade the package.
    }

    public static class EventTypesExtensions
    {
        public static bool HasDefaultValue(this EventType eventType)
        {
            switch (eventType)
            {
                case EventType.Variable:
                case EventType.VariableInstancer:
                    return true;
                default:
                    return false;
            }
        }
    }
}