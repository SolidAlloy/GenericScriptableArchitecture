namespace GenericScriptableArchitecture
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class EventHolder<T1, T2> : EventHolderBaseTwo
    {
        [SerializeField] private ScriptableEvent<T1, T2> _event;
        [SerializeField] private VariableWithHistory<T1> _variable;
        [SerializeField] private EventTypes _type = EventTypes.ScriptableEvent;

        public void AddListener(ScriptableEventListener<T1, T2> listener)
        {
            switch (_type)
            {
                case EventTypes.ScriptableEvent:
                    _event?.AddListener(listener);
                    break;

                case EventTypes.Variable:
                    _variable?.AddListenerOnChangeWithHistory(listener as ScriptableEventListener<T1, T1>);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener),
                        "Unknown enum value when adding listener to the event holder.");
            }
        }

        public void RemoveListener(ScriptableEventListener<T1, T2> listener)
        {
            switch (_type)
            {
                case EventTypes.ScriptableEvent:
                    _event?.RemoveListener(listener);
                    break;

                case EventTypes.Variable:
                    _variable?.RemoveListenerOnChangeWithHistory(listener as ScriptableEventListener<T1, T1>);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(listener),
                        "Unknown enum value when adding listener to the event holder.");
            }
        }
    }
}