namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class EventHolder<T1, T2, T3>
    {
        [SerializeField] private ScriptableEvent<T1, T2, T3> _event;
        [SerializeField] private EventInstancer<T1, T2, T3> _eventInstancer;

        [SerializeField] private EventHolder.EventType _type = EventHolder.EventType.ScriptableEvent;

        public void AddListener(ScriptableEventListener<T1, T2, T3> listener)
        {
            switch (_type)
            {
                case EventHolder.EventType.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventHolder.EventType.EventInstancer:
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
                case EventHolder.EventType.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventHolder.EventType.EventInstancer:
                    _eventInstancer?.RemoveListener(listener);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener), "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}