namespace GenericScriptableArchitecture
{
    public enum EventTypes
    {
        ScriptableEvent,
        Variable,
        VariableInstancer,
        EventInstancer // appending this type to the end of enum so that the serialization does not mess up when people upgrade the package.
    }

    public static class EventTypesExtensions
    {
        public static bool HasDefaultValue(this EventTypes eventType)
        {
            switch (eventType)
            {
                case EventTypes.Variable:
                case EventTypes.VariableInstancer:
                    return true;
                default:
                    return false;
            }
        }
    }
}