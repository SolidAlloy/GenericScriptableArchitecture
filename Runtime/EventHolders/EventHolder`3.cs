namespace GenericScriptableArchitecture
{
    using System;
    using System.IO;
    using UnityEngine;

    [Serializable]
    internal class EventHolder<T1, T2, T3>
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private EventInstancer<T1, T2, T3> _eventInstancer;

        [SerializeField] private EventType _type = EventType.ScriptableEvent;

        public IBaseEvent Event
        {
            get
            {
                return _type switch
                {
                    EventType.ScriptableEvent => _event,
                    EventType.EventInstancer => _eventInstancer,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            set
            {
                if (value is ScriptableEvent<T1, T2, T3> @event)
                {
                    _type = EventType.ScriptableEvent;
                    _event = @event;
                }
                else if (value is EventInstancer<T1, T2, T3> eventInstancer)
                {
                    _type = EventType.EventInstancer;
                    _eventInstancer = eventInstancer;
                }
                else
                {
                    throw new InvalidDataException($"Tried to set an event that is neither of EventTypes: {value.GetType()}");
                }
            }
        }

        public void AddListener(ScriptableEventListener<T1, T2, T3> listener)
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

        public void RemoveListener(ScriptableEventListener<T1, T2, T3> listener)
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
    }
}