namespace GenericScriptableArchitecture
{
    public enum EventTypes { ScriptableEvent, Variable, VariableInstancer }

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