namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class EventHolder
    {
        [SerializeField] public ScriptableEvent _event;
        [SerializeField] public EventInstancer _eventInstancer;
        [SerializeField] public EventType _type = EventType.ScriptableEvent;

        public void AddListener(VoidScriptableEventListener listener)
        {
            switch (_type)
            {
                case EventType.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventType.EventInstancer:
                    _eventInstancer?.AddListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }

        public void RemoveListener(VoidScriptableEventListener listener)
        {
            switch (_type)
            {
                case EventType.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventType.EventInstancer:
                    _eventInstancer?.RemoveListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }

        public enum EventType
        {
            ScriptableEvent,
            Variable,
            VariableInstancer,
            EventInstancer // appending this type to the end of enum so that the serialization does not mess up when people upgrade the package.
        }
    }

    internal static class EventTypesExtensions
    {
        public static bool HasDefaultValue(this EventHolder.EventType eventType)
        {
            switch (eventType)
            {
                case EventHolder.EventType.Variable:
                case EventHolder.EventType.VariableInstancer:
                    return true;
                default:
                    return false;
            }
        }
    }
}